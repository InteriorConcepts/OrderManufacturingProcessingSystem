using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using MyApp.DataAccess.Generated;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using OMPS.Windows;
using OMPS.viewModel;

namespace OMPS.Pages
{
    /// <summary>
    /// Interaction logic for JobSearch.xaml
    /// </summary>
    public partial class OrderSearch : UserControl, INotifyPropertyChanged, IDisposable
    {
        public OrderSearch() { }
        public OrderSearch(MainWindow parentWindow)
        {
            InitializeComponent();
            //
            this.DataContext = this;
            this.RefreshDelay.Elapsed += this.RefreshDelay_Elapsed;
            this.ParentWindow = parentWindow;
        }

        public Main_ViewModel MainViewModel
        {
            get => this.ParentWindow.MainViewModel;
        }

        public double DataGridFontSize
        {
            get => this.MainViewModel.FontSize_DataGrid;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MainViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not "FontSize_DataGrid") return;
            OnPropertyChanged(nameof(DataGridFontSize));
        }

        private void RefreshDelay_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                this.Btn_OrdersRefresh.IsEnabled = true;
            });
            this.RefreshDelay.Stop();
        }


        #region Events

        #endregion


        #region Properties
        readonly System.Timers.Timer RefreshDelay = new(TimeSpan.FromSeconds(10)) { };
        internal MainWindow ParentWindow { get; set
            {
                field = value;
                value?.MainViewModel?.PropertyChanged += new((sender, e) =>
                {
                    if (e.PropertyName is not nameof(ParentWindow.MainViewModel.FontSize_DataGrid)) return;
                    //this.datagrid_orders.UpdateLayout();
                    OnPropertyChanged(nameof(DataGridFontSize));
                });
            }
        }
        internal TabItem ParentTab { get; set; }
        public ObservableCollection<example_queries_GetColorSetsResult> ColorSetInfos { get; set; } = [];
        #endregion


        #region Fields

        #endregion


        #region Methods
        public void LoadRecentOrders(string filters = "%")
        {
            this.Btn_OrdersRefresh.IsEnabled = false;
            this.ColorSetInfos.Clear();
            Debug.WriteLine(filters);
            this.progbar_orders.Value = 50;
            this.progbar_orders.Visibility = Visibility.Visible;
            this.progbar_orders.IsEnabled = true;
            var t = new Task(() =>
            {
                try
                {
                    Debug.WriteLine(0);
                    var data_orders = Ext.Queries.GetColorSets(filters);
                    Debug.WriteLine(1);
                    /*
                    var CDirs = new DirectoryInfo("C:\\").EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Select(d => d.Name);
                    var HDirs = new DirectoryInfo("H:\\Engineering").EnumerateDirectories("*", SearchOption.AllDirectories).Where(d => d.Name[0] is 'J' or 'S').Select(d => d.Name);

                    Debug.WriteLine(string.Join(", ", HDirs));
                    data_orders.Sort((a, b) =>
                    {
                        var aDirC = CDirs.Contains(a.JobNbr);
                        var bDirC = CDirs.Contains(b.JobNbr);
                        if (aDirC && !bDirC)
                        {
                            //a.JobNbr = "C:/" + a.JobNbr;
                            return -1000;
                        }
                        else if (bDirC && !aDirC)
                        {
                            //b.JobNbr = "C:/" + b.JobNbr;
                            return 1000;
                        }
                        var aDirH = HDirs.Contains(a.JobNbr);
                        var bDirH = HDirs.Contains(b.JobNbr);
                        if (aDirH && !bDirH)
                        {
                            //a.JobNbr = "H:/" + a.JobNbr;
                            return -500;
                        }
                        else if (bDirH && !aDirH)
                        {
                            //b.JobNbr = "H:/" + b.JobNbr;
                            return 500;
                        }
                        return b.OrderDate.CompareTo(a.OrderDate);
                    });
                    */

                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.datagrid_orders.BeginEdit();
                        for (int i = 0; i < data_orders.Count; i++)
                        {
                            this.ColorSetInfos.Add(data_orders[i]);
                        }

                        if (this.datagrid_orders.Items.Count is not 0)
                        {
                            this.datagrid_orders.ScrollIntoView(this.datagrid_orders.Items[0]);
                        }
                        Debug.WriteLine(2);
                        this.datagrid_orders.EndInit();
                        this.progbar_orders.Visibility = Visibility.Collapsed;
                        this.progbar_orders.IsEnabled = false;
                    });
                    this.RefreshDelay.Start();

                    /*
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var offset = this.GetColumnPositionSimple(this.datagrid_orders, 2);
                        this.Txt_JobNbr.Margin = new Thickness(offset.X, this.Txt_JobNbr.Margin.Top, this.Txt_JobNbr.Margin.Right, this.Txt_JobNbr.Margin.Bottom);
                        this.Txt_JobNbr.Width = this.datagrid_orders.Columns.Where(c => c.Header is "JobNbr").First().ActualWidth - 1;
                        this.Txt_QuoteNbr.Width = this.datagrid_orders.Columns.Where(c => c.Header is "QuoteNbr").First().ActualWidth - 1;
                        this.Txt_OrderNbr.Width = this.datagrid_orders.Columns.Where(c => c.Header is "OrderNumber").First().ActualWidth - 1;
                        this.Txt_OrderName.Width = this.datagrid_orders.Columns.Where(c => c.Header is "Name").First().ActualWidth - 1;
                        this.Txt_OppNbr.Width = this.datagrid_orders.Columns.Where(c => c.Header is "OpportunityNbr").First().ActualWidth - 1;
                    });
                    */

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            });
            Console.WriteLine("Finished Query");
            t.Start();
            if (filters is "%")
            {
                //this.ParentTab.Header = "Order Search";
            }
            else
            {
                //this.ParentTab.Header = $"Order Search  ({filters})";
            }
        }

        private Point GetColumnPositionSimple(DataGrid grid, int columnIndex)
        {
            var header = FindVisualChild<DataGridColumnHeadersPresenter>(grid)
                ?.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridColumnHeader;

            return header?.TransformToAncestor(grid).Transform(new Point(0, 0)) ?? new Point(0, 0);
        }

        private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result) return result;
                var found = FindVisualChild<T>(child);
                if (found != null) return found;
            }
            return null;
        }
        #endregion


        #region EventHandlers
        private readonly ReadOnlyCollection<string> DataGrid_Orders_ColumnsIncluded =
            [
            "QuoteNbr", "ColorNumber", "OpportunityNbr", "OrderDate", "Name",
            "LineNumber", "LineItem", "LineDescription", "SupplyOrderRef",
            "CompanyName", "ShipToName"
            ];
        private readonly ReadOnlyCollection<string> DataGrid_Orders_ColumnsExcludeHidden =
            [
            "ColorSetID", "QuoteNbr"
            ];
        private readonly ReadOnlyCollection<string> DataGrid_Orders_ColumnHyperlinks =
            [
            "JobNbr", "QuoteNbr", "OrderNumber"
            ];
        private readonly ReadOnlyCollection<string> DataGrid_Orders_ColumnsOrder = [
            "JobNbr", /*"QuoteNbr",*/ "OrderNumber", "Name", "OpportunityNbr"
            ];

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() is not string headerName)
            {
                return;
            }
            if (DataGrid_Orders_ColumnHyperlinks.Contains(headerName))
            {
                var defaultStyle = (Style)FindResource(typeof(DataGridCell));
                Style cellStyle = new (typeof(DataGridCell));
                if (defaultStyle is not null)
                {
                    cellStyle.BasedOn = defaultStyle;
                }
                cellStyle.Setters.Add(new Setter(DataGridCell.ForegroundProperty, new SolidColorBrush(Colors.LightSkyBlue)));
                cellStyle.Setters.Add(new Setter(DataGridCell.VerticalAlignmentProperty, VerticalAlignment.Center));
                cellStyle.Setters.Add(new Setter(DataGridCell.VerticalContentAlignmentProperty, VerticalAlignment.Stretch));
                cellStyle.Setters.Add(new Setter(DataGridCell.HorizontalContentAlignmentProperty, HorizontalAlignment.Left));
                cellStyle.Setters.Add(new Setter(DataGridCell.CursorProperty, Cursors.Hand));
                cellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.Transparent));
                var bind = new Binding("FontSize_DataGrid")
                {
                    Path = new("MainViewModel.FontSize_DataGrid"),
                    Source = this.ParentWindow,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay
                };
                cellStyle.Setters.Add(new Setter(DataGridCell.FontSizeProperty, bind));
                e.Column.SetValue(DataGridColumn.CellStyleProperty, cellStyle);
            }
            if (DataGrid_Orders_ColumnsOrder.IndexOf(headerName) is int headerIdx && headerIdx is not -1)
            {
                e.Column.DisplayIndex = headerIdx;
            }
            e.Column.Width = new DataGridLength(125);
            //Debug.WriteLine(headerName);
            e.Column.Visibility =
                this.DataGrid_Orders_ColumnsExcludeHidden.Contains(headerName) ?
                Visibility.Collapsed :
                Visibility.Visible;

        }

        private void datagrid_orders_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Grid Loaded");
            if (datagrid_orders.Columns.Count is 0) return;
            for (short i = 0; i < datagrid_orders.Items.Count; i++)
            {
                //Debug.WriteLine(datagrid_orders.Items[i]);
                var row = (DataGridRow)datagrid_orders.ItemContainerGenerator.ContainerFromIndex(i);
                this.SetOrdersRowHeader(row);
            }
        }

        private void datagrid_orders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton is not MouseButton.Left) return;
            if (datagrid_orders.SelectedItem is not example_queries_GetColorSetsResult item) return;
            if (!Ext.IsJobNumValid(item.JobNbr)) return;
            //this.ParentWindow.Tab_Create_EngOrder().page?.JobNbr = item.JobNbr;
            //this.ParentWindow.Page_EngOrder.JobNbr = item.JobNbr;
            this.ParentWindow.MainViewModel.EngOrder_VM.JobNbr = item.JobNbr;
            this.ParentWindow.MainViewModel.CurrentPage = PageTypes.EngOrder;
        }

        private void OrdersViewSource_Filter(object sender, FilterEventArgs e)
        {
            // Assuming 'MyDataItem' is the type of objects in collection
            if (e.Item is not example_queries_GetColorSetsResult item)
            {
                return;
            }

            // Get text from TextBox
            var filterText = Txt_JobNbr.Text.ToLower();
            // If the filter text is empty, accept all items
            if (filterText is null || string.IsNullOrWhiteSpace(filterText))
            {
                //e.Accepted = true;
                e.Accepted = true;
                return;
            }
            e.Accepted = item.JobNbr.Contains(filterText);
        }

        private void Txt_JobNbr_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is not Key.Enter) return;
            this.LoadRecentOrders($"%{this.Txt_JobNbr.Text}%");
            var viewSource = (CollectionViewSource)Resources["OrdersViewSource"];
            viewSource?.View?.Refresh();
        }

        private void Btn_OrdersRefresh_Click(object sender, RoutedEventArgs e)
        {
            this.LoadRecentOrders();
        }

        public void SetOrdersRowHeader(DataGridRow row)
        {
            return;
            if (row.Item is not example_queries_GetColorSetsResult item) return;

            IEnumerable<string>? matchesC = Directory.EnumerateDirectories("C:\\", $"{item.JobNbr}", SearchOption.TopDirectoryOnly);
            IEnumerable<string>? matchesH = Directory.EnumerateDirectories("H:\\Engineering", $"{item.JobNbr}", SearchOption.AllDirectories);
            bool foundC = matchesC.Any(),
                 foundH = matchesH.Any();
            if (foundC is false && foundH is false) return;

            PackIconKind icon = PackIconKind.None;
            string text = "";
            if (foundC && !foundH)
            {
                icon = PackIconKind.FolderHome;
                text = "C";
                Debug.WriteLine(string.Join(", ", matchesC));
            }
            else if (foundH && !foundC)
            {
                icon = PackIconKind.FolderNetwork;
                text = "H";
                Debug.WriteLine(string.Join(", ", matchesH));
            }
            else if (foundC && foundH)
            {
                icon = PackIconKind.Folders;
                Debug.WriteLine(string.Join(", ", matchesH.Concat(matchesC)));
            }
            else
            {
                return;
            }

            var grd = new Grid() { VerticalAlignment = VerticalAlignment.Center };
            grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            var pkicon =
                new PackIcon()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new(0),
                    Kind = icon
                };
            var txt =
                new TextBlock()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    Margin = new(0),
                    Text = text,
                    FontFamily = new FontFamily("Consolas"),
                    Foreground = Brushes.White
                };
            grd.Children.Add(pkicon);
            grd.Children.Add(txt);
            Grid.SetColumn(pkicon, 0);
            Grid.SetColumn(txt, 1);
            row.Header = grd;
        }

        private void datagrid_orders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            this.SetOrdersRowHeader(e.Row);
        }

        private async void datagrid_orders_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton is not MouseButton.Left) return;
            if (this.datagrid_orders.SelectedItem is not example_queries_GetColorSetsResult item) return;
            if (this.datagrid_orders.SelectedCells is not IList<DataGridCellInfo> cells || cells.Count is 0) return;
            var cell = this.datagrid_orders.CurrentCell;
            Debug.WriteLine(cell.Column.Header.ToString());
            if (cell.Column.Header.ToString() is "JobNbr")
            {
                if (!Ext.IsJobNumValid(item.JobNbr)) return;
                this.ParentWindow.MainViewModel.EngOrder_VM.JobNbr = item.JobNbr;
                this.ParentWindow.MainViewModel.CurrentPage = PageTypes.EngOrder;
                return;
            }
            if (cell.Column.Header.ToString() is "QuoteNbr" or "OrderNumber")
            {
                //this.ParentWindow.MainViewModel.QuoteOrder_VM.QuoteNbr = item.QuoteNbr;
                this.ParentWindow.MainViewModel.CurrentPage = PageTypes.QuoteOrder;
            }
            //this.ParentWindow.Tab_Create_EngOrder().page?.JobNbr = item.JobNbr;
            //this.ParentWindow.Page_EngOrder.JobNbr = item.JobNbr;
        }
        #endregion

        private void datagrid_orders_Sorting(object sender, DataGridSortingEventArgs e)
        {
            
        }

        private void datagrid_orders_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.RefreshDelay.Elapsed -= this.RefreshDelay_Elapsed;
            this.RefreshDelay.Dispose();
            this.ColorSetInfos.Clear();
        }
    }
}
