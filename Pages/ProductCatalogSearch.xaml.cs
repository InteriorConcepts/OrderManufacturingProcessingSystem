using MaterialDesignThemes.Wpf;
using MyApp.DataAccess.Generated;
using OMPS.viewModel;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for ProductCatalog.xaml
    /// </summary>
    public partial class ProductCatalogSearch : UserControl, INotifyPropertyChanged, IDisposable
    {
        public ProductCatalogSearch()
        {

            InitializeComponent();
            //
            this.DataContext = this;

            
            var res = Ext.Queries.GetProducts();
            foreach (var item in res)
            {
                this.ProductInfos.Add(item);
            }
        }

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region Properties
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
        public ObservableCollection<example_queries_GetProductsResult> ProductInfos { get; set; } = [];
        #endregion

        #region "Methods"


        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.ProductInfos.Clear();
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
    }
}
