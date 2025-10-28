using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyApp.DataAccess.Generated;
using OMPS.Models;
using OMPS.viewModel;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
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
using Key = System.Windows.Input.Key;

namespace OMPS.Pages
{

    /// <summary>
    /// Interaction logic for ProductCatalog.xaml
    /// </summary>
    public partial class ProductCatalogSearch : UserControl, INotifyPropertyChanged, IDisposable
    {
        public ProductCatalogSearch()
        {
            InitializeComponent();
            //
            this.DataContext = this;
            //LoadProducts();
        }


        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region Properties
        public bool isLoaded { get; set; } = false;

        private List<Models.Product.IcProductCatalog> _products = [];
        public IReadOnlyList<Models.Product.IcProductCatalog> Products => _products;

        public const ushort ITEMLIMIT_MAX = 500;
        public const ushort ITEMLIMIT_MIN = 1;
        public ushort ItemLimit
        {
            get => field;
            set
            {
                if (value > ITEMLIMIT_MAX) value = ITEMLIMIT_MAX;
                else if (value < ITEMLIMIT_MIN) value = ITEMLIMIT_MIN;
                field = value;
                OnPropertyChanged(nameof(ItemLimit));
            }
        } = 25;

        public uint QueryFilterAmount { get; set { field = value; OnPropertyChanged(nameof(QueryFilterAmount)); } } = 0;
        public uint QueryTotalAmount { get; set { field = value; OnPropertyChanged(nameof(QueryTotalAmount)); } } = 0;


        public Main_ViewModel MainViewModel
        {
            get => Ext.MainWindow.MainViewModel;
        }
        public double DataGridFontSize
        {
            get => MainViewModel.FontSize_Base;
        }

        internal MainWindow ParentWindow
        {
            get; set
            {
                field = value;
                value?.MainViewModel?.PropertyChanged += new((sender, e) =>
                {
                    if (e.PropertyName is not nameof(ParentWindow.MainViewModel.FontSize_Base)) return;
                    //this.datagrid_orders.UpdateLayout();
                    OnPropertyChanged(nameof(DataGridFontSize));
                });
            }
        }

        #endregion

        #region "Methods"
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

        public async void LoadProducts(string? pcodeFilter = null, string? pdescFilter = null)
        {
            isLoaded = true;
            this.ProgressBar_Show();

            await this.LoadProductsAsync(pcodeFilter, pdescFilter);

            this.ProgressBar_Hide();
        }

        // Query method
        public IQueryable<Models.Product.IcProductCatalog> LoadProductsQueryAsync(Models.Product.ProductDbCtx ctx)
            => ctx.IcProductCatalogs
                    .Where(p => p.Status == "A")
                    .OrderBy(p => p.ProductCode)
                    .Select(p => new Models.Product.IcProductCatalog
                    {
                        ProductId = p.ProductId,
                        ProductCode = p.ProductCode,
                        Description = p.Description
                    })
                    .AsNoTracking() // No change tracking
                    .AsSplitQuery();

        public IQueryable<Models.Product.IcProductCatalog> LoadActiveProductsQueryAsync(Models.Product.ProductDbCtx ctx)
            => this.LoadProductsQueryAsync(ctx);

        public async Task LoadProductsAsync(string? pcodeFilter = null, string? pdescFilter = null)
        {
            this.datagrid_main.BeginInit();
#if NEWDBSQL
            await Task.Run(async () =>
            {
                Debug.WriteLine("Products");
                using var ctx = new Models.Product.ProductDbCtx();
                // Safe read-only query
                var query = this.LoadActiveProductsQueryAsync(ctx);
                if (!(pcodeFilter is null or ""))
                {
                    query = query
                        .Where(p => p.ProductCode.ToLower().Contains(pcodeFilter.ToLower()));
                }
                if (!(pdescFilter is null or ""))
                {
                    query = query
                        .Where(p => (p.Description ?? "").ToLower().Contains(pdescFilter.ToLower()));
                }
                QueryTotalAmount = (uint)await query.CountAsync();
                _products = await query.Take(ItemLimit).ToListAsync();
                QueryFilterAmount = (uint)_products.Count;
                await ctx.DisposeAsync();
            });

            OnPropertyChanged(nameof(Products));
#endif
            this.datagrid_main.EndInit();
        }

        // Update method
        public async Task UpdateProductAsync(Models.Product.IcProductCatalog product)
        {
#if NEWDBSQL
            using var context = new Models.Product.ProductDbCtx();
            var dbProduct = await context.IcProductCatalogs.FindAsync(product.ProductId);
            if (dbProduct is null) return;
            // Update the tracked entity
            context.Entry(dbProduct).CurrentValues.SetValues(product);
            await context.SaveChangesAsync();

            // Reload collection
            await LoadProductsAsync();
#endif
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this._products = [];
        }
#endregion

        #region "EventHandlers"
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ProductsViewSource_Filter(object sender, FilterEventArgs e)
        {

        }
        #endregion


        private async void Txt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is not Key.Enter) return;
            if (sender is not TextBox txt) return;
            btn_reload_Click(sender, null);
        }

        private void btn_reload_Click(object sender, RoutedEventArgs e)
        {
            this.LoadProducts(this.Txt_ProdCode.Text, this.Txt_ProdDesc.Text);
        }

        private async void datagrid_main_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.datagrid_main.SelectedIndex is -1) return;
            if (this.datagrid_main.SelectedItem is not Models.Product.IcProductCatalog prod) return;
            this.MainViewModel.CurrentPage = PageTypes.ProductCatalogDetails;
            await this.MainViewModel.ProductCatalogDetails_VM.LoadProductData(prod.ProductCode);
        }
    }
}
