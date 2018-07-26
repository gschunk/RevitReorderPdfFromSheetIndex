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

#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
#endregion

namespace RevitReorderPdf
{
    [Transaction(TransactionMode.Manual)]
    public class ReorderExistingPdfCommand : IExternalCommand
    {
        private ExternalCommandData CommandData { get; set; }
        private UIApplication UIApplication { get { return CommandData.Application; } }
        private UIDocument UIDocument { get { return UIApplication.ActiveUIDocument; } }
        private Application Application { get { return UIApplication.Application; } }
        private Document Document { get { return UIDocument.Document; } }

        public static ReorderOptions ReorderOptions { get; private set; }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            this.CommandData = commandData;

            try
            {
                using (var publishWindow = new ReorderPdfWindow(Document))
                {
                    publishWindow.SetRevitWindowAsOwner();
                    publishWindow.ShowDialog();

                    if (publishWindow.DialogResult == true)
                    {
                        ReorderOptions = publishWindow.ReorderOptions;
                        TaskDialog.Show("Info", "Do reorder!");
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
