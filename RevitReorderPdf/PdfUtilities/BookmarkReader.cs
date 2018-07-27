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

using iTextSharp.text.pdf;
using System.Collections.Generic;

namespace RevitReorderPdf.PdfUtilities
{
    class BookmarkReader
    {
        IList<Dictionary<string, object>> Bookmarks { get; }

        public BookmarkReader(PdfReader reader)
        {
            Bookmarks = SimpleBookmark.GetBookmark(reader);
        }

        public IList<Dictionary<string, object>> GetBookmarksForPage(int pageNumber)
        {
            return GetBookmarksForPage(Bookmarks, pageNumber);
        }

        private IList<Dictionary<string, object>> GetBookmarksForPage(IList<Dictionary<string, object>> bookmarks, int pageNumber)
        {
            var returnValue = new List<Dictionary<string, object>>();

            if (bookmarks == null) { return returnValue; }

            foreach (var bookmark in bookmarks)
            {
                object page;
                bookmark.TryGetValue("Page", out page);
                if (page != null)
                {
                    var targetPage = int.Parse(((string)page).Split(' ')[0]);
                    if (targetPage == pageNumber)
                    {
                        returnValue.Add(bookmark);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.Print("No target page");
                }
                object children;
                bookmark.TryGetValue("Kids", out children);
                returnValue.AddRange(GetBookmarksForPage((IList<Dictionary<string, object>>)children, pageNumber));
            }

            return returnValue;
        }
    }
}
