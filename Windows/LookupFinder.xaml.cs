using Microsoft.EntityFrameworkCore;
using OMPS.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Odbc;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SCH = SQL_And_Config_Handler;

namespace OMPS.Windows
{
    /// <summary>
    /// Interaction logic for LookupFinder.xaml
    /// </summary>
    public partial class LookupFinder : Window, INotifyPropertyChanged, IDisposable
    {
        public LookupFinder_ViewModel ViewModel
        {
            get => (LookupFinder_ViewModel)DataContext;
        }

        public LookupFinder()
        {
            this.DataContext = new LookupFinder_ViewModel(this);
            InitializeComponent();
            //
            //this.LookupMaterials("%PLW%");
        }

        public async Task LookupItem(string? partFilter = null, string? descFilter = null, string defaultPartContraint = "%", string defaultDescConstraint = "%")
        {
            ViewModel.LastLookup = this.LookupItem;
            await Lookup(
                partFilter, descFilter,
                defaultPartContraint, defaultDescConstraint
             );
        }

        public async Task LookupMaterials(string? partFilter = null, string? descFilter = null, string defaultPartContraint = "%", string defaultDescConstraint = "%")
        {
            ViewModel.LastLookup = this.LookupMaterials;
            await Lookup(
                partFilter, descFilter,
                defaultPartContraint, defaultDescConstraint
            );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async Task Lookup(string? partFilter = null, string? descFilter = null, string defaultPartContraint = "%", string defaultDescConstraint = "%", ushort limit = 25)
        {
            if (ViewModel is null) return;
            ViewModel.LastLookupArgs = [
                partFilter ?? "", descFilter ?? "",
                defaultPartContraint, defaultDescConstraint
            ];
            this.Txt_FilterPart.Text = partFilter ?? "";
            this.Txt_FilterDesc.Text = descFilter ?? "";
            using var ctx = new DBModels.Product.ProductDbCtx();
            var query = ctx.IcItems
                .Where(p =>
                    EF.Functions.Like(p.Item, defaultPartContraint) &&
                    EF.Functions.Like(p.Item, (partFilter == null ? "%" : $"%{partFilter}%")) &&
                    EF.Functions.Like(p.Description, defaultDescConstraint) &&
                    EF.Functions.Like(p.Description, (descFilter == null ? "%" : $"%{descFilter}%"))
                );
            if (ViewModel.Total is -1)
            {
                ViewModel.Total = await query.CountAsync();
            }
            ViewModel._items.Clear();
            var res = await query
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();
            if (res is null) return;
            for (int i = 0; i < res.Count; i++)
            {
                ViewModel._items.Add(res[i]);
            }
            OnPropertyChanged("Items");
            //this.DataGrid_Lookup.ItemsSource = Data;
            //this.DataGrid_AutoGeneratingColumn(this.DataGrid_Lookup, null);
            this.Lbl_datagrid_count.Content = $"Showing {(res.Count < limit ? res.Count: limit)} of {ViewModel.Total} items";
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = true;
            return;
            if (e.Item is not DBModels.Product.IcItem item) return;
            var passed = true;
            if (!string.IsNullOrWhiteSpace(this.Txt_FilterPart.Text))
                passed &= (item.Item?.ToString()?.Contains(this.Txt_FilterPart.Text, StringComparison.OrdinalIgnoreCase) ?? false);
            if (!string.IsNullOrWhiteSpace(this.Txt_FilterDesc.Text))
            passed &= (item.Description?.ToString()?.Contains(this.Txt_FilterDesc.Text, StringComparison.OrdinalIgnoreCase) ?? false);
            e.Accepted = passed;
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs? e)
        {
            //Debug.WriteLine("$$$");
            //Debug.WriteLine(1);
            if (sender is not DataGrid dg) return;
            if (e is null || e?.Column.Header is not string header) return;

            //Debug.WriteLine(3);
            //Debug.WriteLine(4);
            var colWidth = header switch
            {
                "Item" => new DataGridLength(2, DataGridLengthUnitType.Star),
                _ => new DataGridLength(5, DataGridLengthUnitType.Star)
            };
            e.Column.Width = colWidth;
            e.Column.Visibility = (header is "Item" or "Description" ? Visibility.Visible : Visibility.Collapsed);
        }

        private void Txt_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key is not System.Windows.Input.Key.Enter) return;
            /*
            var viewSource = (CollectionViewSource)Resources["DataViewSource"];
            viewSource?.View?.Refresh();
            */
            ViewModel.LastLookup?.DynamicInvoke(
                this.Txt_FilterPart.Text, this.Txt_FilterDesc.Text,
                ViewModel.LastLookupArgs[2], ViewModel.LastLookupArgs[3]
            );
        }

        private void DataGrid_Lookup_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.DataGrid_Lookup.SelectedItem is null) return;
            if (this.DataGrid_Lookup.SelectedItem is not object item) return;
            ViewModel.ReturnObject = item;
            this.DialogResult = true;
        }

        private void LookupFinderWindow_Closing(object sender, CancelEventArgs e)
        {
            if (ViewModel.ReturnObject is not null) return;
            this.DialogResult = false;
        }

        private void DataGrid_Lookup_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Padding = new Thickness(0);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class LookupFinder_ViewModel: ViewModelBase
    {
        public LookupFinder _lookupFinder;
        public static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }
        internal static MainWindow ParentWindow { get => Ext.MainWindow; }
        internal ObservableCollection<DBModels.Product.IcItem> _items = [];
        public IReadOnlyCollection<DBModels.Product.IcItem> Items => this._items;

        public object? ReturnObject = null;

        public int Total = -1;
        public string[] LastLookupArgs = [];
        public Func<string, string, string, string, Task>? LastLookup = null;

        public LookupFinder_ViewModel(LookupFinder view)
        {
            this._lookupFinder = view;
        }
    }

}
