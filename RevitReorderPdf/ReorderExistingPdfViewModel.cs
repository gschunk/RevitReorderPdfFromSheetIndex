/*
   Copyright (C) 2018 Pheinex LLC

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>
 */

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace RevitReorderPdf
{
    class ReorderExistingPdfViewModel : ObservableObject
    {
        private static string defaultSelectedPdfFile = "Select PDF file for reordering.";

        private ColumnEntry inclusionExclusionColumn;
        private ColumnEntry sortColumn;
        private ViewSchedule[] schedules;
        private ViewSchedule selectedSchedule;
        private ViewSheet[] sheets;

        private string selectedPdfFile = defaultSelectedPdfFile;

        protected Document Document { get; }

        protected IErrorLogger Logger { get; }

        public bool CanSelectInclusionColumn
        {
            get { return ReorderExistingPdfCommand.CanHaveInclusionColumn; }
        }

        public bool CanReorder
        {
            get
            {
                return
                    SelectedSchedule != null &&
                    SelectedPdfFile != defaultSelectedPdfFile &&
                    SortColumn != null &&
                    File.Exists(SelectedPdfFile);
            }
        }

        /// <summary>
        /// Schedule columns that may be used for determination of inclusion or exclusion of
        /// a sheet in the published set.
        /// </summary>
        public IEnumerable<ColumnEntry> InclusionColumns
        {
            get
            {
                if (SelectedSchedule == null) { return new ColumnEntry[0]; }

                var parameters = new List<ColumnEntry>();
                parameters.Add(new NullColumnEntry());
                try
                {
                    var definition = SelectedSchedule.Definition;
                    foreach (var fieldId in definition.GetFieldOrder())
                    {
                        var field = definition.GetField(fieldId);
                        var parameter = GetParameter(field);
                        if (CanBeInclusionColumn(parameter))
                        {
                            parameters.Add(new ColumnEntry(parameter));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Error", ex.Message);
                }

                return parameters;
            }
        }

        public ColumnEntry InclusionExclusionColumn
        {
            get { return inclusionExclusionColumn; }
            set
            {
                var newValue = value is NullColumnEntry ? null : value;
                SetValue(ref inclusionExclusionColumn, newValue, nameof(InclusionExclusionColumn));
            }
        }

        public ReorderOptions ReorderOptions
        {
            get
            {
                if (SelectedSchedule == null) { return null; }

                var unsortedSheets =
                    new FilteredElementCollector(Document)
                    .OfClass(typeof(ViewSheet))
                    .Cast<ViewSheet>()
                    .Where(vs => SheetIsForPublish(vs))
                    .ToArray();

                return new ReorderOptions(
                    SelectedSchedule,
                    SheetsForPublish.Select(se => se.Sheet).ToArray(),
                    unsortedSheets,
                    InclusionExclusionColumn,
                    SortColumn,
                    SelectedPdfFile);
            }
        }

        public ColumnEntry SortColumn
        {
            get { return sortColumn; }
            set
            {
                var newValue = value is NullColumnEntry ? null : value;
                SetValue(ref sortColumn, newValue, nameof(SortColumn));
            }
        }

        public IEnumerable<ColumnEntry> SortColumns
        {
            get
            {
                if (SelectedSchedule == null) { return new ColumnEntry[0]; }

                var parameters = new List<ColumnEntry>();
                parameters.Add(new NullColumnEntry());
                try
                {
                    var definition = SelectedSchedule.Definition;
                    foreach (var fieldId in definition.GetFieldOrder())
                    {
                        var field = definition.GetField(fieldId);
                        var parameter = GetParameter(field);
                        if (CanBeSortColumn(parameter))
                        {
                            parameters.Add(new ColumnEntry(parameter));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Error", ex.Message);
                }

                return parameters;
            }
        }

        public ViewSchedule[] Schedules
        {
            get
            {
                if (schedules == null)
                {
                    schedules =
                        new FilteredElementCollector(Document)
                        .OfClass(typeof(ViewSchedule))
                        .Cast<ViewSchedule>()
                        .Where(vs => IsSheetList(vs))
                        .ToArray();
                }
                return schedules;
            }
        }

        public ViewSchedule SelectedSchedule
        {
            get { return selectedSchedule; }
            set { SetValue(ref selectedSchedule, value, nameof(SelectedSchedule)); }
        }

        public string SelectedPdfFile
        {
            get { return selectedPdfFile; }
            set { SetValue(ref selectedPdfFile, value, nameof(SelectedPdfFile)); }
        }

        public ViewSheet[] Sheets
        {
            get
            {
                if (sheets == null)
                {
                    sheets = new FilteredElementCollector(Document)
                            .OfClass(typeof(ViewSheet))
                            .Cast<ViewSheet>().ToArray();
                }
                return sheets;
            }
        }

        public SheetEntry[] SheetsForPublish
        {
            get
            {
                if (SelectedSchedule == null) { return new SheetEntry[0]; }

                var sheetsForPublish
                    = new FilteredElementCollector(Document)
                        .OfClass(typeof(ViewSheet))
                        .Cast<ViewSheet>()
                        .Where(vs => SheetIsForPublish(vs))
                        .Select(vs => new SheetEntry(vs, SortColumn?.Parameter?.Definition));

                return sheetsForPublish.OrderBy(se => se.Sequence).ThenBy(se => se.SheetNumber).ToArray();
            }
        }

        public ReorderExistingPdfViewModel(Document document, IErrorLogger logger)
        {
            this.Document = document;
            this.Logger = logger;

            if (ReorderExistingPdfCommand.ReorderOptions?.Schedule == null)
            {
                this.SelectedSchedule = Schedules.FirstOrDefault();
            }
            else
            {
                this.SelectedSchedule = Schedules.Where(schedule => schedule.ViewName == ReorderExistingPdfCommand.ReorderOptions.Schedule.ViewName).FirstOrDefault();
                this.InclusionExclusionColumn = ReorderExistingPdfCommand.ReorderOptions.InclusionColumn;
                this.SortColumn = ReorderExistingPdfCommand.ReorderOptions.SortColumn;
                this.SelectedPdfFile = ReorderExistingPdfCommand.ReorderOptions.PdfFileName;
            }

            this.PropertyChanged += ReorderExistingPdfViewModel_PropertyChanged;
        }

        private void ReorderExistingPdfViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedPdfFile):
                    OnPropertyChanged(nameof(CanReorder));
                    break;
                case nameof(SelectedSchedule):
                    OnPropertyChanged(nameof(CanReorder));
                    OnPropertyChanged(nameof(InclusionColumns));
                    OnPropertyChanged(nameof(SortColumns));
                    OnPropertyChanged(nameof(SheetsForPublish));
                    break;
                case nameof(InclusionExclusionColumn):
                    OnPropertyChanged(nameof(SheetsForPublish));
                    break;
                case nameof(SortColumn):
                    OnPropertyChanged(nameof(SheetsForPublish));
                    break;
            }
        }

        protected bool SheetIsForPublish(ViewSheet sheet)
        {
            if (InclusionExclusionColumn == null) { return true; }

            var inclusionParameter = sheet.get_Parameter(InclusionExclusionColumn.Parameter.Definition);

            return !inclusionParameter.HasValue || inclusionParameter.AsInteger() == 1;
        }

        /// <summary>
        /// Tests if the specified parameter is eligible to be used as the column for
        /// specifying if a sheet is included in the set.
        /// </summary>
        /// <param name="parameter">The parameter to be tested</param>
        /// <returns></returns>
        protected bool CanBeInclusionColumn(Parameter parameter)
        {
            if (parameter == null) { return false; }

            return parameter.Definition.ParameterType == ParameterType.YesNo;
        }

        protected bool CanBeSortColumn(Parameter parameter)
        {
            if (parameter == null) { return false; }

            return parameter.Definition.ParameterType == ParameterType.Integer;
        }

        protected bool IsSheetNumberParameter(Parameter parameter)
        {
            try
            {
                if (parameter == null) { return false; }
                if (Sheets.Length == 0) { return false; }

                var sheetNumberParameter = Sheets.First().get_Parameter(BuiltInParameter.SHEET_NUMBER);
                if (parameter?.Id == sheetNumberParameter.Id)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error", ex.Message);
            }
            return false;
        }

        protected bool IsSheetList(ViewSchedule schedule)
        {
            try
            {
                if (Sheets.Length == 0) { return false; }

                var definition = schedule.Definition;
                foreach (var fieldId in definition.GetFieldOrder())
                {
                    var field = definition.GetField(fieldId);
                    var parameter = GetParameter(field);
                    if (IsSheetNumberParameter(parameter))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error", ex.Message);
            }
            return false;
        }

        protected Parameter GetParameter(ScheduleField field)
        {
            var sheets =
                        new FilteredElementCollector(Document)
                        .OfClass(typeof(ViewSheet))
                        .Cast<ViewSheet>();

            Parameter parameter = null;
            foreach (var sheet in sheets)
            {
                parameter = GetParameter(sheet, field.ParameterId);
                break;
            }
            return parameter;
        }

        protected Parameter GetParameter(ViewSheet sheet, ElementId parameterId)
        {
            foreach (Parameter parameter in sheet.Parameters)
            {
                if (parameter.Id == parameterId)
                {
                    return parameter;
                }
            }
            return null;
        }
    }
}
