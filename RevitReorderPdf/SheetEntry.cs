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
    public class SheetEntry
    {
        public ViewSheet Sheet { get; }

        public string SheetNumber
        {
            get
            {
                return Sheet.get_Parameter(BuiltInParameter.SHEET_NUMBER).AsString();
            }
        }

        public string SheetName
        {
            get
            {
                return Sheet.get_Parameter(BuiltInParameter.SHEET_NAME).AsString();
            }
        }

        public Definition SortParameter { get; }

        public int Sequence
        {
            get
            {
                if (SortParameter == null) { return 0; }

                return Sheet.get_Parameter(SortParameter).AsInteger();
            }
        }

        public SheetEntry(ViewSheet sheet, Definition sortParameter)
        {
            this.Sheet = sheet;
            this.SortParameter = sortParameter;
        }
    }
}
