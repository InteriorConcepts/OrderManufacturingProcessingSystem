using Microsoft.Win32;
using MyApp.DataAccess.Generated;
using OMPS.viewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Windows.ApplicationModel.Activation;
using SCH = SQL_And_Config_Handler;

namespace OMPS
{

    public enum PageTypes
    {
        OrderSearch = 1,
        EngOrder = 2
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Pages.OrderSearch Page_OrderSearch;
        //public Pages.EngOrder Page_EngOrder;

        //public ViewController ViewController { get; set; }

        public Dictionary<string, string> Tabs = [];

        //public example_queries_GetColorSetResult ColorSetInfo { get; set; } = new();
        //public ObservableCollection<example_queries_GetItemLinesByJobResult> MfgItemLines { get; set; } = [];
        public string Title_Main_Prefix = "Production Bridge";
        public MainWindow()
        {
            InitializeComponent();
            //
            this.DataContext = new Main_ViewModel() { ParentWin = this };
            this.SetWindowTitle("");
            ((Main_ViewModel)this.DataContext)?.ParentWin = this;
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
            SystemEvents.TimerElapsed += ((object sender, TimerElapsedEventArgs e) =>
            {
                if (e.TimerId != timerid) return;
                this.Lbl_Time.Content = DateTime.Now.ToLongTimeString();
                this.Lbl_Date.Content = DateTime.Now.ToLongDateString();
            });

            ((Main_ViewModel)this.DataContext)?.OrderSearch_VM.LoadRecentOrders();
        }

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

        public void SetTabTitle(string title)
        {
            //this.Current().tab?.Header = title;
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

        public void ChangeView(PageTypes pageType)
        {
            this.MainViewModel.Current = (pageType) switch
            {
                PageTypes.OrderSearch => this.MainViewModel.OrderSearch_VM,
                PageTypes.EngOrder => this.MainViewModel.EngOrder_VM,
                _ => null
            };
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton rdiobtn) return;
            if (rdiobtn.Tag is not PageTypes ptype) return;
            this.ChangeView(ptype);
        }

        private void grid_TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Drag");
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
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
        public const int SC_MAXIMIZE = 0xF030;
        private const int SC_RESTORE = 0xF120;

        private void Btn_WinMin_Click(object sender, RoutedEventArgs e)
        {
            IntPtr hWnd = FindWindow(null, this.Title);
            if (hWnd != IntPtr.Zero)
            {
                // Send the minimize message
                SendMessage(hWnd, WM_SYSCOMMAND, (IntPtr)SC_MINIMIZE, IntPtr.Zero);
            }
            //this.WindowState = WindowState.Minimized;
        }

        private void Btn_WinMax_Click(object sender, RoutedEventArgs e)
        {
            IntPtr hWnd = FindWindow(null, this.Title);
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            if (this.WindowState == WindowState.Maximized)
            {
                SendMessage(hWnd, WM_SYSCOMMAND, (IntPtr)SC_RESTORE, IntPtr.Zero);
            } else
            {

                SendMessage(hWnd, WM_SYSCOMMAND, (IntPtr)SC_MAXIMIZE, IntPtr.Zero);
            }
            
        }

        private void Btn_WinClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_Home_Click(object sender, RoutedEventArgs e)
        {
            this.ChangeView(PageTypes.OrderSearch);
        }
    }
}