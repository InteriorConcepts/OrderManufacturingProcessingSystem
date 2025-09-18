using MyApp.DataAccess.Generated;
using OMPS.Components;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SCH = SQL_And_Config_Handler;

namespace OMPS
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public example_queriesQueries Queries = new();


        public example_queries_GetColorSetResult ColorSetInfo { get; set; } = new();
        public ObservableCollection<example_queries_GetItemLinesByJobResult> MfgItemLines { get; set; } = [];

        public MainWindow()
        {
            InitializeComponent();
            //
            this.DataContext = this;
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


            this.datagrid_main.ItemsSource = MfgItemLines;

            //MfgItemLines.CollectionChanged += this.MyData_CollectionChanged; ;
        }

        public void LoadDataForJob(string job)
        {

            MfgItemLines.Clear();
            var data_info = Queries.GetColorSet(job).First();
            /*
            try
            {
                // Test 1: Direct property assignment (should work)
                ColorSetInfo.SupplyOrderRef = data_info.SupplyOrderRef;

                // Test 2: Reflection assignment (might not work)
                var property = typeof(example_queries_GetColorSetResult).GetProperty("SupplyOrderRef");
                property.SetValue(ColorSetInfo, data_info.SupplyOrderRef);

                // Test 3: Check if values are actually different
                if (!ColorSetInfo.SupplyOrderRef.Equals(data_info.SupplyOrderRef))
                {
                    property.SetValue(ColorSetInfo, data_info.SupplyOrderRef);
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            */
            PropertyCopier<example_queries_GetColorSetResult>.Copy(data_info, ColorSetInfo);

            var data_mfglines = Queries.GetItemLinesByJob(job);
            this.datagrid_main.BeginEdit();
            for (int i = 0; i < data_mfglines.Count; i++)
            {
                MfgItemLines.Add(data_mfglines[i]);
            }
            if (this.datagrid_main.Items.Count is not 0)
            {
                this.datagrid_main.ScrollIntoView(this.datagrid_main.Items[0]);
            }
            this.datagrid_main.EndInit();

            //MyDataTable = ConvertListToDataTable(data);
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


        public int RowSpan = 1;
        public void ToggleSideGrid()
        {
            this.pnl_dock.Visibility =
                this.pnl_dock.Visibility is Visibility.Collapsed ?
                Visibility.Visible :
                Visibility.Collapsed;
            this.RowSpan = (this.pnl_dock.Visibility is Visibility.Collapsed ? 2 : 1);
            Grid.SetColumnSpan(datagrid_main, RowSpan);
        }

        public void ToggleHeader()
        {
            this.grid_header.Visibility =
                this.grid_header.Visibility is Visibility.Collapsed ?
                Visibility.Visible :
                Visibility.Collapsed;
        }

        public static bool IsJobNumValid(string job)
        {
            if (job[0] is not 'J' && job[0] is not 'S') return false;
            if (!job[1..].All(c => int.TryParse(c.ToString(), out int _))) return false;
            return true;
        }

        public static void FormatJobNum(ref string job)
        {
            if (job.Length >= (job[0] is 'J' ? 6 : 5) && job.Length is not 10)
            {
                job = $"{job[0]}{job[(job.Length - 5)..(job.Length)].PadLeft(9, '0')}";
            }
        }

        private void Btn_CollapseSideGrid_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleSideGrid();
        }

        private void Btn_CollapseTopBar_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleHeader();
        }

        private void Txtbx_Job_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key is not System.Windows.Input.Key.Enter) return;
            if (sender as TextBox is not TextBox txtbx) return;
            if (txtbx.Text is not string job || job.Length is 0) return;
            if (!IsJobNumValid(job)) return;
            FormatJobNum(ref job);
            LoadDataForJob(job);
        }

        private void dataSideGridScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            this.pnl_side.AddHandler(MouseWheelEvent, new RoutedEventHandler(DataGridMouseWheelHorizontal), true);
        }

        private void DataGridMouseWheelHorizontal(object sender, RoutedEventArgs e)
        {
            MouseWheelEventArgs eargs = (MouseWheelEventArgs)e;
            double x = (double)eargs.Delta;
            double y = dataSideGridScrollViewer.VerticalOffset;
            dataSideGridScrollViewer.ScrollToVerticalOffset(y - x);
        }

        private void datagrid_main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is not Key.Enter) return;
            if (this.pnl_dock.Visibility is Visibility.Collapsed)
                ToggleSideGrid();
            e.Handled = true;
            this.pnl_dock.Focus();
            this.pnl_side.Focus();
            /*
            this.pnl_side.SelectAllCells();
            var cell = this.pnl_side.SelectedCells[0];
            this.pnl_side.UnselectAllCells();
            this.pnl_side.CurrentCell = cell;
            //MessageBox.Show(this.pnl_side.CurrentCell.Item.ToString());
            this.pnl_side.BeginEdit();
            //Debug.WriteLine(this.datagrid_main.CurrentCell.Column.DisplayIndex);
            */
            /*
            this.pnl_side.CurrentCell =
                new DataGridCellInfo(
                    this.pnl_side.Items[0],
                    this.pnl_side.Columns[0]
                );
            */

        }

    }
}