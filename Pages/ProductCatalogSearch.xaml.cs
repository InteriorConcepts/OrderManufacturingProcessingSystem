using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyApp.DataAccess.Generated;
using OMPS.viewModel;
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
using OMPS.Models;

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

        private List<Models.IcProductCatalog> _products = [];
        public IReadOnlyList<Models.IcProductCatalog> Products => _products;


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

        public async void LoadProducts()
        {

            this.ProgressBar_Show();

            await this.LoadProductsAsync();

            this.ProgressBar_Hide();
        }

        // Query method
        public async Task LoadProductsAsync()
        {
            this.datagrid_main.BeginInit();
            await Task.Run(async () =>
            {
                using var context = new IcEmqContext();

                // Safe read-only query
                _products = await context.IcProductCatalogs
                    .Where(p => p.Status == "A")
                    .OrderBy(p => p.ProductCode)
                    .Take(25)
                    .Select(p => new Models.IcProductCatalog
                    {
                        ProductId = p.ProductId,
                        ProductCode = p.ProductCode,
                        Description = p.Description
                    })
                    .AsNoTracking() // No change tracking
                    .AsSplitQuery()
                    .ToListAsync();
            });

            OnPropertyChanged(nameof(Products));
            this.datagrid_main.EndInit();
        }

        // Update method
        public async Task UpdateProductAsync(Models.IcProductCatalog product)
        {
            using var context = new IcEmqContext();
            var dbProduct = await context.IcProductCatalogs.FindAsync(product.ProductId);
            if (dbProduct is null) return;
            // Update the tracked entity
            context.Entry(dbProduct).CurrentValues.SetValues(product);
            await context.SaveChangesAsync();

            // Reload collection
            await LoadProductsAsync();
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

        private void Txt_ProdCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void btn_reload_Click(object sender, RoutedEventArgs e)
        {
            this.LoadProducts();
        }

        private void datagrid_main_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.datagrid_main.SelectedIndex is -1) return;
            if (this.datagrid_main.SelectedItem is not Models.IcProductCatalog prod) return;
            this.MainViewModel.CurrentPage = PageTypes.ProductCatalogDetails;
            this.MainViewModel.ProductCatalogDetails_VM.LoadProductData(prod.ProductCode);
        }
    }
}
