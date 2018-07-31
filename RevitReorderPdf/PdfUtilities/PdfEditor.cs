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

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RevitReorderPdf.PdfUtilities
{
    class PdfEditor
    {
        public void ReorderPdf(string inputFile, string outputFile, Dictionary<int, int> sortKeys)
        {
            Document document = null;
            PdfCopy writer = null;
            PdfReader reader = null;
            try
            {
                document = new Document();
                writer = new PdfCopy(document, new FileStream(outputFile, FileMode.Create));
                if (writer == null) { return; }

                document.Open();
                reader = new PdfReader(inputFile);

                CopyPages(reader, writer, sortKeys);
                CopyBookmarks(reader, writer, sortKeys);

            }
            finally
            {
                reader?.Dispose();
                writer?.Dispose();
                document?.Dispose();
            }
        }

        private void CopyPages(PdfReader reader, PdfCopy writer, Dictionary<int, int> sortKeys)
        {
            for (int newPageNumber = 1; newPageNumber <= sortKeys.Count; newPageNumber++)
            {
                var oldPageNumber = sortKeys[newPageNumber];
                PdfImportedPage page = writer.GetImportedPage(reader, oldPageNumber);
                writer.AddPage(page);
            }
        }

        private void CopyBookmarks(PdfReader reader, PdfCopy writer, Dictionary<int, int> sortKeys)
        {
            var outlines = SimpleBookmark.GetBookmark(reader);
            var reverseSortKeys = new Dictionary<int, int>();
            foreach (var newPageNumber in sortKeys.Keys)
            {
                var oldPageNumber = sortKeys[newPageNumber];
                reverseSortKeys.Add(oldPageNumber, newPageNumber);
            }

            UpdateTargetPages(outlines, reverseSortKeys);

            SortBookmarks(outlines);

            writer.Outlines = outlines;
        }

        private void UpdateTargetPages(IList<Dictionary<string, object>> existingOutlines, Dictionary<int, int> reverseSortKeys)
        {
            if (existingOutlines == null) { return; }

            foreach (var outline in existingOutlines)
            {
                object oldPageTarget;
                outline.TryGetValue("Page", out oldPageTarget);
                if (oldPageTarget != null)
                {
                    var oldPageNumber = int.Parse(((string)oldPageTarget).Split(' ')[0]);
                    var newPageNumber = reverseSortKeys[oldPageNumber];
                    var shift = newPageNumber - oldPageNumber;
                    var pageOutlines = new List<Dictionary<string, object>>();
                    pageOutlines.Add(outline);
                    SimpleBookmark.ShiftPageNumbers(pageOutlines, shift, null);
                }
                else
                {
                    System.Diagnostics.Debug.Print("No target page");
                }
                object children;
                outline.TryGetValue("Kids", out children);
                UpdateTargetPages(children as IList<Dictionary<string, object>>, reverseSortKeys);
            }
        }

        private void SortBookmarks(IList<Dictionary<string, object>> outlines)
        {
            if (outlines == null) { return; }

            var sortedOutlines = outlines.OrderBy(ol => GetOutlineTargetPage(ol)).ToArray();

            for (int i = 0; i < outlines.Count; i++)
            {
                outlines[i] = sortedOutlines[i];
                object children;
                outlines[i].TryGetValue("Kids", out children);
                SortBookmarks(children as IList<Dictionary<string, object>>);
            }
        }

        private int GetOutlineTargetPage(Dictionary<string, object> outline)
        {
            object pageTarget;
            outline.TryGetValue("Page", out pageTarget);
            if (pageTarget == null)
            {
                return 0;
            }
            else
            {
                var pageNumber = int.Parse(((string)pageTarget).Split(' ')[0]);
                return pageNumber;
            }
        }
    }
}
