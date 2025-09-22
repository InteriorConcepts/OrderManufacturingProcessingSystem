using MyApp.DataAccess.Generated;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OMPS.Pages
{
    /// <summary>
    /// Interaction logic for EngOrder.xaml
    /// </summary>
    public partial class EngOrder : Page
    {
        public EngOrder(MainWindow parentWindow)
        {
            InitializeComponent();
            //
            this.DataContext = this;
            this.ParentWindow = parentWindow;
            this.dpnl_DataFilter.Visibility = Visibility.Collapsed;
            //this.FrmFin.ItemSource = Finishes_Default;
            this.JobNbrChanged += EngOrder_JobNbrChanged;
        }

        #region Events
        public event EventHandler<string> JobNbrChanged;
        #endregion

        #region Properties
        public string[] Finishes_Default { get; } = ["NA", "CH", "DB", "GY", "PL", "TP"];
        public string[] Finishes_BS { get; } = ["NA", "SL", "BK"];

        private string? _jobNbr;
        public string? JobNbr
        {
            get => this._jobNbr;
            set
            {
                if (value is null or "") return;
                if (EqualityComparer<string?>.Default.Equals(this._jobNbr, value.ToUpper())) return;
                this._jobNbr = value.ToUpper();
                this.JobNbrChanged.Invoke(this, this._jobNbr);
            }
        }
        private MainWindow ParentWindow { get; }
        public example_queries_GetColorSetResult ColorSetInfo { get; set; } = new();
        public ObservableCollection<example_queries_GetItemLinesByJobResult> MfgItemLines { get; set; } = [];
        #endregion


        #region Fields

        public int RowSpan = 1;
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
        #endregion


        #region Methods
        public void LoadDataForJob(string job)
        {
            this.MfgItemLines.Clear();
            var data_info = Ext.Queries.GetColorSet(job).First();
            PropertyCopier<example_queries_GetColorSetResult>.Copy(data_info, this.ColorSetInfo);

            var data_mfglines = Ext.Queries.GetItemLinesByJob(job);
            this.datagrid_main.BeginEdit();
            for (int i = 0; i < data_mfglines.Count; i++)
            {
                this.MfgItemLines.Add(data_mfglines[i]);
            }
            if (this.datagrid_main.Items.Count is not 0)
            {
                this.datagrid_main.ScrollIntoView(this.datagrid_main.Items[0]);
            }
            this.datagrid_main.EndInit();
        }

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

        private void DataGridMouseWheelHorizontal(object sender, RoutedEventArgs e)
        {
            MouseWheelEventArgs eargs = (MouseWheelEventArgs)e;
            double x = (double)eargs.Delta;
            double y = dataSideGridScrollViewer.VerticalOffset;
            dataSideGridScrollViewer.ScrollToVerticalOffset(y - x);
        }
        #endregion


        #region EventHandlers
        private void EngOrder_JobNbrChanged(object? sender, string e)
        {
            this.LoadDataForJob(e);
        }

        public void Txtbx_Job_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key is not System.Windows.Input.Key.Enter) return;
            if (sender as TextBox is not TextBox txtbx) return;
            if (txtbx.Text is not string job || job.Length is 0) return;
            if (!Ext.IsJobNumValid(job)) return;
            Ext.FormatJobNum(ref job);
            //LoadDataForJob(job);
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
        }

        public void DataGrid_IceManuf_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() is not string headerName)
            {
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

        private void dataSideGridScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            this.pnl_side.AddHandler(MouseWheelEvent, new RoutedEventHandler(DataGridMouseWheelHorizontal), true);
        }

        private void datagrid_main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Enter)
            {
                if (this.pnl_dock.Visibility is Visibility.Collapsed)
                    ToggleSideGrid();
                e.Handled = true;
                this.pnl_dock.Focus();
                this.pnl_side.Focus();
                return;
            }
            if (e.Key is Key.F && (Keyboard.Modifiers & ModifierKeys.Control) is ModifierKeys.Control)
            {
                this.dpnl_DataFilter.Visibility = Visibility.Visible;
                this.Txt_Filter.Focus();
                return;
            }
        }

        public void Btn_CollapseSideGrid_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleSideGrid();
        }

        public void Btn_CollapseTopBar_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleHeader();
        }

        private void Txt_Filter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is not Key.Enter) return;
            var viewSource = (CollectionViewSource)Resources["MfgItemsViewSource"];
            viewSource?.View?.Refresh();
        }

        private void MfgItemsViewSource_Filter(object sender, FilterEventArgs e)
        {
            // Assuming 'MyDataItem' is the type of objects in your collection

            if (e.Item is not example_queries_GetItemLinesByJobResult item)
            {
                return;
            }

            // Get text from TextBox
            string filterText = Txt_Filter.Text.ToLower();
            // If the filter text is empty, accept all items
            if (filterText is null || string.IsNullOrWhiteSpace(filterText))
            {
                e.Accepted = true;
                return;
            }

            var properties = typeof(example_queries_GetItemLinesByJobResult).GetProperties();

            string[] filterGroups = filterText.Split(' ');
            string[][] groupFilters = [.. filterGroups.Select(g => g.Split('+', StringSplitOptions.RemoveEmptyEntries))];

            int i = 0;
            foreach (var group in groupFilters)
            {
                List<string> groupReqd = [.. group];
                foreach (var filter in group)
                {
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(item)?.ToString();
                        //Debug.WriteLine(filter + " == " + value);
                        if (value is not null && value.ToLower().Contains(filter))
                        {
                            groupReqd.Remove(filter);
                            if (groupReqd.Count is 0)
                            {
                                e.Accepted = true;
                                return;
                            }
                        }
                    }
                    i++;
                }
            }

            // No match found across columns
            e.Accepted = false;
        }

        private void Btn_FilterClose_Click(object sender, RoutedEventArgs e)
        {
            this.datagrid_main.Focus();
            this.dpnl_DataFilter.Visibility = Visibility.Collapsed;
        }
        #endregion

        private void Page_EngOrder_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
