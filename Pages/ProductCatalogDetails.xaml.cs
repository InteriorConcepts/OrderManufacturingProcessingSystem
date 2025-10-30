﻿using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for ProductCatalogDetails.xaml
    /// </summary>
    public partial class ProductCatalogDetails : UserControl, INotifyPropertyChanged
    {
        public ProductCatalogDetails(MainWindow parentWindow)
        {
            this.ParentWindow = parentWindow;
            InitializeComponent();
            this.PropertyChanged += ProductCatalogDetails_PropertyChanged;
        }

        private async void ProductCatalogDetails_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not string propName) return;
            if (propName is "ProductCode")
            {
                await this.LoadProductData();
            }
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
            }
        } = "";
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
        public async Task LoadProductData()
        {
            await Task.Run(() =>
            {
                var res_items = Ext.Queries.GetProductSubPartsByProductCode(this.ProductCode);
                if (res_items is null) return;
                if (res_items.Count is 0) return;
            });
        }
        #endregion


        #region "EventHandlers"
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void Btn_RefreshHeader_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_SaveHeader_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_CollapseTopBar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_CollapseSideGrid_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
