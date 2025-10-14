using Microsoft.Win32;
using MyApp.DataAccess.Generated;
using OMPS.Pages;
using OMPS.viewModel;
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
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Radios;
using Windows.UI.Text;
using static OMPS.Ext;
using SCH = SQL_And_Config_Handler;

namespace OMPS.Windows
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //public ViewController ViewController { get; set; }

        public required Main_ViewModel _viewModel;

        public Dictionary<string, string> Tabs = [];

        //public example_queries_GetColorSetResult ColorSetInfo { get; set; } = new();
        //public ObservableCollection<example_queries_GetItemLinesByJobResult> MfgItemLines { get; set; } = [];
        public string Title_Main_Prefix = "Production Bridge";
        public MainWindow()
        {
            InitializeComponent();
            Ext.MainWindow = this;
            this.DataContext = this._viewModel = new Main_ViewModel();
            this.MainViewModel.ParentWin = this;
            //
            this.SetWindowTitle("");
            //this.ViewController = new(this);
            var res = SCH.SQLDatabaseConnection.Init();
            if (res.Item1 is false || res.Item2 is not null)
            {
                string msg = "Could not load Config:\n";
                if (res is (bool, Exception) && res.Item2 is not null)
                {
                    msg += res.Item2.Message + "\n" + res.Item2.StackTrace;
                } else
                {
                    msg += SCH.Global.Config.GetErrorString();
                }
                MessageBox.Show(msg);
                App.Current.Shutdown(-1);
            }
            var timerid = SystemEvents.CreateTimer(1000);
            SystemEvents.TimerElapsed += (async (object sender, TimerElapsedEventArgs e) =>
            {
                if (e.TimerId != timerid) return;
                await Task.Delay(Math.Abs(DateTime.Now.Millisecond - 1000));
                this.Lbl_Time.Content = this.MainViewModel.CurrentDatetime.ToLongTimeString();
                this.Lbl_Date.Content = this.MainViewModel.CurrentDatetime.ToLongDateString();
            });

            this.Loaded += MainWindow_Loaded;



            //((OrderSearch?)((Main_ViewModel)this.DataContext)["OrderSearch", PageTypes.OrderSearch])?.LoadRecentOrders();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //if (MainViewModel.AddNewPage(PageTypes.OrderSearch) is not string tag) return;
            //(MainViewModel[tag] as OrderSearch)?.LoadRecentOrders();
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {

        }

        /*
        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x84) // WM_NCHITTEST
            {
                // Extract mouse coordinates from lParam
                int x = (int)(lParam.ToInt64() & 0xFFFF);
                int y = (int)(lParam.ToInt64() >> 16);

                // Convert screen coordinates to client coordinates if needed
                // Point clientPoint = PointFromScreen(new Point(x, y));

                // Perform custom hit-testing logic based on coordinates
                // and return the appropriate HT* value (e.g., HTCAPTION, HTMINBUTTON, HTCLOSE)
                // Example: If the point is within a custom title bar area, return HTCAPTION (0x2)

                // Set handled to true if you've handled the message to prevent default processing
                handled = true;
                return new IntPtr(0x2); // Example: Returning HTCAPTION
            }

            return IntPtr.Zero; // Let other messages be handled by default
        }
        */

        public Main_ViewModel MainViewModel
        {
            get => ((Main_ViewModel)this.DataContext);
        }

        public bool IsLoading
        {
            get;
            set {
                field = value;
                if (field)
                {
                    this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
                } else
                {
                    this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                }
            }
        }

        public System.Windows.Shell.TaskbarItemProgressState LoadingState
        {
            get;
            set {
                field = value;
                this.TaskbarItemInfo.ProgressState = value;
            }
        }

        public void SetWindowTitle(string title)
        {
            if (string.IsNullOrEmpty(title.Trim()))
            {
                this.Title = $"{this.Title_Main_Prefix}";
            }
            else
            {
                this.Title = $"{this.Title_Main_Prefix} | {title}";
            }
        }

        public void SetUrlRelPath(string path)
        {
            this.Txt_Url.Text = $"pbridge://{path}";
        }

        public void SetTabTitle(string title)
        {
            /*
            this.TabBtn_LineItemInfo.Content =
                this.TabBtn_LineItemInfo.Content.ToString()?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[0] +
                Environment.NewLine +
                title;
            */
        }

        public static DataTable ConvertListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            // Get properties of the class
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            dataTable.BeginLoadData();
            // Create columns in the DataTable based on class properties
            foreach (PropertyInfo prop in properties)
            {
                // Handle Nullable types
                Type columnType = prop.PropertyType;
                if (columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = Nullable.GetUnderlyingType(columnType);
                }
                dataTable.Columns.Add(prop.Name, columnType);
            }

            // Populate the DataTable with data from the list
            foreach (T item in items)
            {
                DataRow row = dataTable.NewRow();
                foreach (PropertyInfo prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value; // Handle null values
                }
                dataTable.Rows.Add(row);
            }
            dataTable.EndLoadData();

            return dataTable;
        }

        private void MyData_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine(e.Action);
        }

        private readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsExcludedHidden =
            ["IceManufID", "ColorSetID", "ProductID",
            "ProductLinkID", "ItemID"];
        private readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsReadonly =
            ["QuoteNbr", "JobNbr", "CustOrderNbr",
            "Usertag1", "Multiplier", "Area", "CreatedByID",
            "BpartnerAvailable", "CustomerAvailable", "CreationDate",
            "ChangeDate", "ChangedbyID", "ChangebyIDOffline"];
        private readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsOrder = [
            "PartNbr"
            ];
        public void DataGrid_IceManuf_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() is not string headerName) {
                return;
            }
            if (this.DataGrid_IceManuf_ColumnsOrder.Contains(headerName))
            {
                e.Column.DisplayIndex = this.DataGrid_IceManuf_ColumnsOrder.IndexOf(headerName);
            }
            e.Column.Visibility =
                this.DataGrid_IceManuf_ColumnsExcludedHidden.Contains(headerName) ?
                Visibility.Collapsed :
                Visibility.Visible;
            e.Column.IsReadOnly = this.DataGrid_IceManuf_ColumnsReadonly.Contains(headerName);
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
            this.MainViewModel.CurrentPage = ptype;
        }

        private void grid_TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Drag");
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    //SendMessage(hWnd, WM_SYSCOMMAND, (IntPtr)SC_RESTORE, IntPtr.Zero);
                    DragMove();
                    //this.WindowState = WindowState.Normal;
                }
                catch (InvalidOperationException _) { }
            }
        }







        // Import SendMessage and FindWindow from user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Constants for minimizing
        private const uint WM_SYSCOMMAND = 0x0112;
        private const int SC_MINIMIZE = 0xF020;
        private const int HTMINBUTTON = 0x8;
        private const int SC_MAXIMIZE = 0xF030;
        private const int HTMAXBUTTON = 0x9;
        private const int SC_RESTORE = 0xF120;
        private const int HTCLOSE = 0x14;

        private void Btn_WinMin_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void Btn_WinMax_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.Grid_Main.Margin = new(0);
                SystemCommands.RestoreWindow(this);
            } else
            {
                this.Grid_Main.Margin = new(8, 8, 8, 4);
                SystemCommands.MaximizeWindow(this);
            }
        }

        private void Btn_WinClose_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Btn_Home_Click(object sender, RoutedEventArgs e)
        {
            this.MainViewModel.CurrentPage = PageTypes.Home;
        }

        private void Btn_Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.MainViewModel.CurrentPage is PageTypes.None) return;
            this.Spnl_FrameTabs.Children.OfType<RadioButton>()
                .LastOrDefault(r =>
                    (PageTypes)r.Tag == this.MainViewModel.PreviousPage
                )?
                .ClearValue(RadioButton.FontStyleProperty);
            this.MainViewModel.CurrentPage = this.MainViewModel.PreviousPage;
        }

        private void Btn_Back_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.MainViewModel.PreviousPage is PageTypes.None) return;
            var radios = this.Spnl_FrameTabs.Children.OfType<RadioButton>();
            var curRadio = radios.LastOrDefault(r => r.IsChecked is true);
            var prevRadio = radios.LastOrDefault(r => (PageTypes)r.Tag == this.MainViewModel.PreviousPage);
            if (prevRadio is null) return;
            prevRadio.FontStyle = FontStyles.Italic;
        }

        private void Btn_Back_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.MainViewModel.PreviousPage is PageTypes.None) return;
            var radios = this.Spnl_FrameTabs.Children.OfType<RadioButton>();
            var curRadio = radios.LastOrDefault(r => r.IsChecked is true);
            var prevRadio = radios.LastOrDefault(r => (PageTypes)r.Tag == this.MainViewModel.PreviousPage);
            if (prevRadio is null) return;
            prevRadio.ClearValue(RadioButton.FontStyleProperty);
        }

        public BlurEffect blur = new() { KernelType = KernelType.Gaussian, Radius = 2, RenderingBias = RenderingBias.Performance };
        private void Btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow _configWin = new (this._viewModel) { MainVM = this._viewModel };
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
            this.Spnl_SideNav.Visibility =
                this.Spnl_SideNav.Visibility is Visibility.Collapsed ?
                Visibility.Visible : Visibility.Collapsed;
        }

        private void Btn_SideNav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not PageTypes pageType) return;
            this.MainViewModel.CurrentPage = pageType;
        }
    }
}