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
        public ColumnEntry InclusionColumn { get; }
        public ViewSchedule Schedule { get; }
        public ViewSheet[] Sheets { get; }
        public ColumnEntry SortColumn { get; }

        public ReorderOptions(ViewSchedule schedule, ViewSheet[] sheets, ColumnEntry inclusionColumn, ColumnEntry sortColumn)
        {
            this.InclusionColumn = inclusionColumn;
            this.Schedule = schedule;
            this.Sheets = sheets;
            this.SortColumn = sortColumn;
        }
    }
}
