using OMPS.viewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public ObservableCollection<TimeZoneInfo> TimeZoneInfos = [];
        public ConfigurationWindow(Main_ViewModel mainVM)
        {
            this.MainVM = mainVM;
            InitializeComponent();
            this.DataContext = this;
            this.MainVM.PropertyChanged += this.MainVM_PropertyChanged;
            this.TimeZoneInfos.Clear();
            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                this.TimeZoneInfos.Add(tz); //.BaseUtcOffset.ToString() + "  " + (tz.IsDaylightSavingTime(TimeZoneInfo.ConvertTime(DateTime.Now, tz)) ? tz.DaylightName : tz.StandardName));
                var desc = (tz.IsDaylightSavingTime(TimeZoneInfo.ConvertTime(DateTime.Now, tz)) ? tz.DaylightName : tz.StandardName);
                this.Cmbx_Tiemzones.Items.Add($"{tz.DisplayName} {desc}");
            }
            this.Cmbx_Tiemzones.SelectedIndex = TimeZoneInfos.IndexOf(TimeZoneInfo.Local);
            //TimeZoneInfo.GetSystemTimeZones().Select(z => (offset: z.BaseUtcOffset, display: z.DisplayName, name: (z.IsDaylightSavingTime(TimeZoneInfo.ConvertTime(DateTime.Now, z)) ? z.DaylightName : z.StandardName)));

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

        private void Cmbx_Tiemzones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Cmbx_Tiemzones.SelectedIndex is int indx && indx is -1) return;
            if (indx > TimeZoneInfos.Count - 1) return;
            this.MainVM.CurrentTimezone = this.TimeZoneInfos[indx];
        }
    }
}
