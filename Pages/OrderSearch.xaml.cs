using MyApp.DataAccess.Generated;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for JobSearch.xaml
    /// </summary>
    public partial class OrderSearch : Page
    {
        public OrderSearch(MainWindow parentWindow)
        {
            InitializeComponent();
            //
            this.DataContext = this;
            this.ParentWindow = parentWindow;
        }


        #region Events

        #endregion


        #region Properties
        private MainWindow ParentWindow { get; }
        internal TabItem ParentTab { get; set; }
        public ObservableCollection<example_queries_GetColorSetsResult> ColorSetInfos { get; set; } = [];
        #endregion


        #region Fields

        #endregion


        #region Methods
        public void LoadRecentOrders(string filters = "%")
        {
            this.ColorSetInfos.Clear();
            var t = new Task(() =>
            {
                try
                {
                    var data_orders = Ext.Queries.GetColorSets(filters);
                    Application.Current.Dispatcher.Invoke(() =>
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
                        this.datagrid_orders.EndInit();
                    });
                    
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("****");
                        var offset = this.GetColumnPositionSimple(this.datagrid_orders, 2);
                        this.Txt_JobNbr.Margin = new Thickness(offset.X, this.Txt_JobNbr.Margin.Top, this.Txt_JobNbr.Margin.Right, this.Txt_JobNbr.Margin.Bottom);
                        this.Txt_JobNbr.Width = this.datagrid_orders.Columns.Where(c => c.Header is "JobNbr").First().ActualWidth - 1;
                        this.Txt_QuoteNbr.Width = this.datagrid_orders.Columns.Where(c => c.Header is "QuoteNbr").First().ActualWidth - 1;
                        this.Txt_OrderNbr.Width = this.datagrid_orders.Columns.Where(c => c.Header is "OrderNumber").First().ActualWidth - 1;
                        this.Txt_OrderName.Width = this.datagrid_orders.Columns.Where(c => c.Header is "Name").First().ActualWidth - 1;
                        this.Txt_OppNbr.Width = this.datagrid_orders.Columns.Where(c => c.Header is "OpportunityNbr").First().ActualWidth - 1;
                    });
                    
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            });
            Console.WriteLine("Finished Query");
            t.Start();
            if (filters is "%")
            {
                this.ParentTab.Header = "Order Search";
            }
            else
            {
                this.ParentTab.Header = $"Order Search  ({filters})";
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
            "ColorSetID"
            ];

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() is not string headerName)
            {
                return;
            }
            //Debug.WriteLine(headerName);
            e.Column.Visibility =
                this.DataGrid_Orders_ColumnsExcludeHidden.Contains(headerName) ?
                Visibility.Collapsed :
                Visibility.Visible;

        }

        private void datagrid_orders_Loaded(object sender, RoutedEventArgs e)
        {
            if (datagrid_orders.Columns.Count is 0) return;
        }

        private void datagrid_orders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton is not MouseButton.Left) return;
            if (datagrid_orders.SelectedItem is not example_queries_GetColorSetsResult item) return;
            if (!Ext.IsJobNumValid(item.JobNbr)) return;
            this.ParentWindow.Tab_Create_EngOrder().page?.JobNbr = item.JobNbr;
            //this.ParentWindow.Page_EngOrder.JobNbr = item.JobNbr;
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
            var viewSource = (CollectionViewSource)Resources["OrdersViewSource"];
            viewSource?.View?.Refresh();
        }
        #endregion

    }
}
