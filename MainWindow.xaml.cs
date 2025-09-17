using MyApp.DataAccess.Generated;
using OMPS.Components;
using System.Buffers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using SCH = SQL_And_Config_Handler;

namespace OMPS
{

    public class StyleConfig
    {
        public static Color Dark_Button_BG { get; } = Color.FromRgb(20, 20, 20);
        public static Color Dark_Button_FG { get; } = Color.FromRgb(230, 230, 230);
        public Color Light_Button_BG { get; } = Color.FromRgb(230, 230, 230);
        public Color Light_Button_FG { get; } = Color.FromRgb(20, 20, 20);
    }

    public static class Styles
    {

        public static readonly DependencyProperty StylesProperty =
            DependencyProperty.Register("Styles", typeof(StyleConfig), typeof(LabelInputPair),
                new PropertyMetadata(new StyleConfig()));

        public static StyleConfig GetStyles(DependencyObject obj) => (StyleConfig)obj.GetValue(StylesProperty);
        public static void SetStyles(DependencyObject obj, StyleConfig value) => obj.SetValue(StylesProperty, value);
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
        public static readonly DependencyProperty StylesProperty =
            DependencyProperty.Register("Styles", typeof(StyleConfig), typeof(LabelInputPair),
                new PropertyMetadata(new StyleConfig()));

        public StyleConfig Styles
        {
            get { return (StyleConfig)GetValue(StylesProperty); }
            set { SetValue(StylesProperty, value); }
        }
        */

        public static readonly DependencyProperty Button_BGProperty =
            DependencyProperty.Register("Button_BG", typeof(ResourceKey), typeof(MainWindow),
                new PropertyMetadata(null));
        public ResourceKey Button_BG
        {
            get { return (ResourceKey)GetValue(Button_BGProperty); }
            set { SetValue(Button_BGProperty, value); }
        }

        public static readonly DependencyProperty Button_FGProperty =
            DependencyProperty.Register("Button_FG", typeof(ResourceKey), typeof(MainWindow),
                new PropertyMetadata(null));
        public ResourceKey Button_FG
        {
            get { return (ResourceKey)GetValue(Button_FGProperty); }
            set { SetValue(Button_FGProperty, value); }
        }

        public example_queriesQueries Queries = new();
        public ObservableCollection<example_queries_GetItemLinesByJobResult> MyData { get; set; } = [];
        //public DataTable MyDataTable = new();
        public MainWindow()
        {
            InitializeComponent();
            //
            var res = SCH.SQLDatabaseConnection.Init();
            if (res is not (bool, Exception) ||
                SCH.Global.Config.IsConfigCompletelyLoaded is false ||
                (res.Item1 is false || res.Item2 is not null))
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

            this.datagrid_main.ItemsSource = MyData;
            this.MyData.CollectionChanged += this.MyData_CollectionChanged; ;
            this.DataContext = this;
        }

        public void LoadDataForJob(string job)
        {
            MyData.Clear();
            var data = Queries.GetItemLinesByJob(job);
            this.datagrid_main.BeginEdit();
            for (int i = 0; i < data.Count; i++)
            {
                MyData.Add(data[i]);
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