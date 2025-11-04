using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using MyApp.DataAccess.Generated;
using OMPS.viewModel;
using OMPS.Windows;
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
using System.Threading.Tasks;
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

namespace OMPS.Pages
{
    /// <summary>
    /// Interaction logic for JobSearch.xaml
    /// </summary>
    public partial class OrderSearch : UserControl, INotifyPropertyChanged, IDisposable
    {
        public OrderSearch()
        {
            InitializeComponent();
            //
            this.DataContext = this;
            this.RefreshDelay.Elapsed += this.RefreshDelay_Elapsed;
            //this.LoadRecentOrders();
        }




        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion


        #region Properties

        internal static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }
        internal static MainWindow ParentWindow { get => Ext.MainWindow; }
        internal static double DataGridFontSize { get => MainViewModel.FontSize_Base; }

#if NEWDBSQL
        public List<Models.Order.AIcColorSet> _colorSetInfos = [];
        public IReadOnlyCollection<Models.Order.AIcColorSet> ColorSetInfos => this._colorSetInfos;
#else
        public ObservableCollection<example_queries_GetColorSetsResult> ColorSetInfos { get; set; } = [];
#endif
        #endregion


        #region Fields
        readonly System.Timers.Timer RefreshDelay = new(TimeSpan.FromSeconds(10)) { };
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
            /*"JobNbr", "QuoteNbr", "OrderNumber"*/
            ];
        #endregion


        #region Methods
        public async void LoadRecentOrders(string filters = "%")
        {
            this.Btn_OrdersRefresh.IsEnabled = false;
            Debug.WriteLine(filters);
            this.progbar_orders.IsEnabled = true;
            this.progbar_orders.Visibility = Visibility.Visible;
            this.progbar_orders.Value = 50;
            this.datagrid_orders.BeginEdit();

#if NEWDBSQL
            using (var ctx = new Models.Order.OrderDbCtx())
            {
                var cutoff = DateTime.Now.AddDays(-60);
                this._colorSetInfos = await ctx.AIcColorSets
                    .Where(cs => cs.OrderDate >= cutoff)
                    .Where(cs => (cs.SupplyOrderRef ?? "").Contains(filters.Replace("%", "")))
                    .OrderBy(cs => cs.OrderDate)
                    .Take(25)
                    .AsNoTracking()
                    .AsSingleQuery()
                    .ToListAsync();
            }
            OnPropertyChanged(nameof(ColorSetInfos));
#else
            this.ColorSetInfos.Clear();
            await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine(0);
                    var data_orders = Ext.Queries.GetColorSets(filters);
                    Debug.WriteLine(1);

                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        for (int i = 0; i < data_orders.Count; i++)
                        {
                            Debug.WriteLine("Order");
                            this.ColorSetInfos.Add(data_orders[i]);
                        }

                        if (this.datagrid_orders.Items.Count is not 0)
                        {
                            this.datagrid_orders.ScrollIntoView(this.datagrid_orders.Items[0]);
                        }
                        Debug.WriteLine(2);
                    });

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            });
#endif
            this.datagrid_orders.EndInit();
            this.RefreshDelay.Start();
            Console.WriteLine("Finished Query");
            this.progbar_orders.Visibility = Visibility.Collapsed;
            this.progbar_orders.IsEnabled = false;
            if (filters is "%")
            {
                //this.ParentTab.Header = "Order Search";
            }
            else
            {
                //this.ParentTab.Header = $"Order Search  ({filters})";
            }
        }

        
#endregion


        #region EventHandlers
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RefreshDelay_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                this.Btn_OrdersRefresh.IsEnabled = true;
            });
            this.RefreshDelay.Stop();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() is not string headerName)
            {
                return;
            }
            if (headerName is "JobNbr" or "OrderNumber")
            {
                e.Cancel = true;
                return;
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
            /*
            if (datagrid_orders.Columns.Count is 0) return;
            for (short i = 0; i < datagrid_orders.Items.Count; i++)
            {
                //Debug.WriteLine(datagrid_orders.Items[i]);
                var row = (DataGridRow)datagrid_orders.ItemContainerGenerator.ContainerFromIndex(i);
            }
            */
        }

        private void datagrid_orders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton is not MouseButton.Left) return;
#if NEWDBSQL
            if (datagrid_orders.SelectedItem is not Models.Order.AIcColorSet item) return;
            if (item.SupplyOrderRef is null || !Ext.IsJobNumValid(item.SupplyOrderRef)) return;
            Ext.MainViewModel.EngOrder_VM.JobNbr = item.SupplyOrderRef;
#else
            if (datagrid_orders.SelectedItem is not example_queries_GetColorSetsResult item) return;
            if (!Ext.IsJobNumValid(item.JobNbr)) return;
            //Ext.MainWindow.Tab_Create_EngOrder().page?.JobNbr = item.JobNbr;
            //Ext.MainWindow.Page_EngOrder.JobNbr = item.JobNbr;
            Ext.MainViewModel.EngOrder_VM?.JobNbr = item.JobNbr;
#endif
            Ext.MainViewModel.CurrentPage = PageTypes.EngOrder;
        }

        private void OrdersViewSource_Filter(object sender, FilterEventArgs e)
        {
#if NEWDBSQL
            if (e.Item is not Models.Order.AIcColorSet item)
            {
                return;
            }
#else
            if (e.Item is not example_queries_GetColorSetsResult item)
            {
                return;
            }
#endif

            // Get text from TextBox
            var filterText = Txt_JobNbr.Text.ToLower();
            // If the filter text is empty, accept all items
            if (filterText is null || string.IsNullOrWhiteSpace(filterText))
            {
                //e.Accepted = true;
                e.Accepted = true;
                return;
            }
#if NEWDBSQL
            e.Accepted = (item.SupplyOrderRef is not null && item.SupplyOrderRef.Contains(filterText));
#else
            e.Accepted = item.JobNbr.Contains(filterText);
#endif
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

        public static void SetupOrderRowHeader(DataGridRow row)
        {

#if NEWDBSQL
            if (row.Item is not Models.Order.AIcColorSet item) return;

            var matchesC = JobFoldersC?.FirstOrDefault(d => d.Name == item.SupplyOrderRef);
            var matchesH = JobFoldersH?.FirstOrDefault(d => d.Name == item.SupplyOrderRef);
#else
            if (row.Item is not example_queries_GetColorSetsResult item) return;

            var matchesC = Ext.JobFoldersC?.FirstOrDefault(d => d.Name == item.JobNbr);
            var matchesH = Ext.JobFoldersH?.FirstOrDefault(d => d.Name == item.JobNbr);
#endif

            bool foundC = matchesC is not null,
                 foundH = matchesH is not null;

            if (!foundC && !foundH)
            {
                row.Header = " ";
                return;
            }

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
            }
            else
            {
                icon = PackIconKind.None;
                text = "";
            }

            //row.ApplyTemplate();
            var rowHeader = Ext.FindVisualChild<DataGridRowHeader>(row);
            rowHeader?.ApplyTemplate();
            var grid = rowHeader?.Template.FindName("Grid_RowHeader", rowHeader) as Grid;
            var foo = grid?.Children.OfType<UIElement>();
            //grid?.Tag = new DirectoryInfo?[] { matchesH, matchesC };
            foreach (var dir in new[] { matchesH, matchesC })
            {
                if (dir is null) continue;
                var mi = new MenuItem
                {
                    Header = Ext.TruncatePath(dir.FullName),
                    FontSize = 12,
                    Tag = dir,
                    ContextMenu = new() { }
                };
                mi.ContextMenu.Items.Add(dir.FullName);
                mi.Click += (sender, e) =>
                {
                    Process.Start("explorer.exe", dir.FullName);
                };
                grid?.ContextMenu?.Items.Add(mi);
            }
            /*
            var tt = grid?.ToolTip as ToolTip;
            tt?.Content = "Folders\n" + string.Join("\n", (grid?.Tag as DirectoryInfo?[])?.Select(d => d?.Name) ?? []);
            */
            var ico = grid?.Children[0] as PackIcon;
            var txt = grid?.Children[1] as TextBlock;
            ico?.Kind = icon;
            txt?.Text = text;
            /*
            var grd = row.Header as Grid;
            grd?.Tag = new DirectoryInfo?[] { matchesH, matchesC };
            var tt = grd?.ToolTip as ToolTip;
            tt?.Content = "Folders\n" + string.Join("\n", (grd?.Tag as DirectoryInfo?[])?.Select(d => d?.Name) ?? []);
            var ico = grd?.Children[0] as PackIcon;
            var txt = grd?.Children[1] as TextBlock;
            ico?.Kind = icon;
            txt?.Text = text;
            */


            /////
            // Slow AF!
            /////
            /////
            /*
            var grd = new Grid
            {
                VerticalAlignment = VerticalAlignment.Center,
                Tag = new DirectoryInfo?[] { matchesH, matchesC },
                ContextMenu = new ContextMenu()
                {
                    Background = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                    Foreground = Brushes.GhostWhite,
                    BorderBrush = Brushes.White,
                    BorderThickness = new(2),
                }
            };
            grd.ContextMenu.Items.Add(new MenuItem()
            {
                IsEnabled = false,
                Header = "Folders"
            });
            foreach (var dir in grd.Tag as DirectoryInfo?[])
            {
                if (dir is null) continue;
                var mi = new MenuItem
                {
                    Header = TruncatePath(dir.FullName),
                    FontSize = 12,
                    Tag = dir,
                    ContextMenu = new() { }
                };
                mi.ContextMenu.Items.Add(dir.FullName);
                mi.Click += (sender, e) =>
                {
                    Process.Start("explorer.exe", dir.FullName);
                };
                grd.ContextMenu.Items.Add(mi);
            }
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
            grd.MouseLeftButtonUp += new MouseButtonEventHandler((sender, e) =>
            {
                if (sender is not Grid g) return;
                if (g.Tag is not object tag) return;
                if (tag is not IEnumerable<DirectoryInfo?> paths) return;
                if (!paths.Any()) return;
                if (paths.FirstOrDefault() is DirectoryInfo first)
                    MessageBox.Show(first.FullName);
            });
            Grid.SetColumn(pkicon, 0);
            Grid.SetColumn(txt, 1);
            row.Header = grd;
            */
        }

        public static void SetOrdersRowHeader(DataGridRow row)
        {
            //return;
#if NEWDBSQL
            if (row.Item is not Models.Order.AIcColorSet item) return;
#else
            if (row.Item is not example_queries_GetColorSetsResult item) return;
#endif

            if (row.Template.FindName("Grid_RowHeader", row) is Grid)
            {
                return;
            } else
            {
                SetupOrderRowHeader(row);
            }
        }

        private void datagrid_orders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //Debug.WriteLine("row");
            //this.SetOrdersRowHeader(e.Row);
        }

        private async void datagrid_orders_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton is not MouseButton.Left) return;
#if NEWDBSQL
            if (this.datagrid_orders.SelectedItem is not Models.Order.AIcColorSet item) return;
#else
            if (this.datagrid_orders.SelectedItem is not example_queries_GetColorSetsResult item) return;
#endif
            if (this.datagrid_orders.SelectedCells is not IList<DataGridCellInfo> cells || cells.Count is 0) return;
            var cell = this.datagrid_orders.CurrentCell;
            Debug.WriteLine(cell.Column.Header.ToString());
            if (cell.Column.Header.ToString() is "JobNbr")
            {
#if NEWDBSQL
                if (item.SupplyOrderRef is null || !Ext.IsJobNumValid(item.SupplyOrderRef)) return;
                Ext.MainViewModel.EngOrder_VM.JobNbr = item.SupplyOrderRef;
#else
                if (!Ext.IsJobNumValid(item.JobNbr)) return;
                Ext.MainViewModel.EngOrder_VM?.JobNbr = item.JobNbr;
#endif
                Ext.MainViewModel.CurrentPage = PageTypes.EngOrder;
                return;
            }
            if (cell.Column.Header.ToString() is "QuoteNbr" or "OrderNumber")
            {
                Ext.MainViewModel.QuoteOrder_VM?.QuoteNbr = item.QuoteNbr;
                Ext.MainViewModel.CurrentPage = PageTypes.QuoteOrder;
            }
            //Ext.MainWindow.Tab_Create_EngOrder().page?.JobNbr = item.JobNbr;
            //Ext.MainWindow.Page_EngOrder.JobNbr = item.JobNbr;
        }

        private void datagrid_orders_Sorting(object sender, DataGridSortingEventArgs e)
        {

        }

        private void datagrid_orders_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

        }
        #endregion

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.RefreshDelay.Elapsed -= this.RefreshDelay_Elapsed;
            this.RefreshDelay.Dispose();
#if NEWDBSQL
            this._colorSetInfos.Clear();
#else
            this.ColorSetInfos.Clear();
#endif
        }
    }
}
