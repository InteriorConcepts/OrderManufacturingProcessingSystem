using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MyApp.DataAccess.Generated;
using OMPS.Components;
using OMPS.Pages;
using OMPS.viewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml.Linq;
using static OMPS.Ext;
using SCH = SQL_And_Config_Handler;

namespace OMPS.Windows
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Dictionary<string, string> Tabs = [];

        //public example_queries_GetColorSetResult ColorSetInfo { get; set; } = new();
        //public ObservableCollection<example_queries_GetItemLinesByJobResult> MfgItemLines { get; set; } = [];
        public MainWindow()
        {
            Ext.MainWindow = this;
            InitializeComponent();
            this.DataContext = Ext.MainViewModel;
            Ext.MainViewModel.Init();

            Ext.SetWindowTitle("");

            var timerid = SystemEvents.CreateTimer(1000);
            SystemEvents.TimerElapsed += (async (object sender, TimerElapsedEventArgs e) =>
            {
                if (e.TimerId != timerid) return;
                await Task.Delay(Math.Abs(DateTime.Now.Millisecond - 1000));
                this.Lbl_Time.Content = Ext.MainViewModel.CurrentDatetime.ToLongTimeString();
                this.Lbl_Date.Content = Ext.MainViewModel.CurrentDatetime.ToLongDateString();
            });

            this.Loaded += MainWindow_Loaded;
        }

        #region Dll Imports
        // Import SendMessage and FindWindow from user32.dll
        [LibraryImport("user32.dll", SetLastError = true)]
        internal static partial IntPtr SendMessage(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.SysInt)]
        internal static partial IntPtr FindWindow(
            [MarshalAs(UnmanagedType.LPWStr)] string? lpClassName,
            [MarshalAs(UnmanagedType.LPWStr)] string? lpWindowName);
        #endregion

        #region Fields
        public string Title_Main_Prefix = "Production Bridge";

        internal static readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsExcludedHidden =
            ["IceManufID", "ColorSetID", "ProductID",
            "ProductLinkID", "ItemID"];
        internal static readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsReadonly =
            ["QuoteNbr", "JobNbr", "CustOrderNbr",
            "Usertag1", "Multiplier", "Area", "CreatedByID",
            "BpartnerAvailable", "CustomerAvailable", "CreationDate",
            "ChangeDate", "ChangedbyID", "ChangebyIDOffline"];
        internal static readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsOrder = [
            "PartNbr"
            ];

        // Constants for minimizing
        const uint WM_SYSCOMMAND = 0x0112;
        const int SC_MINIMIZE = 0xF020;
        const int HTMINBUTTON = 0x8;
        const int SC_MAXIMIZE = 0xF030;
        const int HTMAXBUTTON = 0x9;
        const int SC_RESTORE = 0xF120;
        const int HTCLOSE = 0x14;
        #endregion

        #region Properties
        public bool IsLoading
        {
            get;
            set
            {
                field = value;
                if (field)
                {
                    this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
                }
                else
                {
                    this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                }
            }
        }

        public System.Windows.Shell.TaskbarItemProgressState LoadingState
        {
            get;
            set
            {
                field = value;
                this.TaskbarItemInfo.ProgressState = value;
            }
        }

        #endregion

        #region Event Handlers
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ver.Content = $"v{Ext.ApplicationVersionStr}";
            (this.ver.ToolTip as ToolTip)?.Content = $"Version: {Ext.ApplicationVersionStr}\nBuild date: {Ext.ApplicationBuildDateStr}";
            Ext.MainViewModel.CurrentPage = PageTypes.Home;
            //if (MainViewModel.AddNewPage(PageTypes.OrderSearch) is not string tag) return;
            //(MainViewModel[tag] as OrderSearch)?.LoadRecentOrders();
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {

        }

        private void MyData_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine(e.Action);
        }

        public void DataGrid_IceManuf_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() is not string headerName)
            {
                return;
            }
            if (DataGrid_IceManuf_ColumnsOrder.Contains(headerName))
            {
                e.Column.DisplayIndex = DataGrid_IceManuf_ColumnsOrder.IndexOf(headerName);
            }
            e.Column.Visibility =
                DataGrid_IceManuf_ColumnsExcludedHidden.Contains(headerName) ?
                Visibility.Collapsed :
                Visibility.Visible;
            e.Column.IsReadOnly = DataGrid_IceManuf_ColumnsReadonly.Contains(headerName);
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
        }

        private void TabControler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SetWindowTitle(Current().tab?.Header.ToString() ?? "");
        }

        public void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton rdiobtn) return;
            if (rdiobtn.Tag is not PageTypes ptype) return;
            Ext.MainViewModel.CurrentPage = ptype;
        }

        private void grid_TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton is not MouseButton.Left) return;
            Debug.WriteLine("Drag");
            try
            {
                DragMove();
            }
            catch (InvalidOperationException) { }
        }


        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    this.Grid_Main.Margin = new(7, 7, 7, 7);
                    break;
                case WindowState.Minimized:

                    break;
                case WindowState.Normal:
                    this.Grid_Main.Margin = new(0);
                    break;
            }
        }

        private void Btn_WinMin_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void Btn_WinMax_Click(object sender, RoutedEventArgs e)
        {
            //Ext.MainViewModel.ToggleCurrentContentControl(Visibility.Collapsed);
            this.BeginInit();
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                //SystemCommands.RestoreWindow(this);
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                //SystemCommands.MaximizeWindow(this);
            }
            this.EndInit();
            //Ext.MainViewModel.ToggleCurrentContentControl(Visibility.Visible);
        }

        private void Btn_WinClose_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Btn_Home_Click(object sender, RoutedEventArgs e)
        {
            Ext.MainViewModel.CurrentPage = PageTypes.Home;
        }

        private void Btn_Back_Click(object sender, RoutedEventArgs e)
        {
            if (Ext.MainViewModel.CurrentPage is PageTypes.None) return;
            this.Spnl_FrameTabs.Children.OfType<RadioButton>()
                .LastOrDefault(r =>
                    (PageTypes)r.Tag == Ext.MainViewModel.PreviousPage
                )?
                .ClearValue(RadioButton.FontStyleProperty);
            Ext.MainViewModel.CurrentPage = Ext.MainViewModel.PreviousPage;
        }

        private void Btn_Back_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Ext.MainViewModel.PreviousPage is PageTypes.None) return;
            var radios = this.Spnl_FrameTabs.Children.OfType<RadioButton>();
            var curRadio = radios.LastOrDefault(r => r.IsChecked is true);
            var prevRadio = radios.LastOrDefault(r => (PageTypes)r.Tag == Ext.MainViewModel.PreviousPage);
            if (prevRadio is null) return;
            prevRadio.FontStyle = FontStyles.Italic;
        }

        private void Btn_Back_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Ext.MainViewModel.PreviousPage is PageTypes.None) return;
            var radios = this.Spnl_FrameTabs.Children.OfType<RadioButton>();
            var curRadio = radios.LastOrDefault(r => r.IsChecked is true);
            var prevRadio = radios.LastOrDefault(r => (PageTypes)r.Tag == Ext.MainViewModel.PreviousPage);
            if (prevRadio is null) return;
            prevRadio.ClearValue(RadioButton.FontStyleProperty);
        }

        private void Btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow _configWin = new() { };
            //this.Spnl_ContentFrames.Opacity = 0.55;
            _configWin.Owner = this;
            _configWin.ShowDialog();
            //this.Spnl_ContentFrames.Opacity = 1.0;
        }

        private void Btn_Chat_Click(object sender, RoutedEventArgs e)
        {
            Chat chatWin = new()
            {
                Owner = this
            };
            chatWin.ShowDialog();
        }

        public void TabBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not RadioButton rdiobtn) return;
            if (rdiobtn.Content is not StackPanel stkpnl) return;
            if (stkpnl.Children.OfType<UIElement>().FirstOrDefault(e => e is Button) is not Button btn) return;
            btn.Visibility = Visibility.Hidden;
        }

        public void TabBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not RadioButton rdiobtn) return;
            if (rdiobtn.Content is not StackPanel stkpnl) return;
            if (stkpnl.Children.OfType<UIElement>().FirstOrDefault(e => e is Button) is not Button btn) return;
            btn.Visibility = Visibility.Visible;
        }

        public void Tab_CloseBtn(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Parent is not StackPanel stkpnl) return;
            if (stkpnl.Parent is not RadioButton rdiobtn) return;
            if (rdiobtn.Tag is not PageTypes pageType) return;
            MessageBox.Show(pageType.ToString());
        }

        private void Btn_ToggleSideNav_Click(object sender, RoutedEventArgs e)
        {
            this.Dpnl_SizeNav.Visibility =
                this.Dpnl_SizeNav.Visibility is Visibility.Collapsed ?
                Visibility.Visible : Visibility.Collapsed;
        }

        private void Btn_SideNav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not PageTypes pageType) return;
            this.Dpnl_SizeNav.Visibility = Visibility.Collapsed;
            Ext.MainViewModel.CurrentPage = pageType;
        }

        private void Btn_ToastTest_Click(object sender, RoutedEventArgs e)
        {
            var vals = Enum.GetValues<FeedbackToast.IconTypes>();
            var rand = new Random((int)DateTime.Now.Ticks);
            this.MainToastContainer.CreateToast("Test123", Environment.StackTrace, vals[rand.Next(0, vals.Length)]).Show();
        }
        #endregion
    }
}