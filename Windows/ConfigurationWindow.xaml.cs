using OMPS.ViewModels;
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

namespace OMPS.Windows
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ObservableCollection<TimeZoneInfo> TimeZoneInfos = [];
        public ObservableCollection<DataGridGridLinesVisibility> DataGridGridLinesVisibilities = [];
        public ConfigurationWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.TimeZoneInfos.Clear();
            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                this.TimeZoneInfos.Add(tz); //.BaseUtcOffset.ToString() + "  " + (tz.IsDaylightSavingTime(TimeZoneInfo.ConvertTime(DateTime.Now, tz)) ? tz.DaylightName : tz.StandardName));
                var desc = (tz.IsDaylightSavingTime(TimeZoneInfo.ConvertTime(DateTime.Now, tz)) ? tz.DaylightName : tz.StandardName);
                this.Cmbx_Tiemzones.Items.Add($"{tz.DisplayName} {desc}");
            }
            this.Cmbx_Tiemzones.SelectedIndex = TimeZoneInfos.IndexOf(TimeZoneInfo.Local);
            foreach (var ev in Enum.GetValues<DataGridGridLinesVisibility>().Cast<DataGridGridLinesVisibility>())
            {
                this.DataGridGridLinesVisibilities.Add(ev);
                this.Cmbx_GridLines.Items.Add(ev);
            }
            this.Cmbx_GridLines.SelectedIndex = DataGridGridLinesVisibilities.IndexOf(MainViewModel.DataGridGridLinesVisibility);
            //TimeZoneInfo.GetSystemTimeZones().Select(z => (offset: z.BaseUtcOffset, display: z.DisplayName, name: (z.IsDaylightSavingTime(TimeZoneInfo.ConvertTime(DateTime.Now, z)) ? z.DaylightName : z.StandardName)));

        }

        public static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }
        public static MainWindow ParentWindow { get => Ext.MainWindow; }
        public static double DataGridFontSize { get => MainViewModel.FontSize_Base; }

        private void Btn_FontSizeDown_Click(object sender, RoutedEventArgs e)
        {
            Ext.MainViewModel.FontSize_Base -= 1;
        }

        private void Btn_FontSizeUp_Click(object sender, RoutedEventArgs e)
        {
            Ext.MainViewModel.FontSize_Base += 1;
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
            Ext.MainViewModel.CurrentTimezone = this.TimeZoneInfos[indx];
        }

        private void Cmbx_GridLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Cmbx_GridLines.SelectedIndex is int indx && indx is -1) return;
            if (indx > DataGridGridLinesVisibilities.Count - 1) return;
            Ext.MainViewModel.DataGridGridLinesVisibility = this.DataGridGridLinesVisibilities[indx];
        }
    }
}
