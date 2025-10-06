using OMPS.viewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OMPS
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public required Main_ViewModel MainVM {  get; set; }
        public ConfigurationWindow(Main_ViewModel mainVM)
        {
            this.MainVM = mainVM;
            InitializeComponent();
            this.DataContext = this;
            this.MainVM.PropertyChanged += MainVM_PropertyChanged;
        }

        private void MainVM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not "FontSize_DataGrid") return;
            this.Txtblk_Title.FontSize = this.MainVM.FontSize_DataGrid * 1.25;
        }

        private void Btn_FontSizeDown_Click(object sender, RoutedEventArgs e)
        {
            this.MainVM.FontSize_DataGrid -= 1;
        }

        private void Btn_FontSizeUp_Click(object sender, RoutedEventArgs e)
        {
            this.MainVM.FontSize_DataGrid += 1;
        }

        private void Btn_CloseConfigWin_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void ConfigWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is not Key.Escape)
            {
                e.Handled = false;
                return;
            }
            this.Btn_CloseConfigWin_Click(this.Btn_CloseConfigWin, e);
        }
    }
}
