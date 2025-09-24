using MyApp.DataAccess.Generated;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
                    });
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (this.datagrid_orders.Items.Count is not 0)
                        {
                            this.datagrid_orders.ScrollIntoView(this.datagrid_orders.Items[0]);
                        }
                        this.datagrid_orders.EndInit();
                    });
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            });
            Console.WriteLine("Finished Query");
            t.Start();
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
            Debug.WriteLine(headerName);
            e.Column.Visibility =
                this.DataGrid_Orders_ColumnsExcludeHidden.Contains(headerName) ?
                Visibility.Collapsed :
                Visibility.Visible;
        }

        private void datagrid_orders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton is not MouseButton.Left) return;
            if (datagrid_orders.SelectedItem is not example_queries_GetColorSetsResult item) return;
            if (!Ext.IsJobNumValid(item.JobNbr)) return;
            this.ParentWindow.NavigateToPage(MainWindow.PageTypes.EngOrder);
            this.ParentWindow.Page_EngOrder.JobNbr = item.JobNbr;
        }
        #endregion
    }
}
