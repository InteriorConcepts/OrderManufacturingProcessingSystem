using MyApp.DataAccess.Generated;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using Windows.AI.MachineLearning;

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
            this.JobNbrChanged += this.EngOrder_JobNbrChanged;
        }

        #region Events
        public event EventHandler<string> JobNbrChanged;
        #endregion

        #region Properties
        public Dictionary<string, string[]> ItemLineFilers { get; set; } = [];
        public string[] Finishes_Default { get; } = ["NA", "CH", "DB", "GY", "PL", "TP"];
        public string[] Finishes_BS { get; } = ["NA", "SL", "BK"];

        private string? _jobNbr;
        public string? JobNbr
        {
            get => this._jobNbr;
            set
            {
                if (value is null or "") return;
                Ext.FormatJobNum(ref value);
                if (!Ext.IsJobNumValid(value)) return;
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
            "ProductLinkID", "ItemID", "CreatedByID", "ChangedbyID", "ChangedbyIDOffline"];
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

        public bool MfgItems_Filter(example_queries_GetItemLinesByJobResult item, string filterText)
        {

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
                                //e.Accepted = true;
                                return true;
                            }
                        }
                    }
                    i++;
                }
            }

            // No match found across columns
            //e.Accepted = false;
            return false;
        }

        private void UpdateProperty(object dataItem, string propertyName, string value)
        {
            var property = dataItem.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                try
                {
                    var convertedValue = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(dataItem, convertedValue);
                }
                catch (Exception ex)
                {
                    // Handle conversion errors
                    System.Diagnostics.Debug.WriteLine($"Error updating property: {ex.Message}");
                }
            }
        }
        #endregion


        #region EventHandlers
        private void EngOrder_JobNbrChanged(object? sender, string e)
        {
            this.LoadDataForJob(e);
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
            if (this.DataGrid_IceManuf_ColumnsExcludedHidden.Contains(headerName)) {
                e.Cancel = true;
            }
            e.Column.Visibility =
                this.DataGrid_IceManuf_ColumnsExcludedHidden.Contains(headerName) ?
                Visibility.Collapsed :
                Visibility.Visible;
            e.Column.IsReadOnly = this.DataGrid_IceManuf_ColumnsReadonly.Contains(headerName);
        }

        private void dataSideGridScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            //this.datagrid_side.AddHandler(MouseWheelEvent, new RoutedEventHandler(DataGridMouseWheelHorizontal), true);
        }

        private void datagrid_main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Enter)
            {
                if (this.pnl_dock.Visibility is Visibility.Collapsed)
                    ToggleSideGrid();
                e.Handled = true;
                var colIdx = this.datagrid_main.CurrentCell.Column.DisplayIndex;
                this.pnl_dock.Focus();
                this.grid_dataeditregion.Focus();
                if (this.grid_dataeditregion.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
                Debug.WriteLine(colIdx);
                if (txts.ElementAt(colIdx) is not TextBox txt) return;
                txt.Focus();
                txt.Select(txt.Text.Length, 0);
                return;
            }
            if (e.Key is Key.F && (Keyboard.Modifiers & ModifierKeys.Control) is ModifierKeys.Control)
            {
                this.dpnl_DataFilter.Visibility = Visibility.Visible;
                this.Txt_Filter.Focus();
                return;
            }
        }

        private void Page_EngOrder_PreviewKeyDown(object sender, KeyEventArgs e)
        {
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
            // Assuming 'MyDataItem' is the type of objects in collection
            if (e.Item is not example_queries_GetItemLinesByJobResult item)
            {
                return;
            }

            // Get text from TextBox
            var filterText = Txt_Filter.Text.ToLower();
            // If the filter text is empty, accept all items
            if (filterText is null || string.IsNullOrWhiteSpace(filterText))
            {
                //e.Accepted = true;
                e.Accepted = true;
                return;
            }
            e.Accepted = this.MfgItems_Filter(item, filterText);            
        }

        private void Btn_FilterClose_Click(object sender, RoutedEventArgs e)
        {
            this.datagrid_main.Focus();
            this.dpnl_DataFilter.Visibility = Visibility.Collapsed;
        }

        private void datagrid_main_Loaded(object sender, RoutedEventArgs e)
        {
            this.grid_dataeditregion.RowDefinitions.Clear();
            if (typeof(example_queries_GetItemLinesByJobResult).GetProperties() is not PropertyInfo[] props) return;
            if (props.Length is 0) return;
            List<string> cols = [.. this.datagrid_main.Columns.OrderBy(c => c.DisplayIndex).Select(c => c.Header.ToString())];
            props = [.. props.OrderBy(p => cols.IndexOf(p.Name))];
            var rowIdx = 0;
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i] is not PropertyInfo prop) return;
                if (this.DataGrid_IceManuf_ColumnsExcludedHidden.Contains(prop.Name)) continue;
                var lbl = new Label() {
                    Width = 100,
                    Content = prop.Name,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                var txt = new TextBox() {
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    IsReadOnly = this.DataGrid_IceManuf_ColumnsReadonly.Contains(prop.Name),
                };
                Binding bind = new(prop.Name)
                {
                    Path = new PropertyPath($"SelectedItem.{prop.Name}"),
                    Source = this.datagrid_main,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay
                };
                txt.SetBinding(TextBox.TextProperty, bind);
                this.grid_dataeditregion.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32, GridUnitType.Pixel) });
                this.grid_dataeditregion.Children.Add(lbl);
                this.grid_dataeditregion.Children.Add(txt);
                Grid.SetRow(lbl, rowIdx);
                Grid.SetRow(txt, rowIdx);
                Grid.SetColumn(lbl, 0);
                Grid.SetColumn(txt, 1);
                rowIdx++;
            }
        }
        private void Btn_SaveHeader_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void Btn_AcceptItemLineEdits_Click(object sender, RoutedEventArgs e)
        {
            if (this.grid_dataeditregion.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
            foreach (var item in txts)
            {
                var binding = item.GetBindingExpression(TextBox.TextProperty);
                var propertyName = binding.ParentBinding.Path.Path.Split('.').Last();
                datagrid_main.BeginEdit();
                UpdateProperty(datagrid_main.SelectedItem, propertyName, item.Text);
                // Trigger Cell & Row Edit events
                /*
                item.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode = BindingMode.OneWayToSource;
                item.Text = item.Text;
                item.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode = BindingMode.OneWay;
                */
            }
        }

        private void Btn_RevertItemLineEdits_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_DeleteItemLine_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Cell changes are committed automatically with ObservableCollection
                // track changes here if needed
                //TrackChange(e.Row.Item);
                Debug.WriteLine((e.Row.Item as example_queries_GetItemLinesByJobResult).ItemNbr);
            }
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Row changes committed
            }
        }

        private void Btn_AcceptChanges_Click(object sender, RoutedEventArgs e)
        {
            // Force commit any pending edits
            datagrid_main.CommitEdit(DataGridEditingUnit.Row, true);
            datagrid_main.CommitEdit(DataGridEditingUnit.Cell, true);
        }

        private void Btn_RejectChanges_Click(object sender, RoutedEventArgs e)
        {
            // Cancel pending edits
            datagrid_main.CancelEdit(DataGridEditingUnit.Row);
            datagrid_main.CancelEdit(DataGridEditingUnit.Cell);
        }
        #endregion
    }
}
