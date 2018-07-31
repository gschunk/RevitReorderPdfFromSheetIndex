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

namespace RevitReorderPdf
{
    public class ReorderOptions
    {
        public ColumnEntry InclusionColumn { get; set; }
        public ViewSchedule Schedule { get; set; }
        public ViewSheet[] SortedSheets { get; set; }
        public ColumnEntry SortColumn { get; set; }
        public string PdfFileName { get; set; }
        public ViewSheet[] UnsortedSheets { get; set; }

        public ReorderOptions() { }

        public ReorderOptions(ViewSchedule schedule, ViewSheet[] sortedSheets, ViewSheet[] unsortedSheets, ColumnEntry inclusionColumn, ColumnEntry sortColumn, string pdfFileName)
        {
            this.InclusionColumn = inclusionColumn;
            this.Schedule = schedule;
            this.SortedSheets = sortedSheets;
            this.SortColumn = sortColumn;
            this.PdfFileName = pdfFileName;
            this.UnsortedSheets = unsortedSheets;
        }
    }
}
