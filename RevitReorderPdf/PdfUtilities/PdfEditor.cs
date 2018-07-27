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
using System;
using System.Collections.Generic;
using System.IO;

namespace RevitReorderPdf.PdfUtilities
{
    class PdfEditor
    {
        public void ReversePdf(string inputFile, string outputFile)
        {
            Document document = null;
            PdfCopy writer = null;
            PdfReader reader = null;
            try
            {
                document = new Document();
                writer = new PdfCopy(document, new FileStream(outputFile, FileMode.Create));
                if (writer == null)
                {
                    return;
                }

                document.Open();

                reader = new PdfReader(inputFile);

                var getter = new BookmarkReader(reader);
                var bookmarks = new List<Dictionary<string, object>>();

                var pages = 0;

                for (int i = reader.NumberOfPages; i > 0; i--)
                {
                    PdfImportedPage page = writer.GetImportedPage(reader, i);
                    writer.AddPage(page);
                    pages++;
                    var referencingBookmarks = getter.GetBookmarksForPage(i);
                    if (referencingBookmarks.Count > 0)
                    {
                        var oldPageNumber = i;
                        var newPageNumber = pages;
                        SimpleBookmark.ShiftPageNumbers(referencingBookmarks, newPageNumber - oldPageNumber, null);
                        foreach (var bk in referencingBookmarks)
                        {
                            bookmarks.Add(new Dictionary<string, object>(bk));
                        }
                    }
                }

                writer.Outlines = bookmarks;
            }
            finally
            {
                reader?.Dispose();
                writer?.Dispose();
                document?.Dispose();
            }
        }

        public void ReorderPdf(string inputFile, string outputFile, Dictionary<int, int> sortKeys)
        {
            Document document = null;
            PdfCopy writer = null;
            PdfReader reader = null;
            try
            {
                document = new Document();
                writer = new PdfCopy(document, new FileStream(outputFile, FileMode.Create));
                if (writer == null)
                {
                    return;
                }

                document.Open();

                reader = new PdfReader(inputFile);

                var getter = new BookmarkReader(reader);
                var bookmarks = new List<Dictionary<string, object>>();
                
                for (int newPageNumber = 1; newPageNumber <= sortKeys.Count; newPageNumber++)
                {
                    var oldPageNumber = sortKeys[newPageNumber];
                    PdfImportedPage page = writer.GetImportedPage(reader, oldPageNumber);
                    var referencingBookmarks = getter.GetBookmarksForPage(oldPageNumber);
                    writer.AddPage(page);
                    if (referencingBookmarks.Count > 0)
                    {
                        var shift = newPageNumber - oldPageNumber;
                        SimpleBookmark.ShiftPageNumbers(referencingBookmarks, shift, null);
                        foreach (var bk in referencingBookmarks)
                        {
                            bookmarks.Add(new Dictionary<string, object>(bk));
                        }
                    }
                }

                writer.Outlines = bookmarks;
            }
            finally
            {
                reader?.Dispose();
                writer?.Dispose();
                document?.Dispose();
            }
        }
    }
}
