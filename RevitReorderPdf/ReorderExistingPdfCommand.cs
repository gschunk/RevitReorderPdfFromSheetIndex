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
using RevitReorderPdf.PdfUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public static bool CanHaveInclusionColumn { get; set; } = false;

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

                        var inputFilePath = ReorderOptions.PdfFileName;
                        var directory = Path.GetDirectoryName(inputFilePath);
                        var outputFileName = string.Format("{0} - Reordered.pdf", Path.GetFileNameWithoutExtension(inputFilePath));
                        var outputFilePath = Path.Combine(directory, outputFileName);

                        var sortMatrix = GetSortMatrix(ReorderOptions.UnsortedSheets, ReorderOptions.SortedSheets);

                        var reorderer = new PdfEditor();
                        reorderer.ReorderPdf(inputFilePath, outputFilePath, sortMatrix);

                        Process.Start(outputFilePath);
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

        /// <summary>
        /// Creates a dictionary mapping new page number to old page number for sheet reordering
        /// </summary>
        /// <param name="unsortedSheets"></param>
        /// <param name="sortedSheets"></param>
        /// <returns>A dictionay where the new page number is the key, and the old page number is the value</returns>
        private Dictionary<int, int> GetSortMatrix(ViewSheet[] unsortedSheets, ViewSheet[] sortedSheets)
        {
            if (unsortedSheets.Length != sortedSheets.Length)
            {
                throw new Exception("Lengths of sheets arrays to not match.");
            }

            var unsortedIndexes = new Dictionary<string, int>();
            for (int i = 0; i < unsortedSheets.Length; i++)
            {
                var currentPageNumber = i + 1;
                unsortedIndexes[unsortedSheets[i].SheetNumber] = currentPageNumber;
            }

            var sortIndexes = new Dictionary<int, int>();
            for (int i = 0; i < sortedSheets.Length; i++)
            {
                var newPageNumber = i + 1;
                var oldPageNumber = unsortedIndexes[sortedSheets[i].SheetNumber];
                sortIndexes[newPageNumber] = oldPageNumber;
            }

            return sortIndexes;
        }

    }
}
