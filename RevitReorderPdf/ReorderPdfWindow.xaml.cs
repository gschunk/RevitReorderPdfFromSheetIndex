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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RevitReorderPdf
{
    /// <summary>
    /// Interaction logic for ReorderPdfWindow.xaml
    /// </summary>
    public partial class ReorderPdfWindow : Window, IDisposable
    {
        private ReorderExistingPdfViewModel ViewModel { get; }

        public ReorderOptions ReorderOptions { get; private set; }

        public ReorderPdfWindow(Document document)
        {
            InitializeComponent();

            ViewModel = new ReorderExistingPdfViewModel(document, new TaskDialogBoxErrorLogger());

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            DataContext = ViewModel;

            UpdateUi();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateUi();
        }

        private void UpdateUi()
        {
            ReorderButton.IsEnabled = ViewModel.CanReorder;
        }

        private void ReorderButton_Click(object sender, RoutedEventArgs e)
        {
            ReorderOptions = ViewModel.ReorderOptions;
            this.DialogResult = true;
            Close();
        }

        public void Dispose()
        {
            this.Close();
        }

        private void SelectPdfFileButton_Click(object sender, RoutedEventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "pdf files (*.pdf)|*.pdf";
                openFileDialog.RestoreDirectory = true;

                var result = openFileDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    ViewModel.SelectedPdfFile = openFileDialog.FileName;
                }
            }
        }
    }
}
