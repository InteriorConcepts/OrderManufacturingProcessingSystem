using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
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

using MaterialDesignThemes.Wpf;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using OMPS.DBModels;
using OMPS.DBModels.Order;
using OMPS.ViewModels;
using OMPS.Windows;

using static OMPS.Ext;

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
            this.DataGridView_TabChanged += ProductCatalogDetails_DataGridView_TabChanged;
        }

        private async void ProductCatalogDetails_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not string propName) return;
        }


        #region "Events"
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<string>? DataGridView_TabChanged;
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

        public bool ProductInfo_HasChanges { get => this.ProductInfo_Changes.Count is not 0; }
        public DBModels.Product.IcProductCatalog ProductInfo { get; set; } = new();
        public Dictionary<string, object> ProductInfo_Changes
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(ProductInfo_Changes));
                OnPropertyChanged(nameof(ProductInfo_HasChanges));
            }
        } = [];


        public List<DBModels.Product.IcProdBom> _prodBoms = [];
        public IReadOnlyCollection<DBModels.Product.IcProdBom> ProdBoms => this._prodBoms;

        public List<DBModels.Product.IcMfgBom> _mfgItems = [];
        public IReadOnlyCollection<DBModels.Product.IcMfgBom> MfgItems => this._mfgItems;

        public DBModels.Product.IcProdBom? CurrentProdBom
        {
            get => field;
            set
            {
                if (value is null)
                {
                    field = null;
                    return;
                }
                if (field == value) return;
                var i = new DBModels.Product.IcProdBom();
                i.CopyPropertiesFrom(value);
                field = i;
                OnPropertyChanged(nameof(CurrentProdBom));
            }
        }
        public DBModels.Product.IcMfgBom? CurrentMfgBom
        {
            get => field;
            set
            {
                if (value is null)
                {
                    field = null;
                    return;
                }
                if (field == value) return;
                var i = new DBModels.Product.IcMfgBom();
                i.CopyPropertiesFrom(value);
                field = i;
                OnPropertyChanged(nameof(CurrentProdBom));
            }
        }

        public static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }
        internal static MainWindow ParentWindow { get => Ext.MainWindow; }
        internal static double DataGridFontSize { get => Ext.MainViewModel.FontSize_Base; }



        public string[] AssembledValues { get; } = ["yes", "no"];
        public string[] Finishes_Default { get; } = ["NA", "CH", "DB", "GY", "PL", "TP"];
        public string[] ColorByValues { get; } = ["NA", "AccFin", "Aveera-TopFin", "FrmFin", "PmoldFin", "PedFin", "ShelfFin", "TangentLegFin", "TblBaseFin", "TblLegFin", "TblLegBaseFin", "TblLegExtraFin", "TblSlimLegFin", "Chase", "Fab", "FabChase", "FabLam", "FabMel", "Acrylic", "AcrylicFab", "AcrylicLam", "AcrylicMel", "Lam", "LamChase", "Mel", "MelChase", "MelLam", "WSEfin"];
        public string[] TypeValues { get; } = ["Acc", "Accessory", "Acclaim", "Assy", "Aveera Collection", "Catch-All", "CC", "Ch", "Dtr", "Ergo", "Ergonomic Accessories", "Fab", "Frame", "Jnt", "Manuf", "Matl", "Met", "Metal Products", "Ped", "Pedestal", "Pmd", "Pnl", "Pre", "Project Board", "Seating", "She", "Shelf", "Soft Seating", "Special", "Storage Cabinet", "Symmetry", "Table", "Tableo", "Tbl", "Til", "Tile", "Tub", "Type", "WBB", "Wks", "Wsta", "WTil"];
        public string[] SubTypeValues { get; } = ["Acc", "Accessory", "Accessory-Pnl", "Accessory-She", "Adj Ht Base", "Arch-Leg", "Arch-leg Flip-top", "Arch-leg Flip-top Pin-clip", "Arch-leg Pin-clip", "Arch-leg X-base", "Arch-leg X-base Flip-top", "aStatic", "BOM", "Book Case", "Bookcase", "BORCO", "Cabinet", "Cart", "C-Bal", "C-Balance", "CC", "CCC", "CCL", "CCLC", "CCS", "CCSC", "Center Drawer", "Chase Panel Powered", "Clamp Mount", "Collaboration", "Conference Chair", "Configurable", "Connector", "CPU", "CPU Holder", "Crank2", "Crank3", "Cushion", "Deck", "DK", "Door", "Dot", "Down Under Unit", "Drafting", "Dtr", "edge", "Electrical", "Executive Chair", "Fastener", "File Center", "Fixed Pedestal", "Footrest", "Frame", "Freestanding", "GLASS", "Glass Tile", "Guest Chair", "Hanging Pedestal", "HCDoor", "H-leg", "Insert", "Jnt", "Keyboard", "Keyboard Platform", "Lateral", "Lateral File", "Lateral File-Open Shelves", "Lateral File-Storage Cab", "Leg", "Leg Arch", "Leg Arch X", "Leg H", "Leg Post", "Leg T", "Leg T X", "LegPost", "Lighting", "Locker", "Lounge", "Matl", "Media Cabinet", "MFG", "Mi", "Misc", "Mobile Pedestal", "Monitor", "Monitor Arm", "Motion", "Note", "OMD", "Open", "Open Front", "Option", "Ottoman", "Paper Management", "PaperManagement", "Ped", "Pin-clip", "Pmd", "Pnl", "Post Leg", "Post-leg", "Post-leg Pin-clip", "Project Board", "Reach", "Recon Part", "ReconPart", "Shelf", "Slim Post-leg Pin-clip Ht Adj 24-34", "Special", "SST", "Stackable Multi-Purpose", "Stick", "Stool Chair", "Storage", "Storage Cabinet", "SubType", "Table", "Tangent", "Task Chair", "TBL", "Teacher Desk", "Technology Ed Furniture", "Tedge", "T-edge", "TIL", "Tile", "T-leg", "T-leg Flip-top", "T-leg Flip-top Pin-clip", "T-leg Pin-clip", "T-leg X-base", "Tote", "Tote Storage", "TRESPA", "Trough", "Tub", "tun", "Unique Shape", "Vertical File", "Wardrobe", "WBT", "WCA", "WCF", "WCL", "WCS", "WDR", "WHR", "Wire Management", "WireManagement", "wkd", "wks", "WksDeckShelf", "WKSEDGE", "WKSP", "WksShelvesDecks", "worksurcae", "Worksurface", "WRD", "wrks", "WRT", "WS12", "WS89", "WSA", "WSB", "WSB12", "WSB9", "WSC", "WSD", "WSE", "WSEdge", "WSF", "WSL", "WSP", "WSR", "WstaClusterCoreRing", "WstaClusterCoreRings", "WSVEM", "WVCAL", "WVCAR", "WVCCL", "WVCCR", "WVCEM", "WVCFM", "WVCPM", "WVCQL", "WVCQR", "WVCRL", "WVCRR", "WVCTM", "WVSAM", "WVSEM", "WVSRL", "WVSRR", "WVVEM", "WVVFM", "WVVPM", "WVVQL", "WVVQR", "WVVRL", "WVVRR", "WVVTM", "WXA", "WXC"];

        public string[] DeptValues { get; } = ["AssyCut", "AssyPick", "Ship", "WoodCut", "WoodPick"];
        public string[] WorkCtrValues { get; } = [
            "300-00",
            "300-01",
            "300-02",
            "300-03",
            "300-04",
            "300-05",
            "300-06",
            "300-07",
            "310-00",
            "310-01",
            "310-02",
            "310-03",
            "310-04",
            "320-00",
            "330-00",
            "330-01",
            "330-02",
            "340-00",
            "340-01",
            "360-00",
            "600-00",
            "600-01",
            "600-02",
            "600-03",
            "600-04",
            "600-05",
            "610-00",
            "620-00",
            "620-01",
            "620-02",
            "850-00",
            "850-01",
            "OS",
        ];
        public string[] Finishes_BS { get; } = ["NA", "SL", "BK"];
        public string[] Finishes_Omt { get; } = ["NA", "BK", "BL", "GN", "RD", "SL", "WH"];
        public string[] Finishes_SlimLeg { get; } = ["NA", "CH", "PL"];
        public string[] Finishes_TangLeg { get; } = ["NA", "Tang-Coral", "Tang-DarkBlue", "Tang-Lime", "Tang-Silver", "Tang-White"];
        public string[] Finishes_PedCush { get; } = ["NA", "New Hempstead NH333 Black", "New Hempstead NH359 Azure", "New Hempstead NH361 Nickel", "New Hempstead NH366 Steel", "New Hempstead NH369 Navy", "New Hempstead NH389 Red Red W", "New Hempstead NH395 Grey", "New Hempstead NH406 Fire", "New Hempstead NH419 Aubergine", "New Hempstead NH420 Jodhpurs", "New Hempstead NH422 Napa", "New Hempstead NH424 Cocoa", "New Hempstead NH425 Zen", "New Hempstead NH434 Brick", "New Hempstead NH509 Galaxy", "Sprint Abyss 64038", "Sprint Blackberry 64036", "Sprint Blaze 64029", "Sprint Breeze 64028", "Sprint Cherry 64034", "Sprint Cordovan 64035", "Sprint Driftwood 64033", "Sprint Fern 64027", "Sprint Gold 64024", "Sprint Graphite 64026", "Sprint Ivory 64021", "Sprint Nocturne 640310", "Sprint Peat 64037", "Sprint Quarry 64022", "Sprint Storm 64032"];




        public string UnsavedChangesPrompt
        {
            get => $"Unsaved changes ({Pending_LineChangesCount}), Grid is locked";
        }
        public ObservableCollection<string> Pending_LineChanges
        {
            get => field;
            set
            {
                if (field == value) return;
                if (field == null) return;
                field = value;
            }
        } = [];
        public bool IsPending_LineChanges
        {
            get => Pending_LineChangesCount is not 0;
        }
        public ushort Pending_LineChangesCount
        {
            get => (ushort)Pending_LineChanges.Count;
        }
        public bool NoPending_LineChanges { get => Pending_LineChangesCount is 0; }

        #endregion


        #region "Methods"
        public async Task LoadAllProductData()
        {
            isLoaded = true;
            this.ProgressBar_Show();

            await LoadProductInfo();
            await LoadDataGridItems();

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


        public enum DataGridViews_ProductCatalog
        {
            ProdBom,
            MfgBom
        }
        public DataGridViews_ProductCatalog CurrentDataGridView = DataGridViews_ProductCatalog.ProdBom;

        public async Task LoadDataGridItems()
        {
            switch (CurrentDataGridView)
            {
                case DataGridViews_ProductCatalog.ProdBom:
                    await this.LoadProductBomItems();
                    break;
                case DataGridViews_ProductCatalog.MfgBom:
                    await this.LoadProductMfgItems();
                    break;
                default:
                    break;
            }
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
            this.pnl_ProdBoms_dock.Visibility =
                this.Btn_CollapseSideGrid.IsChecked ?? true ?
                Visibility.Visible :
                Visibility.Collapsed;
            Grid.SetColumnSpan(this.pnl_ProdBoms_dock, (this.pnl_ProdBoms_dock.Visibility is Visibility.Collapsed ? 2 : 1));
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RevertEditControls()
        {
            foreach (Label item in this.WPnl_ProdBoms_EditLabels.Children)
            {
                item.Tag = null;
            }
            foreach (Control item in this.WPnl_ProdBoms_EditInputs.Children)
            {
                var dpinfo = Ext.DpValueFromInputType(item);
                if (dpinfo is null) continue;
                item.GetBindingExpression(dpinfo.Value.dp)?.UpdateTarget();
            }
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
            if (sender is not DBModels.Product.IcProductCatalog obj) return;
            if (obj.GetType().GetProperty(propName) is not PropertyInfo propInfo) return;
            if (propInfo.GetValue(obj) is not object val) return;
            this.ProductInfo_Changes[propName] = val;
            OnPropertyChanged(nameof(ProductInfo_HasChanges));
            Debug.WriteLine($"{propName} -> {val}");
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

        private void ProductCatalogDetails_DataGridView_TabChanged(object? sender, string e)
        {

        }

        private void RadioButton_DataGridView_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is null || sender is not RadioButton rdiobtn || rdiobtn.Tag is not string tag) return;
            this.DataGridView_TabChanged?.Invoke(this, tag);
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

        private void Btn_RevertItemLinePropEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_AcceptItemLineEdits_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_DeleteItemLine_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_NewItemLine_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_RevertItemLineEdits_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void Btn_rowEdit_Lookup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_ProdBoms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.WPnl_ProdBoms_EditInputs.BindingGroup.BeginEdit();
            if (this.DataGrid_ProdBoms.SelectedItem is DBModels.Product.IcProdBom item)
            {
                CurrentProdBom = item;
            }
            RevertEditControls();
        }
    }
}
