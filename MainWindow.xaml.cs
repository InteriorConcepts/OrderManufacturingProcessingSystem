using Microsoft.Win32;
using MyApp.DataAccess.Generated;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Windows.ApplicationModel.Activation;
using SCH = SQL_And_Config_Handler;

namespace OMPS
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Pages.OrderSearch Page_OrderSearch;
        //public Pages.EngOrder Page_EngOrder;

        public Dictionary<string, string> Tabs = [];

        //public example_queries_GetColorSetResult ColorSetInfo { get; set; } = new();
        //public ObservableCollection<example_queries_GetItemLinesByJobResult> MfgItemLines { get; set; } = [];
        public string Title_Main_Prefix = "Production Bridge";
        public MainWindow()
        {
            InitializeComponent();
            //
            this.DataContext = this;
            this.SetWindowTitle("");
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

            //this.Page_OrderSearch = new(this);
            //this.Page_EngOrder = new(this) { };
            //this.Page_OrderSearch.LoadRecentOrders();
            //this.MainFrame.Navigate(this.Page_OrderSearch);
            //this.Tab_Create_EngOrder("Job").page?.LoadDataForJob("J000035602");
            this.Tab_Create_OrderSearch("Order Search").page?.LoadRecentOrders();

        }

        public static readonly DependencyProperty TabControl_SelectedIndex_Property =
            DependencyProperty.Register(
                "TabControl_SelectedIndex_Property",
                typeof(int), typeof(MainWindow),
                new PropertyMetadata(1)
            );
        public int TabControl_SelectedIndex
        {
            get => (int)GetValue(TabControl_SelectedIndex_Property);
            set => SetValue(TabControl_SelectedIndex_Property, value);
        }

        public (TabItem? tab, Frame? frame, Page? page) Current()
        {
            var tab = this.TabControler.SelectedItem as TabItem;
            if (tab is null) return (tab, null, null);
            var frm = this.TabControler.SelectedContent as Frame;
            if (frm is null) return (tab, frm, null);
            var page = frm.Content as Page;
            return (tab, frm, page);
        }

        public (TabItem tab, Frame frame, Pages.OrderSearch? page) Tab_Create_OrderSearch(string name = "New Tab", bool setCurrent = true)
        {
            var (tab, frame, page) = this.Tab_Create(name, Page_Create(PageTypes.OrderSearch), setCurrent);
            ((Pages.OrderSearch?)page)?.ParentTab = tab;
            return (tab, frame, (Pages.OrderSearch?)page);
        }

        public (TabItem tab, Frame frame, Pages.EngOrder? page) Tab_Create_EngOrder(string name = "New Tab", bool setCurrent = true)
        {
            var (tab, frame, page) = this.Tab_Create(name, Page_Create(PageTypes.EngOrder), setCurrent);
            ((Pages.EngOrder?)page)?.ParentTab = tab;
            return (tab, frame, (Pages.EngOrder?)page);
        }

        public (TabItem tab, Frame frame, Page? page) Tab_Create(string name = "New Tab", Page? pageToLoad = null, bool setCurrent = true)
        {
            string nameSuff = ("_" + DateTime.Now.Ticks),
                tabName = "TabItem" + nameSuff,
                frmName = "Frame" + nameSuff;
            var tab = new TabItem()
            {
                Name = tabName,
                Header = name,
                Padding = new Thickness(10, 2, 2, 10),
                VerticalContentAlignment = VerticalAlignment.Top,
            };
            var frm = new Frame() {
                Name = frmName,
                NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden
            };
            tab.Content = frm;
            this.TabControler.Items.Add(tab);
            this.Tabs.Add(tabName, frmName);
            if (setCurrent)
            {
                //MessageBox.Show("***");
                var idx = this.TabControler.Items.IndexOf(tab);
                Debug.WriteLine(idx);
                this.Tab_Set(idx);
            }
            if (pageToLoad is not Page page) return (tab, frm, null);
            frm.Navigate(pageToLoad);
            return (tab, frm, page);
        }

        public void Tab_Set(int item)
        {
            this.TabControl_SelectedIndex = item;
        }

        public enum PageTypes
        {
            OrderSearch = 1,
            EngOrder = 2
        }

        public Page? Page_Create(PageTypes pageType)
        {
            return pageType switch
            {
                PageTypes.OrderSearch => new Pages.OrderSearch(this),
                PageTypes.EngOrder => new Pages.EngOrder(this),
                _ => null,
            };
        }

        /*
        public void NavigateToPage(PageTypes pageType, bool runOnload, params object[] onloadParams)
        {
            Page? page = this.Page_Create(pageType);
            if (page is null || page.Content is null) return;
            this.Current().frame?.Navigate(page);
            if (!runOnload) return;
            if (page is Pages.OrderSearch search)
            {
                search.LoadRecentOrders();
            }
            else if (page is Pages.EngOrder order)
            {
                if (onloadParams.Length is 1 && onloadParams[0] is string job)
                    order.LoadDataForJob(job);
            }
            //if (this.MainFrame.Content == page.Content) return;
            //this.MainFrame.Navigate(page);
        }
        */

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
            this.Current().tab?.Header = title;
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
            SetWindowTitle(Current().tab?.Header.ToString() ?? "");
        }
    }
}