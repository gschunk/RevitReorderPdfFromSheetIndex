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

using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;

namespace RevitReorderPdf
{
    class TaskDialogBoxErrorLogger : IErrorLogger
    {
        public void Log(string message)
        {
            TaskDialog.Show(GetAssemblyName(), message);
        }

        public void Log(string title, string message)
        {
            var fullTitle = string.Format("{0}: {1}", GetAssemblyName(), title);
            TaskDialog.Show(fullTitle, message);
        }

        private string GetAssemblyName()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetFileName(path);
        }
    }
}
