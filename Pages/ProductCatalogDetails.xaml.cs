using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OMPS.ViewModels;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
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
using System.Configuration;
using System.Diagnostics;
using OMPS.DBModels;

namespace OMPS.Pages
{
    /// <summary>
    /// Interaction logic for ProductCatalogDetails.xaml
    /// </summary>
    public partial class ProductCatalogDetails : UserControl, INotifyPropertyChanged
    {
        public ProductCatalogDetails()
        {
            InitializeComponent();
            this.PropertyChanged += ProductCatalogDetails_PropertyChanged;
        }

        private async void ProductCatalogDetails_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not string propName) return;
        }


        #region "Events"
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion


        #region "Properties"
        public bool isLoaded { get; set; } = false;
        public string ProductCode {
            get => field;
            set
            {
                if (value is null or "") return;
                if (value == field) return;
                field = value;
                OnPropertyChanged(nameof(ProductCode));
                this.LoadAllProductData();
            }
        } = "Default";

        internal DataGrid? CurrentGrid { get; set; }

        public DBModels.Product.IcProductCatalog ProductInfo { get; set; } = new();

        public List<DBModels.Product.IcProdBom> _prodBoms = [];
        public IReadOnlyCollection<DBModels.Product.IcProdBom> ProdBoms => this._prodBoms;

        public List<DBModels.Product.IcMfgBom> _mfgItems = [];
        public IReadOnlyCollection<DBModels.Product.IcMfgBom> MfgItems => this._mfgItems;

        public static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }
        internal static MainWindow ParentWindow { get => Ext.MainWindow; }
        internal static double DataGridFontSize { get => Ext.MainViewModel.FontSize_Base; }


        #endregion


        #region "Methods"
        public async Task LoadAllProductData()
        {
            isLoaded = true;
            this.ProgressBar_Show();

            await LoadProductInfo();
            await LoadProductBomItems();
            await LoadProductMfgItems();

            this.ProgressBar_Hide();
        }

        public async Task LoadProductInfo()
        {
            await Task.Run(async () =>
            {
                using var ctx = new DBModels.Product.ProductDbCtx();
                var query = ctx.IcProductCatalogs
                    .Where(p => p.ProductCode == this.ProductCode);
                var res = await query.FirstOrDefaultAsync();
                if (res is null) return;
                this.ProductInfo = res;
                this.ProductInfo.PropertyChanged += ProductInfo_PropertyChanged; ;
                OnPropertyChanged(nameof(ProductInfo));
            });
        }

        public async Task LoadProductBomItems()
        {
            this.DataGrid_ProdBoms.BeginInit();
            await Task.Run(async () =>
            {
                using var ctx = new DBModels.Product.ProductDbCtx();
                var query = ctx.IcProdBoms
                    .Include(e => e.Product)
                    .Where(p => p.Product != null && p.Product.ProductCode == this.ProductCode);
                var res = await query.ToListAsync();
                if (res is null) return;
                this._prodBoms = res;
                OnPropertyChanged(nameof(ProdBoms));
            });
            this.DataGrid_ProdBoms.EndInit();
        }

        public async Task LoadProductMfgItems()
        {
            this.DataGrid_MfgItems.BeginInit();
            await Task.Run(async () =>
            {
                using var ctx = new DBModels.Product.ProductDbCtx();
                var query = ctx.IcMfgBoms
                    .Include(e => e.Product)
                    .Where(p => p.Product != null && p.Product.ProductCode == this.ProductCode);
                var res = await query.ToListAsync();
                if (res is null) return;
                this._mfgItems = res;
                OnPropertyChanged(nameof(MfgItems));
            });
            this.DataGrid_MfgItems.EndInit();
        }

        public void ProgressBar_Show()
        {
            this.progbar.Value = 50;
            this.progbar.IsEnabled = true;
            this.progbar.Visibility = Visibility.Visible;
        }
        public void ProgressBar_Hide()
        {
            this.progbar.IsEnabled = false;
            this.progbar.Visibility = Visibility.Collapsed;
        }

        public void ToggleHeader()
        {
            this.grid_header.Visibility =
                this.grid_header.Visibility is Visibility.Collapsed ?
                Visibility.Visible :
                Visibility.Collapsed;
            this.grid_header.ShowGridLines = true;
            this.Btn_CollapseTopBar.IsChecked = this.grid_header.Visibility is Visibility.Visible;
        }

        public void ToggleSideGrid()
        {
            /*
            this.pnl_dock.Visibility =
                this.pnl_dock.Visibility is Visibility.Collapsed ?
                Visibility.Visible :
                Visibility.Collapsed;
            this.RowSpan = (this.pnl_dock.Visibility is Visibility.Collapsed ? 2 : 1);
            Grid.SetColumnSpan(datagrid_main, RowSpan);
            this.Btn_CollapseSideGrid.IsChecked = this.pnl_dock.Visibility is Visibility.Visible;
            */
        }

        public void ToggleDataGrid()
        {
            var currentlyHidden = this.grid_dataGrids.Visibility is Visibility.Collapsed;
            this.grid_dataGrids.Visibility = currentlyHidden ? Visibility.Visible : Visibility.Collapsed;
            this.grid_header.ShowGridLines = true;
            this.Btn_CollapseDataGrid.IsChecked = currentlyHidden;
        }
        #endregion


        #region "EventHandlers"
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ProductInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not string propName) return;
            MessageBox.Show(propName);
        }

        private void Btn_RefreshHeader_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_SaveHeader_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_CollapseTopBar_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleHeader();
        }

        private void Btn_CollapseDataGrid_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleDataGrid();
        }

        private void Btn_CollapseSideGrid_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleSideGrid();
        }

        private void RadioButton_DataGridView_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void MfgItemsViewSource_Filter(object sender, FilterEventArgs e)
        {
            // Assuming 'MyDataItem' is the type of objects in collection
            if (sender is not DBModels.Product.IcMfgBom item)
            {
                return;
            }

            // Get text from TextBox
            var filterText = "";
            // If the filter text is empty, accept all items
            if (filterText is null || filterText is "")
            {
                e.Accepted = true;
                return;
            }
            //e.Accepted = Ext.MfgItems_Filter(item, filterText);
        }
        private void ProdBomsViewSource_Filter(object sender, FilterEventArgs e)
        {
            // Assuming 'MyDataItem' is the type of objects in collection
            if (sender is not DBModels.Product.IcProdBom item)
            {
                return;
            }

            // Get text from TextBox
            var filterText = "";
            // If the filter text is empty, accept all items
            if (filterText is null || filterText is "")
            {
                e.Accepted = true;
                return;
            }
            //e.Accepted = Ext.MfgItems_Filter(item, filterText);
        }
        #endregion

    }
}
