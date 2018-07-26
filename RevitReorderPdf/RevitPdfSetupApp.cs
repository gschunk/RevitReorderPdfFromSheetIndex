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
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
#endregion

namespace RevitReorderPdf
{
    class RevitPdfSetupApp : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            AddPdfPanel(a);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        private void AddPdfPanel(UIControlledApplication uiControlledApplication)
        {
            try
            {
                var dllName = GetAssemblyName();
                var panelName = "PDF";
                var buttonName = string.Format("{0}Button", nameof(ReorderExistingPdfCommand));
                var buttonText = "Reorder Existing PDF";
                var buttonHint = "Reorder Existing PDF File Based on Sheet Index";
                var imageName = "ReorderPdf_32x32.png";
                var className = typeof(ReorderExistingPdfCommand).FullName;

                var panel = uiControlledApplication.CreateRibbonPanel(panelName);
                
                var assemblyPath = GetAssemblyPath();

                PushButtonData pushButtonData = new PushButtonData(buttonName, buttonText, assemblyPath, className);
                PushButton pushButton = panel.AddItem(pushButtonData) as PushButton;
                pushButton.ToolTip = buttonHint;

                var image = NewBitmapImage(imageName);
                if (image != null) pushButton.LargeImage = image;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
        }

        private string GetAssemblyName()
        {
            return Path.GetFileName(GetAssemblyPath());
        }

        private string GetAssemblyPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            return Uri.UnescapeDataString(uri.Path);
        }

        private BitmapImage NewBitmapImage(string imageName)
        {
            using (Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageName))
            {
                if (imageStream != null)
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = imageStream;
                    image.EndInit();
                    return image;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
