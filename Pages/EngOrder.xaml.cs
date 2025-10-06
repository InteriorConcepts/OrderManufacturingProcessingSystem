using Microsoft.VisualBasic;
using MyApp.DataAccess.Generated;
using OMPS.Components;
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
using SCH = SQL_And_Config_Handler;

namespace OMPS.Pages
{
    /// <summary>
    /// Interaction logic for EngOrder.xaml
    /// </summary>
    public partial class EngOrder : UserControl
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
        internal MainWindow ParentWindow { get; set; }
        internal DataGrid CurrentGrid { get; set; }
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
                this.Last_ManufData = null;
                this._jobNbr = value.ToUpper();
                this.JobNbrChanged.Invoke(this, this._jobNbr);
            }
        }
        public example_queries_GetColorSetResult ColorSetInfo { get; set; } = new();
        public ObservableCollection<example_queries_GetItemLinesByJobResult> MfgItemLines { get; set; } = [];
        #endregion


        #region Fields

        public int RowSpan = 1;
        private readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsExcludedHidden =
            ["IceManufID", "ColorSetID", "ProductID",
            "ProductLinkID", "ItemID", "QuoteNbr", "CustOrderNbr",
            "BpartnerAvailable", "CustomerAvailable", "CreatedByID",
            "ChangedbyID", "ChangebyIDOffline"];
        private readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsReadonly =
            ["QuoteNbr", "JobNbr", "CustOrderNbr",
            "Usertag1", "Multiplier", "Area", "CreationDate", "ChangeDate"];
        private readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsOrder = [
            "JobNbr", "PartNbr", "ItemNbr", "CatalogNbr", "Qty", "Multiplier",
            "Description", "UofM", "Type", "SubType", "IDNbr", "Explode",
            "Assembled", "AssyNbr", "TileIndicator", "ItemFin", "ColorBy",
            "WorkCtr"
            ];
        #endregion


        #region Methods
        public void LoadDataForJob(string job)
        {
            try
            {
                this.CurrentGrid?.Visibility = Visibility.Collapsed;
                this.LoadColorSetData(job);
                if (this.RadioBtn_View_QPO.IsChecked is true)
                {
                    this.CurrentGrid = this.datagrid_QPO;
                    this.LoadQPartsOrdered(job);
                }
                else if (this.RadioBtn_View_QDO.IsChecked is true)
                {
                    this.CurrentGrid = this.datagrid_QIO;
                    this.LoadQItemsOrdered(job);
                }
                else if (this.RadioBtn_View_M.IsChecked is true)
                {
                    this.CurrentGrid = this.datagrid_main;
                    this.LoadManufData(job);
                }
                else if (this.RadioBtn_View_MP.IsChecked is true)
                {
                    this.CurrentGrid = this.datagrid_MP;
                    this.LoadManufParts(job);
                }
                this.CurrentGrid?.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void LoadColorSetData(string job)
        {
            var t = new Task(() =>
            {
                var data_info = Ext.Queries.GetColorSet(job).First();
                PropertyCopier<example_queries_GetColorSetResult>.Copy(data_info, this.ColorSetInfo);
            });
            t.Start();
        }

        public void LoadQPartsOrdered(string job)
        {

        }

        public void LoadQItemsOrdered(string job)
        {

        }

        private DateTime? Last_ManufData = null;
        public void LoadManufData(string job)
        {
            if (this.Last_ManufData is not null && (DateTime.Now - this.Last_ManufData.Value).TotalSeconds is double sec && sec < 10)
            {
                Debug.WriteLine($"Last update was {sec} sec ago");
                return;
            }
            Debug.WriteLine("Loading Manuf Data");
            this.MfgItemLines.Clear();
            this.progbar_itemlines.Value = 50;
            this.progbar_itemlines.IsEnabled = true;
            this.progbar_itemlines.Visibility = Visibility.Visible;
            var t = new Task(() =>
            {
                var data_mfglines = Ext.Queries.GetItemLinesByJob(job);
                Application.Current.Dispatcher.Invoke(() =>
                {
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
                    this.ParentWindow.SetTabTitle($"{this.JobNbr}");
                    this.progbar_itemlines.Value = 0;
                    this.progbar_itemlines.IsEnabled = false;
                    this.progbar_itemlines.Visibility = Visibility.Collapsed;
                });
                this.Last_ManufData = DateTime.Now;
            });
            t.Start();
        }

        public void LoadManufParts(string job)
        {

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

        private (string, bool, Type?, object?) UpdateProperty(object dataItem, string propertyName, string value)
        {
            var property = dataItem.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                try
                {
                    var prevValue = property.GetValue(dataItem);
                    var convertedValue = Convert.ChangeType(value, property.PropertyType);
                    //Debug.WriteLine($"{prevValue} | {convertedValue}");
                    if (!((object?)prevValue as object)?.Equals((object?)convertedValue as object) ?? true)
                    {
                        property.SetValue(dataItem, convertedValue);
                        return (propertyName, true, property.PropertyType, convertedValue);
                    } else
                    {
                        return (propertyName, false, property.PropertyType, null);
                    }
                }
                catch (Exception ex)
                {
                    // Handle conversion errors
                    System.Diagnostics.Debug.WriteLine($"Error updating property: {ex.Message}");
                    return (propertyName, false, property.PropertyType, null);
                }
            }
            return (propertyName, false, null, null);
        }

        public bool UpdateItemLineData(Guid manufid, example_queries_GetItemLinesByJobResult item)
        {
            if (item is null) return false;
            var res = Ext.Queries.SetItemLineByJobAndManufID(
                item.PartNbr, item.ItemNbr, item.IDNbr, item.CatalogNbr, item.Qty, item.Type,
                item.SubType, item.Description, item.UofM, item.ItemFin, item.ItemCore, item.ColorBy,
                item.Dept, item.WorkCtr, item.ScrapFactor, item.SizeDivisor, item.Depth, item.Width, item.Fabwidth,
                item.Height, item.FabHeight, item.Assembled, item.AssyNbr, item.TileIndicator, item.Explode,
                item.Option01, item.Option02, item.Option03, item.Option04, item.Option05, item.Option06,
                item.Option07, item.Option08, item.Option09, item.Option10, item.Usertag1, item.CoreSize,
                item.Multiplier, item.Area, item.JobNbr, manufid
            );
            if (res is null) return false;
            return true;
        }

        public bool CopyItemLineData(Guid manufid, example_queries_GetItemLinesByJobResult item)
        {
            if (item is null) return false;
            var res = Ext.Queries.CopyManufItemLineByManufID(" (COPY)", manufid);
            if (res is null) return false;
            return true;
        }

        public bool DeleteItemLine(Guid manufid, string job)
        {
            var res = Ext.Queries.DeleteManufItemLineByManufID(job, manufid);
            if (res is null) return false;
            return true;
        }

        public void ToggleFiltersPanel()
        {
            Debug.WriteLine("Toggle Filters");
            if (this.dpnl_DataFilter.Visibility is Visibility.Collapsed)
            {
                this.dpnl_DataFilter.Visibility = Visibility.Visible;
                this.Txt_Filter.Focus();
            }
            else
            {
                this.dpnl_DataFilter.Visibility = Visibility.Collapsed;
                this.CurrentGrid.Focus();
            }
        }
        #endregion


        #region EventHandlers
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton) return;
            if (this.JobNbr is null) return;
            Debug.WriteLine("Load Job Data");
            this.LoadDataForJob(this.JobNbr);
        }

        private void EngOrder_JobNbrChanged(object? sender, string e)
        {
            this.LoadDataForJob(e);
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = new TextBlock()
            {
                Text = e.Row.GetIndex().ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = 32 - 8 - 1,
                TextAlignment = TextAlignment.Right,
                Margin = new (0),
                Padding = new (0)
            };
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
                this.ToggleFiltersPanel();
                e.Handled = true;
            }
        }

        private void Page_EngOrder_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.F && (Keyboard.Modifiers & ModifierKeys.Control) is ModifierKeys.Control)
            {
                this.ToggleFiltersPanel();
                e.Handled = true;
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
            this.ToggleFiltersPanel();
            e.Handled = true;
        }

        public static readonly DependencyProperty Pending_LineChangesProperty =
            DependencyProperty.Register(
                "Pending_LineChanges", typeof(bool), typeof(EngOrder),
                new PropertyMetadata(false)
            );

        private bool Pending_LineChanges {
            get => (bool)GetValue(Pending_LineChangesProperty);
            set => SetValue(Pending_LineChangesProperty, (bool)value);
        }
        public bool NoPending_LineChanges { get => !this.Pending_LineChanges; }
        private void datagrid_main_Loaded(object sender, RoutedEventArgs e)
        {
            this.grid_dataeditregion.Children.Clear();
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
                txt.PreviewKeyDown += this.Txt_PreviewKeyDown;
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

        private void Txt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            this.Pending_LineChanges = true;
        }

        private void Btn_SaveHeader_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void Btn_AcceptItemLineEdits_Click(object sender, RoutedEventArgs e)
        {
            if (Ext.PopupConfirmation("Accept changes made to item line? This action cannot be undone.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) is not MessageBoxResult.Yes) return;
            if (this.grid_dataeditregion.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
            List<(string, bool, Type?, object?)> changes = [];
            if (this.datagrid_main.SelectedItem is not example_queries_GetItemLinesByJobResult line) return;
            foreach (var item in txts)
            {
                var binding = item.GetBindingExpression(TextBox.TextProperty);
                var propertyName = binding.ParentBinding.Path.Path.Split('.').Last();
                if (propertyName is null ||
                    this.DataGrid_IceManuf_ColumnsExcludedHidden.Contains(propertyName) ||
                    this.DataGrid_IceManuf_ColumnsReadonly.Contains(propertyName))
                {
                    continue;
                }
                this.datagrid_main.BeginEdit();
                var propRes = UpdateProperty(line, propertyName, item.Text);
                this.datagrid_main.CommitEdit();
                if (propRes.Item2 is true)
                {
                    changes.Add(propRes);
                }
                // Trigger Cell & Row Edit events
                /*
                item.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode = BindingMode.OneWayToSource;
                item.Text = item.Text;
                item.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode = BindingMode.OneWay;
                */
            }
            if (changes.Count is 0) return;
            this.UpdateItemLineData(line.IceManufID, line);
            Debug.WriteLine(string.Join("\n", changes.Select(c => $"{c.Item1}: {c.Item3} = {c.Item4}")));
            this.Pending_LineChanges = false;
        }

        private void Btn_RevertItemLineEdits_Click(object sender, RoutedEventArgs e)
        {
            if (Ext.PopupConfirmation("Discard changes made to item line? All unsaved changes will be lost.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) is not MessageBoxResult.Yes) return;
            if (this.grid_dataeditregion.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
            foreach (var item in txts)
            {
                var binding = item.GetBindingExpression(TextBox.TextProperty);
                //var propertyName = binding.ParentBinding.Path.Path.Split('.').Last();
                binding.UpdateTarget();
                // Trigger Cell & Row Edit events
                /*
                item.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode = BindingMode.OneWayToSource;
                item.Text = item.Text;
                item.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode = BindingMode.OneWay;
                */
            }
            this.Pending_LineChanges = false;
        }

        private void Btn_DeleteItemLine_Click(object sender, RoutedEventArgs e)
        {
            /*
            Ext.PopupConfirmation("Astr", "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            Ext.PopupConfirmation("Errr", "", MessageBoxButton.YesNo, MessageBoxImage.Error);
            Ext.PopupConfirmation("Excl", "", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            Ext.PopupConfirmation("Hand", "", MessageBoxButton.YesNo, MessageBoxImage.Hand);
            Ext.PopupConfirmation("Info", "", MessageBoxButton.YesNo, MessageBoxImage.Information);
            Ext.PopupConfirmation("None", "", MessageBoxButton.YesNo, MessageBoxImage.None);
            Ext.PopupConfirmation("Ques", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            Ext.PopupConfirmation("Stop", "", MessageBoxButton.YesNo, MessageBoxImage.Stop);
            Ext.PopupConfirmation("Warn", "", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            */
            if (Ext.PopupConfirmation("Are you sure you want to delete this item line? This action cannot be undone.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Stop) is not MessageBoxResult.Yes) return;
            if (this.datagrid_main.SelectedItem is not example_queries_GetItemLinesByJobResult line) return;
            var res = this.DeleteItemLine(line.IceManufID, line.JobNbr);
            if (res)
                this.MfgItemLines.Remove(line);
            else
                MessageBox.Show("Deletion failed");
        }

        private void Btn_NewItemLine_Click(object sender, RoutedEventArgs e)
        {
            if (Ext.PopupConfirmation("Are you sure you'd like to create a new line using the current line's values? New line will have \"(COPY)\" append to the end of its description", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) is not MessageBoxResult.Yes) return;
            if (this.datagrid_main.SelectedItem is not example_queries_GetItemLinesByJobResult line) return;
            this.CopyItemLineData(line.IceManufID, line);
            if (this.JobNbr is null) return;
            this.LoadManufData(this.JobNbr);
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

        private void Btn_FilterClose_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
