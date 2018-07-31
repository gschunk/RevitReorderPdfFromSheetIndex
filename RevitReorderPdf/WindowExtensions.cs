﻿/*
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

namespace RevitReorderPdf
{
    static class WindowExtensions
    {
        public static void SetRevitWindowAsOwner(this System.Windows.Window window)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(window);
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            windowInteropHelper.Owner = currentProcess.MainWindowHandle;
        }
    }
}