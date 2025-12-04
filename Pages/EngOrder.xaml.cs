using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OMPS.Components;
using OMPS.DBModels;
using OMPS.DBModels.Order;
using OMPS.DBModels.Product;
using OMPS.ViewModels;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using static OMPS.Ext;
using SCH = SQL_And_Config_Handler;

namespace OMPS.Pages
{
    /// <summary>
    /// Interaction logic for EngOrder.xaml
    /// </summary>
    public partial class EngOrder : UserControl, INotifyPropertyChanged
    {
        public EngOrder()
        {
            InitializeComponent();
            //
            this.DataContext = this;
            this.dpnl_DataFilter.Visibility = Visibility.Collapsed;
            //this.FrmFin.ItemSource = Finishes_Default;
            this.JobNbrChanged += this.EngOrder_JobNbrChanged;
            this.PropertyChanged += this.EngOrder_PropertyChanged;

            this.EngOrder_LoookupButtons = [.. this.SPnl_LookupButtons.Children.OfType<Button>().Concat(this.SPnl_LookupButtonsExtra.Children.OfType<Button>())];
            this.EngOrder_LookupInputs = [.. this.SPnl_LookupInputs.Children.OfType<TextBox>().Concat(this.SPnl_LookupInputsExtra.Children.OfType<TextBox>())];

            foreach (Control cntrl in SPnl_LookupInputs.Children)
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(TextBox.TagProperty, typeof(TextBox));
                dpd.AddValueChanged(cntrl as TextBox, LabelInputLookupPair_TagChanged);
            }

            Ext.EngOrder_Filters?.PropertyChanged += ((object? sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName is not "filters") return;
                this.WPnl_SavedFilters.Children.Clear();
                foreach (var filter in Ext.EngOrder_Filters.filters ?? [])
                {
                    var btn = new Button()
                    {
                        Style = FindResource("MaterialDesignPaperButton") as Style,
                        Tag = filter.value,
                        Content = filter.name,
                        Padding = new(4, 1, 4, 1),
                        Margin = new(0, 0, 3, 1),
                    };
                    btn.SetBinding(
                        Button.FontSizeProperty,
                        new Binding("MainViewModel.FontSize_H5") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
                    );
                    btn.Click += this.Btn_SavedFilters_Click;
                    this.WPnl_SavedFilters.Children.Add(btn);
                }
            });
            Ext.EngOrder_Filters?.filters = Ext.EngOrder_Filters.filters;
        }

        private void ColorSetInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (this.IsLoadingJobData) return;
            if (e.PropertyName is not string propName) return;
            //MessageBox.Show(propName);
            if (sender is not DBModels.Order.AIcColorSet obj) return;
            if (obj.GetType().GetProperty(propName) is not PropertyInfo propInfo) return;
            if (propInfo.GetValue(obj) is not object val) return;
            this.ColorSetInfo_Changes[propName] = val;
            OnPropertyChanged(nameof(ColorSetInfo_HasChanges));
            Debug.WriteLine($"{propName} -> {val}");
        }

        private void EngOrder_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not string propName) return;
            if (propName is "ColorSetInfo")
            {
                this.ColorSetInfo_PropertyChanged(sender, e);
                OnPropertyChanged(nameof(ColorSetInfo_HasChanges));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #region Events
        public event EventHandler<string> JobNbrChanged;
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion


        #region Properties
        public DBModels.Order.AIcManuf? CurrentItem
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
                var i = new AIcManuf();
                i.CopyPropertiesFrom(value);
                field = i;
                OnPropertyChanged(nameof(CurrentItem));
            }
        }
        public static double DataGridFontSize { get => Ext.MainViewModel.FontSize_Base; }
        public static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }

        public const short DELAY_HEADER_REFRESH = 10000;

        public Dictionary<string, bool> ExternalProc_GroupActive = new()
        {
            { "stdEngProcs", false }
        };

        public bool IsLoadingJobData { get; set; } = false;
        private DateTime? Last_ManufData = null;

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
        public ushort Pending_LineChangesCount {
            get => (ushort)Pending_LineChanges.Count;
        }
        public bool NoPending_LineChanges { get => Pending_LineChangesCount is 0; }


        internal DataGrid? CurrentGrid { get; set; }
        public Dictionary<string, string[]> ItemLineFilers { get; set; } = [];
        public string[] Finishes_Default { get; } = ["NA", "CH", "DB", "GY", "PL", "TP"];
        public string[] ColorByValues { get; } = ["NA", "AccFin", "Aveera-TopFin", "FrmFin", "PmoldFin", "PedFin", "ShelfFin", "TangentLegFin", "TblBaseFin", "TblLegFin", "TblLegBaseFin", "TblLegExtraFin", "TblSlimLegFin", "Chase", "Fab", "FabChase", "FabLam", "FabMel", "Acrylic", "AcrylicFab", "AcrylicLam", "AcrylicMel", "Lam", "LamChase", "Mel", "MelChase", "MelLam", "WSEfin"];
        public string[] TypeValues { get; } = ["Acc", "Accessory", "Acclaim", "Assy", "Aveera Collection", "Catch-All", "CC", "Ch", "Dtr", "Ergo", "Ergonomic Accessories", "Fab", "Frame", "Jnt", "Manuf", "Matl", "Met", "Metal Products", "Ped", "Pedestal", "Pmd", "Pnl", "Pre", "Project Board", "Seating", "She", "Shelf", "Soft Seating", "Special", "Storage Cabinet", "Symmetry", "Table", "Tableo", "Tbl", "Til", "Tile", "Tub", "Type", "WBB", "Wks", "Wsta", "WTil"];
        public string[] SubTypeValues { get; } = ["Acc", "Accessory", "Accessory-Pnl", "Accessory-She", "Adj Ht Base", "Arch-Leg", "Arch-leg Flip-top", "Arch-leg Flip-top Pin-clip", "Arch-leg Pin-clip", "Arch-leg X-base", "Arch-leg X-base Flip-top", "aStatic", "BOM", "Book Case", "Bookcase", "BORCO", "Cabinet", "Cart", "C-Bal", "C-Balance", "CC", "CCC", "CCL", "CCLC", "CCS", "CCSC", "Center Drawer", "Chase Panel Powered", "Clamp Mount", "Collaboration", "Conference Chair", "Configurable", "Connector", "CPU", "CPU Holder", "Crank2", "Crank3", "Cushion", "Deck", "DK", "Door", "Dot", "Down Under Unit", "Drafting", "Dtr", "edge", "Electrical", "Executive Chair", "Fastener", "File Center", "Fixed Pedestal", "Footrest", "Frame", "Freestanding", "GLASS", "Glass Tile", "Guest Chair", "Hanging Pedestal", "HCDoor", "H-leg", "Insert", "Jnt", "Keyboard", "Keyboard Platform", "Lateral", "Lateral File", "Lateral File-Open Shelves", "Lateral File-Storage Cab", "Leg", "Leg Arch", "Leg Arch X", "Leg H", "Leg Post", "Leg T", "Leg T X", "LegPost", "Lighting", "Locker", "Lounge", "Matl", "Media Cabinet", "MFG", "Mi", "Misc", "Mobile Pedestal", "Monitor", "Monitor Arm", "Motion", "Note", "OMD", "Open", "Open Front", "Option", "Ottoman", "Paper Management", "PaperManagement", "Ped", "Pin-clip", "Pmd", "Pnl", "Post Leg", "Post-leg", "Post-leg Pin-clip", "Project Board", "Reach", "Recon Part", "ReconPart", "Shelf", "Slim Post-leg Pin-clip Ht Adj 24-34", "Special", "SST", "Stackable Multi-Purpose", "Stick", "Stool Chair", "Storage", "Storage Cabinet", "SubType", "Table", "Tangent", "Task Chair", "TBL", "Teacher Desk", "Technology Ed Furniture", "Tedge", "T-edge", "TIL", "Tile", "T-leg", "T-leg Flip-top", "T-leg Flip-top Pin-clip", "T-leg Pin-clip", "T-leg X-base", "Tote", "Tote Storage", "TRESPA", "Trough", "Tub", "tun", "Unique Shape", "Vertical File", "Wardrobe", "WBT", "WCA", "WCF", "WCL", "WCS", "WDR", "WHR", "Wire Management", "WireManagement", "wkd", "wks", "WksDeckShelf", "WKSEDGE", "WKSP", "WksShelvesDecks", "worksurcae", "Worksurface", "WRD", "wrks", "WRT", "WS12", "WS89", "WSA", "WSB", "WSB12", "WSB9", "WSC", "WSD", "WSE", "WSEdge", "WSF", "WSL", "WSP", "WSR", "WstaClusterCoreRing", "WstaClusterCoreRings", "WSVEM", "WVCAL", "WVCAR", "WVCCL", "WVCCR", "WVCEM", "WVCFM", "WVCPM", "WVCQL", "WVCQR", "WVCRL", "WVCRR", "WVCTM", "WVSAM", "WVSEM", "WVSRL", "WVSRR", "WVVEM", "WVVFM", "WVVPM", "WVVQL", "WVVQR", "WVVRL", "WVVRR", "WVVTM", "WXA", "WXC"];

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


        private string? _jobNbr;
        public string? JobNbr
        {
            get => this._jobNbr;
            set
            {
                if (value is null or "") return;
                Ext.FormatJobNum(ref value);
                if (!Ext.IsJobNumValid(value)) return;
                if (EqualityComparer<string?>.Default.Equals(this._jobNbr, value.ToUpper())) return;
                this.Last_ManufData = null;
                this._jobNbr = value.ToUpper();
                this.JobNbrChanged?.Invoke(this, this._jobNbr);
            }
        }

        public bool ColorSetInfo_HasChanges { get => this.ColorSetInfo_Changes.Count is not 0; }
        public AIcColorSet ColorSetInfo { get; set; } = new();
        public Dictionary<string, object> ColorSetInfo_Changes
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(ColorSetInfo_Changes));
                OnPropertyChanged(nameof(ColorSetInfo_HasChanges));
            }
        } = [];
        public List<DBModels.Order.AIcManuf> _mfgItemLines = [];
        public IReadOnlyCollection<DBModels.Order.AIcManuf> MfgItemLines => this._mfgItemLines;
        #endregion


        #region Fields

        public int RowSpan = 1;
        #endregion


        #region Methods
        public async Task LoadDataForJob(string job)
        {
            string tab = "";
            try
            {
                this.LastCell = null;
                this.Pending_LineChanges = [];
                this.Pending_LineChanges.CollectionChanged += this.Pending_LineChanges_CollectionChanged;
                this.IsLoadingJobData = true;
                this.CurrentGrid?.Visibility = Visibility.Collapsed;
                await this.LoadColorSetData(job);
                if (this.RadioBtn_View_QPO.IsChecked is true)
                {
                    tab = this.RadioBtn_View_QPO.Tag.ToString() ?? "";
                    //this.CurrentGrid = this.datagrid_QPO;
                    await this.LoadQPartsOrdered(job);
                }
                else if (this.RadioBtn_View_QDO.IsChecked is true)
                {
                    tab = this.RadioBtn_View_QDO.Tag.ToString() ?? "";
                    //this.CurrentGrid = this.datagrid_QIO;
                    await this.LoadQItemsOrdered(job);
                }
                else if (this.RadioBtn_View_M.IsChecked is true)
                {
                    tab = this.RadioBtn_View_M.Tag.ToString() ?? "";
                    this.CurrentGrid = this.datagrid_main;
                    await this.LoadManufData(job);
                }
                else if (this.RadioBtn_View_MP.IsChecked is true)
                {
                    tab = this.RadioBtn_View_MP.Tag.ToString() ?? "";
                    //this.CurrentGrid = this.datagrid_MP;
                    await this.LoadManufParts(job);
                }
                this.CurrentGrid?.Visibility = Visibility.Visible;
                Ext.MainViewModel?.SetUrlRelPath($"?job={job}&tab={tab}");
            }
            catch (Exception ex)
            {
                Ext.MainWindow.MainToastContainer.CreateToast("Eng Order", $"Error loading data for job:\n{ex.Message}\n{ex.StackTrace}", FeedbackToast.IconTypes.Error);
            }
            IsLoadingJobData = false;
        }

        public async Task LoadColorSetData(string job)
        {
            IsLoadingJobData = true;
            await Task.Run(async () =>
            {
                Debug.WriteLine("Loading Color Set Data");
                using (var ctx = new OrderDbCtx())
                {
                    var res = await ctx.AIcColorSets
                        .Where(c => c.SupplyOrderRef == job)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (res is null) return;
                    this.ColorSetInfo = res;
                    this.ColorSetInfo.PropertyChanged += this.ColorSetInfo_PropertyChanged;
                    OnPropertyChanged(nameof(ColorSetInfo));
                }
            });
        }

        public async Task LoadQPartsOrdered(string job)
        {
        }

        public async Task LoadQItemsOrdered(string job)
        {
        }

        public async Task LoadManufData(string job)
        {
            Debug.WriteLine("Loading Manuf Data");
            this.progbar_itemlines.Value = 50;
            this.progbar_itemlines.IsEnabled = true;
            this.progbar_itemlines.Visibility = Visibility.Visible;
            this.datagrid_main.BeginInit();
            using (var ctx = new DBModels.Order.OrderDbCtx())
            {
                this._mfgItemLines = await ctx.AIcManufs
                    .Where(p => p.JobNbr == job)
                    .OrderBy(p => p.ManufId)
                    .AsNoTracking() // No change tracking
                    .AsSingleQuery()
                    .ToListAsync();
            }
            this.datagrid_main.EndInit();
            this.progbar_itemlines.Value = 0;
            this.progbar_itemlines.IsEnabled = false;
            this.progbar_itemlines.Visibility = Visibility.Collapsed;
            if (this.datagrid_main.Items.Count is not 0)
            {
                this.datagrid_main.ScrollIntoView(this.datagrid_main.Items[0]);
            }
            OnPropertyChanged(nameof(this.MfgItemLines));
            Ext.MainWindow.MainToastContainer.CreateToast("Eng Order", "Item line data loaded").Show();
            this.Last_ManufData = DateTime.Now;
        }

        public async Task LoadManufParts(string job)
        {
        }

        public void ToggleSideGrid()
        {
            this.pnl_dock.Visibility =
                this.pnl_dock.Visibility is Visibility.Collapsed ?
                Visibility.Visible :
                Visibility.Collapsed;
            this.RowSpan = (this.pnl_dock.Visibility is Visibility.Collapsed ? 2 : 1);
            Grid.SetColumnSpan(datagrid_main, RowSpan);
            this.Btn_CollapseSideGrid.IsChecked = this.pnl_dock.Visibility is Visibility.Visible;
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

        private void DataGridMouseWheelHorizontal(object sender, RoutedEventArgs e)
        {
            /*
            MouseWheelEventArgs eargs = (MouseWheelEventArgs)e;
            double x = (double)eargs.Delta;
            double y = dataSideGridScrollViewer.VerticalOffset;
            dataSideGridScrollViewer.ScrollToVerticalOffset(y - x);
            */
        }


        public enum PropUpdateResult
        {
            Worked,
            Error,
            ConversionFailed,
            NoPropOrCantWrite,
            SameValue
        }

        public void ToggleFiltersPanel()
        {
            Debug.WriteLine("Toggle Filters");
            if (this.dpnl_DataFilter.Visibility is Visibility.Collapsed)
            {
                this.dpnl_DataFilter.Visibility = Visibility.Visible;
                this.Btn_ToggleFiltersPnl.IsChecked = true;
                this.Txt_Filter.Focus();
            }
            else
            {
                this.dpnl_DataFilter.Visibility = Visibility.Collapsed;
                this.Btn_ToggleFiltersPnl.IsChecked = false;
                this.CurrentGrid?.Focus();
            }
        }

        public void ResetUI()
        {
            this.Btn_ExtProc_ImpMfg.BorderBrush = 
                this.Btn_ExtProc_EngPrc.BorderBrush =
                this.Btn_ExtProc_Matl.BorderBrush = 
                this.Btn_ExtProc_Cutlst.BorderBrush = 
                this.Btn_ExtProc_SlExp.BorderBrush =
                Brushes.DodgerBlue;
        }


        public void FocusFieldFromRowCell()
        {
            if (this.datagrid_main.SelectedCells.Count is 0) return;
            if (this.pnl_dock.Visibility is Visibility.Collapsed)
                ToggleSideGrid();
            if (this.datagrid_main.SelectedCells.Count is 0) return;
            if (this.datagrid_main.CurrentCell.Column is not DataGridColumn col) return;
            var colIdx = col.DisplayIndex;
            this.pnl_dock.Focus();
            //this.grid_dataeditregion.Focus();
            this.WPnl_DataEditRegion.Focus();
            //if (this.grid_dataeditregion.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
            Debug.WriteLine(colIdx);
            if (colIdx > this.WPnl_EditInputs.Children.Count - 1) return;
            if (this.WPnl_EditInputs.Children[colIdx] is not TextBox txt) return;
            txt.Focus();
            txt.Select(txt.Text.Length, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RevertEditControls()
        {
            /*
            this.WPnl_EditInputs.BindingGroup.CancelEdit();
            var tmp = WPnl_EditInputs.DataContext;
            this.WPnl_EditInputs.DataContext = null;
            this.WPnl_EditInputs.DataContext = tmp;
            this.WPnl_EditInputs.BindingGroup.UpdateSources();
            */
            //var t = DateTime.Now;
            /*
            if (this.WPnl_EditLabels.Children.OfType<Label>() is not IEnumerable<Control> cntrls) return;
            foreach (Label item in cntrls)
            {
                item.Tag = null;
            }
            if (this.WPnl_EditInputs.Children.OfType<Control>() is not IEnumerable<Control> inputs) return;
            foreach (Control item in inputs)
            {
                var dpinfo = Ext.DpValueFromInputType(item);
                if (dpinfo is null) continue;
                var be = item.GetBindingExpression(dpinfo.Value.dp);
                if (be is null) continue;
                be.UpdateTarget();
            }
            */
            
            foreach (Label item in this.WPnl_EditLabels.Children)
            {
                item.Tag = null;
            }
            foreach (Control item in this.WPnl_EditInputs.Children)
            {
                var dpinfo = Ext.DpValueFromInputType(item);
                if (dpinfo is null) continue;
                item.GetBindingExpression(dpinfo.Value.dp)?.UpdateTarget();
            }
            
            //MessageBox.Show((DateTime.Now - t).TotalMilliseconds.ToString());
        }

        public void DoMfgItemsFilter()
        {
            var filterText = new TextRange(this.Txt_Filter.Document.ContentStart, this.Txt_Filter.Document.ContentEnd).Text.ToLower().Trim();
            Ext.MfgItem_FilterGroups = [.. filterText.Split("||").Select(s => s.Trim())];
            Ext.MfgItem_GroupFilters = [.. Ext.MfgItem_FilterGroups.Select(g => g.Split('+', StringSplitOptions.RemoveEmptyEntries))];
            var viewSource = (CollectionViewSource)Resources["MfgItemsViewSource"];
            viewSource?.View?.Refresh();
        }

        private void RevertItemLineInput(Label inputLabel)
        {
            byte i = (byte)this.WPnl_EditLabels.Children.IndexOf(inputLabel);
            Control? inputControl = this.WPnl_EditInputs.Children[i] as Control;
            if (inputControl is null) return;
            var dpinfo = Ext.DpValueFromInputType(inputControl);
            if (dpinfo is null || !dpinfo.HasValue) return;
            if (this.datagrid_main.SelectedItem is not object obj ||
                inputControl.GetBindingExpression(dpinfo.Value.dp) is not BindingExpression be ||
                be.ParentBinding.Path is not PropertyPath pp ||
                obj.GetType().GetProperty(pp.Path) is not PropertyInfo pi)
            {
                return;
            }
            object? value = pi.GetValue(obj);
            this.CurrentItem?.GetType().GetProperty(pi.Name)?.SetValue(this.CurrentItem, value);
            be?.UpdateTarget();
            this.Pending_LineChanges.Remove(pi.Name);
            inputLabel.Tag = null;
        }
        #endregion


        #region EventHandlers

        private void MainViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not "FontSize_Base") return;
            OnPropertyChanged(nameof(DataGridFontSize));
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton) return;
            if (this.JobNbr is null) return;
            Debug.WriteLine("Load Job Data");
            await this.LoadDataForJob(this.JobNbr);
        }

        private async void EngOrder_JobNbrChanged(object? sender, string e)
        {
            this.ResetUI();
            await this.LoadDataForJob(e);
        }


        private void Pending_LineChanges_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Pending_LineChangesCount));
            OnPropertyChanged(nameof(IsPending_LineChanges));
            OnPropertyChanged(nameof(NoPending_LineChanges));
            OnPropertyChanged(nameof(UnsavedChangesPrompt));
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = new TextBlock()
            {
                Text = (e.Row.GetIndex() + 1).ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextAlignment = TextAlignment.Right,
                Margin = new(0, 0, 8, 0),
                Padding = new (4, 0, 4, 0)
            };
        }

        public void DataGrid_IceManuf_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() is not string headerName)
            {
                return;
            }
            string headerNameLower = headerName.ToLower();
            if (Ext.DataGrid_Manuf_ColumnsExcludedHidden.Contains(headerNameLower)) {
                e.Cancel = true;
            }
            e.Column.Visibility =
                Ext.DataGrid_Manuf_ColumnsExcludedHidden.Contains(headerNameLower) ?
                Visibility.Collapsed :
                Visibility.Visible;
            e.Column.IsReadOnly = Ext.DataGrid_Manuf_ColumnsReadonly.Contains(headerNameLower);
            
        }

        private void dataSideGridScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            //this.datagrid_side.AddHandler(MouseWheelEvent, new RoutedEventHandler(DataGridMouseWheelHorizontal), true);
        }

        private void datagrid_main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Enter)
            {
                e.Handled = true;
                FocusFieldFromRowCell();
                return;
            }
            if (e.Key is Key.F && (Keyboard.Modifiers & ModifierKeys.Control) is ModifierKeys.Control)
            {
                this.ToggleFiltersPanel();
                e.Handled = true;
            }
        }

        private void datagrid_main_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FocusFieldFromRowCell();
        }

        public void Btn_CollapseSideGrid_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleSideGrid();
        }

        public void Btn_CollapseTopBar_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleHeader();
        }

        private void Txt_Filter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is not Key.Enter) return;
            e.Handled = true;
            
            this.DoMfgItemsFilter();
        }

        // Doesn't highlight correctly, indicies are messed up
        private void Txt_Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Unhook event to prevent recursion during formatting changes
            Txt_Filter.TextChanged -= Txt_Filter_TextChanged;

            // Get the current text
            TextRange textRange = new TextRange(Txt_Filter.Document.ContentStart, Txt_Filter.Document.ContentEnd);
            string fullText = textRange.Text;

            // Clear previous formatting
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);

            // Example: Highlight the keyword "public"
            Regex publicRegex = new Regex(@"\+|\=");
            foreach (Match match in publicRegex.Matches(fullText))
            {
                // Create a new TextRange for the matched text
                TextPointer start = Txt_Filter.Document.ContentStart.GetPositionAtOffset(match.Index);
                TextPointer end = Txt_Filter.Document.ContentStart.GetPositionAtOffset(match.Index + match.Length);
                Debug.WriteLine(match.Index + " " + (match.Index + match.Length));
                TextRange wordRange = new TextRange(start, end);

                Debug.WriteLine(wordRange.Text);
                // Apply color
                wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DodgerBlue);
            }

            // Re-hook event
            Txt_Filter.TextChanged += Txt_Filter_TextChanged;
        }

        private void MfgItemsViewSource_Filter(object sender, FilterEventArgs e)
        {
            // Assuming 'MyDataItem' is the type of objects in collection
            if (e.Item is not DBModels.Order.AIcManuf item)
            {
                return;
            }

            // Get text from TextBox
            var filterText = new TextRange( this.Txt_Filter.Document.ContentStart, this.Txt_Filter.Document.ContentEnd).Text.ToLower().Trim();
            // If the filter text is empty, accept all items
            if (filterText is null || filterText is "")
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = Ext.MfgItems_Filter(item, filterText);
        }

        private void Btn_FilterClose_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleFiltersPanel();
            e.Handled = true;
        }

        private async void Btn_AcceptItemLineEdits_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not null)
            {
                if (Ext.PopupConfirmation("Accept changes made to item line? This action cannot be undone.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) is not MessageBoxResult.Yes) return;
            }
            //if (this.grid_dataeditregion.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
            if (this.datagrid_main.SelectedItem is not AIcManuf line) return;
            if (CurrentItem is null) return;
            List<(string, PropUpdateResult, Type?, object?)> changes = [];
            var i = -1;
            using (var ctx = new OrderDbCtx())
            {
                var dbline = await ctx.AIcManufs
                    .FirstOrDefaultAsync(i => i.ManufId == line.ManufId);
                if (dbline is null)
                {
                    Ext.MainWindow.MainToastContainer.CreateToast(
                        "Eng Order",
                        $"Item line- Manuf Line not found or may have been deleted. Refresh and try editting again."
                    ).Show();
                    return;
                }
                var lbls = this.WPnl_EditLabels.Children.OfType<Label>().ToArray().AsSpan();
                var txts = this.WPnl_EditInputs.Children.OfType<Control>().ToArray().AsSpan();
                this.datagrid_main.BeginEdit();
                foreach (var item in txts)
                {
                    i++;
                    var lbl = lbls[i];
                    if (lbl.Tag is null)
                    {
                        continue;
                    }
                    var dpinfo = Ext.DpValueFromInputType(item);
                    if (dpinfo is not (DependencyProperty, object) dpinfoval) continue;
                    var binding = item.GetBindingExpression(dpinfoval.dp);
                    
                    var propertyName = binding.ParentBinding.Path.Path;
                    if (propertyName is null ||
                        Ext.DataGrid_Manuf_ColumnsExcludedHidden.Contains(propertyName) ||
                        Ext.DataGrid_Manuf_ColumnsReadonly.Contains(propertyName) ||
                        !this.Pending_LineChanges.Contains(propertyName))
                    {
                        continue;
                    }

                    var value = CurrentItem.GetType()?.GetProperty(propertyName)?.GetValue(CurrentItem, null);
                    //if (value is not object && value is null) continue;
                    var propRes = Ext.UpdateProperty(line, propertyName, value);
                    _ = Ext.UpdateProperty(dbline, propertyName, value);
                    MessageBox.Show(value?.ToString());
                    switch (propRes.Item2)
                    {
                        case PropUpdateResult.SameValue:
                            this.Pending_LineChanges.Remove(propertyName);
                            lbl.Tag = null;
                            break;
                        case PropUpdateResult.Worked:
                            changes.Add(propRes);
                            lbl.Tag = "good";
                            this.Pending_LineChanges.Remove(propertyName);
                            break;
                        case PropUpdateResult.Error:
                            lbl.Tag = "error";
                            Ext.MainWindow.MainToastContainer.CreateToast(
                                "Eng Order",
                                $"Item line- Error for property '{propertyName}'",
                                FeedbackToast.IconTypes.Error
                            ).Show();
                            break;
                        case PropUpdateResult.NoPropOrCantWrite:
                            lbl.Tag = "warn";
                            Ext.MainWindow.MainToastContainer.CreateToast(
                                "Eng Order",
                                $"Item line- NoPropOrCantWrite for property '{propertyName}'",
                                FeedbackToast.IconTypes.Error
                            ).Show();
                            break;
                        case PropUpdateResult.ConversionFailed:
                            Ext.MainWindow.MainToastContainer.CreateToast(
                                "Eng Order",
                                $"Item line- ConversionFailed for property '{propertyName}'",
                                FeedbackToast.IconTypes.Error
                            ).Show();
                            break;
                    }
                }
                this.datagrid_main.CommitEdit();
                await ctx.SaveChangesAsync();
            }
            if (this.Pending_LineChanges.Count is 0)
            {
                Ext.MainWindow.MainToastContainer.CreateToast(
                    "Eng Order",
                    $"Saved {changes} Item Line changes successfully"
                ).Show();
            } else
            {
                Ext.MainWindow.MainToastContainer.CreateToast(
                    "Eng Order",
                    "Item line changes encountered an error with a property",
                    FeedbackToast.IconTypes.Error
                ).Show();
            }
            /*
            if (this.WPnl_EditInputs.BindingGroup.CommitEdit())
            {
                List<(string, bool, object?)> changes = [];
                using (var ctx = new OrderDbCtx())
                {
                    var dbline = await ctx.AIcManufs
                        .FirstOrDefaultAsync(i => i.ManufId == line.ManufId);
                    if (dbline is null)
                    {
                        Ext.MainWindow.MainToastContainer.CreateToast("Eng Order", $"Item line was not found and may have been deleted, refresh item lines and try again", FeedbackToast.IconTypes.Error).Show();
                        return;
                    }
                    var lineprops = line.GetType().GetProperties(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in dbline.GetType().GetProperties(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (lineprops.FirstOrDefault(p => p.Name == prop.Name) is not PropertyInfo lineprop) continue;
                        var lineval = lineprop.GetValue(line);
                        var propval = prop.GetValue(dbline);
                        if (lineval != propval)
                        {
                            prop.SetValue(dbline, lineval);
                            prop.SetValue(this.datagrid_main.SelectedItem, lineval);
                        }
                    }
                    await ctx.SaveChangesAsync();
                    Pending_LineChangesCount = 0;
                }
                if (changes.Count is 0) return;
                Debug.WriteLine(string.Join("\n", changes.Select(c => $"{c.Item1} = {c.Item3}")));
            }
            */
        }

        private void Btn_RevertItemLineEdits_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not null)
            {
                if (Ext.PopupConfirmation("Discard changes made to item line? All unsaved changes will be lost.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) is not MessageBoxResult.Yes) return;
            }
            //if (this.grid_dataeditregion.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
            this.RevertEditControls();
            this.CurrentItem = (AIcManuf)this.datagrid_main.SelectedItem;
            this.WPnl_EditInputs.BindingGroup.UpdateSources();
            Ext.MainWindow.MainToastContainer.CreateToast("Eng Order", "Item line changes discarded").Show();
            this.Pending_LineChanges = [];
        }

        private async void Btn_DeleteItemLine_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not null)
            {
                if (Ext.PopupConfirmation("Are you sure you want to delete this item line? This action cannot be undone.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Stop) is not MessageBoxResult.Yes) return;
            }
            if (this.datagrid_main.SelectedItem is not DBModels.Order.AIcManuf line) return;
            var res = await Ext.DeleteItemLine(line.ManufId, line.JobNbr ?? JobNbr ?? "");
            if (res)
            {
                this._mfgItemLines.Remove(line);
                OnPropertyChanged(nameof(this.MfgItemLines));
                Ext.MainWindow.MainToastContainer.CreateToast("Eng Order", "Item line deleted successfully").Show();
            }
            else
            {
                Ext.MainWindow.MainToastContainer.CreateToast("Eng Order", "Item line deletion failed :(", FeedbackToast.IconTypes.Error).Show();
            }
        }

        private async void Btn_NewItemLine_Click(object sender, RoutedEventArgs e)
        {
            if (Ext.PopupConfirmation("Are you sure you'd like to create a new line using the current line's values? New line will have \"(COPY)\" append to the end of its description", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) is not MessageBoxResult.Yes) return;
            if (this.datagrid_main.SelectedItem is not DBModels.Order.AIcManuf line) return;
            var res = await Ext.CopyItemLineData(line.ManufId);
            if (res is false)
            {
                Ext.MainWindow.MainToastContainer.CreateToast("Eng Order", "Item line not copied successfully :(", FeedbackToast.IconTypes.Error).Show();
                return;
            }
            Ext.MainWindow.MainToastContainer.CreateToast("Eng Order", "Item line copied successfully").Show();
            if (this.JobNbr is null) return;
            await this.LoadManufData(this.JobNbr);
        }

        private void Btn_AcceptChanges_Click(object sender, RoutedEventArgs e)
        {
            // Force commit any pending edits
            datagrid_main.CommitEdit(DataGridEditingUnit.Row, true);
            datagrid_main.CommitEdit(DataGridEditingUnit.Cell, true);
        }

        private void Btn_RejectChanges_Click(object sender, RoutedEventArgs e)
        {
            // Cancel pending edits
            datagrid_main.CancelEdit(DataGridEditingUnit.Row);
            datagrid_main.CancelEdit(DataGridEditingUnit.Cell);
        }

        private async void LabelInputLookupPair_TagChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine("TagChanged");
            Debug.WriteLine(sender?.GetType().Name);
            if (sender is not TextBox txt || txt.Tag is not Guid lookupGuid) return;
            Debug.WriteLine(lookupGuid);
            if (lookupGuid.ToString() is "00000000-0000-0000-0000-000000000000")
            {
                txt.Text = "-";
                return;
            }
            if (SCH.Global.Config is null || !SCH.Global.Config.InitializationSuccessfull) return;
            Debug.WriteLine($"Try Query GUID lookup ({lookupGuid})");
            using (var ctx = new DBModels.Product.ProductDbCtx())
            {
                var res = await ctx.IcItems
                    .Where(p => p.ItemId.ToString() == lookupGuid.ToString())
                    .AsNoTracking() // No change tracking
                    .AsSplitQuery()
                    .ToListAsync();
                if (res is null || res.Count is 0 || res[0] is not IcItem item)
                {
                    txt.Text = "-";
                    return;
                }
                txt.Text = $"{item.Item} - {item.Description}";
            }
        }

        public IList<Button> EngOrder_LoookupButtons = [];
        public IList<TextBox> EngOrder_LookupInputs = [];
        private async void BtnLookup_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (this.EngOrder_LookupInputs[this.EngOrder_LoookupButtons.IndexOf(btn)] is not TextBox txt) return;
            Debug.WriteLine("Try TextBox");
            var (partFilter, descFilter, partConstraint, descConstraint) = btn.Tag.ToString() switch
            {
                "fab" => (null, null, "%", "Fab%"),
                "wks" => ("p-0512gp", null, "%", "PLW%"),
                "web" => (null, null, "70055%", "%"),
                "mel" => ("p-0512gp", "", "%", "Mel%"),
                "lam" => ("p-0512gp", "", "%", "Pnl%"),
                "chs" => (null, null, "%", "CDmel%"),
                "acr" => (null, null, "338%", "Acr%"),
                "hcd" => (null, null, "%-0508gp", "Lam%"),
                "crv" => (null, null, "%-0508gp", "Lam%"),
                _ => (null, null, null, null)
            };
            if (partConstraint is null || descConstraint is null) return;
            using var lookup = new LookupFinder()
            {
                Owner = Ext.MainWindow
            };
            await lookup.LookupMaterials(partFilter, descFilter, partConstraint, descConstraint);
            if (lookup.ShowDialog() is not true || lookup.ViewModel?.ReturnObject is not Dictionary<string, object> obj) return;
            if (txt.GetBindingExpression(TextBox.TagProperty) is not BindingExpression be) return;
            if (be.ResolvedSource.GetType().GetProperty(be.ResolvedSourcePropertyName) is not PropertyInfo pi) return;
            pi.SetValue(be.ResolvedSource, (Guid?)obj["ItemID"] ?? new Guid());
            ColorSetInfo_PropertyChanged(this.ColorSetInfo, new PropertyChangedEventArgs(be.ResolvedSourcePropertyName));
            //txt.Tag = (Guid?)obj["ItemID"] ?? new Guid();
            //MessageBox.Show(lookup.ReturnObject["ItemID"].ToString());
        }

        private async void LabelInputLookupPair_LookupButtonPressed(object sender, LabelInputLookupPair.Lookup_EventArgs e)
        {
            var (partFilter, descFilter, partConstraint, descConstraint) = e.Source.InputLookupType switch
            {
                "fab" => (null, null, "%", "Fab%"),
                "wks" => ("p-0512gp", null, "%", "PLW%"),
                "web" => (null, null, "70055%", "%"),
                "mel" => ("p-0512gp", "", "%", "Mel%"),
                "lam" => ("p-0512gp", "", "%", "Pnl%"),
                "chs" => (null, null, "%", "CDmel%"),
                "acr" => (null, null, "338%", "Acr%"),
                "hcd" => (null, null, "%-0508gp", "Lam%"),
                "crv" => (null, null, "%-0508gp", "Lam%"),
                _ => (null, null, null, null)
            };
            if (partConstraint is null || descConstraint is null) return;
            using var lookup = new LookupFinder()
            {
                Owner = Ext.MainWindow
            };
            await lookup.LookupMaterials(partFilter, descFilter, partConstraint, descConstraint);
            if (lookup.ShowDialog() is not true || lookup.ViewModel?.ReturnObject is not Dictionary<string, object> obj) return;
            e.Source.InputLookup = obj["ItemID"]?.ToString() ?? "-";
            //MessageBox.Show(lookup.ReturnObject["ItemID"].ToString());
        }

        private async void Btn_RefreshHeader_Click(object sender, RoutedEventArgs e)
        {
            if (this.JobNbr is null) return;
            this.ColorSetInfo_Changes = [];
            this.Btn_RefreshHeader.IsEnabled = false;
            await this.LoadColorSetData(this.JobNbr);
            await this.LoadDataForJob(this.JobNbr);
            await Task.Run(async () =>
            {
                await Task.Delay(DELAY_HEADER_REFRESH);
                await Dispatcher.BeginInvoke(() =>
                    this.Btn_RefreshHeader.IsEnabled = true
                );
            });
        }

        private async void Btn_SaveHeader_Click(object sender, RoutedEventArgs e)
        {
            if (this.ColorSetInfo_Changes.Count is 0) return;
            if (this.ColorSetInfo.ColorSetId.ToString().Length < 8) return;
            try
            {
                byte orderChangesMade = 0;
                using var context = new DBModels.Order.OrderDbCtx();
                var dborder = await context.AIcColorSets
                    .Where(o => o.ColorSetId == ColorSetInfo.ColorSetId)
                    .FirstAsync();
                if (dborder is null) return;
                var props = dborder.GetType().GetProperties(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance).ToList();
                foreach (var prop in CollectionsMarshal.AsSpan(props))
                {
                    if (!this.ColorSetInfo_Changes.ContainsKey(prop.Name)) continue;
                    var orderprop = ColorSetInfo.GetType().GetProperty(prop.Name, BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance);
                    if (orderprop is null) continue;
                    var dbval = prop.GetValue(dborder);
                    var orderval = orderprop.GetValue(ColorSetInfo);
                    if (orderval != dbval)
                    {
                        Debug.WriteLine($"{prop.Name} | {dbval} => {orderval}");
                        prop.SetValue(dborder, orderval);
                        orderChangesMade++;
                    }
                }
                await context.SaveChangesAsync();
                Ext.MainWindow.MainToastContainer.CreateToast("Eng Order", $"Saved {orderChangesMade} header changes successfully", FeedbackToast.IconTypes.Info).Show();
                this.ColorSetInfo_Changes = [];
            } catch
            {
                Ext.MainWindow.MainToastContainer.CreateToast("Eng Order", "Header changes not saved successfully :(", FeedbackToast.IconTypes.Error).Show();
            }
        }

        private async void ExtProc_Button_Click(string exePath, Button? sender)
        {
            if (this.ExternalProc_GroupActive["stdEngProcs"] is true)
                return;
            else
                this.ExternalProc_GroupActive["stdEngProcs"] = true;

            this.ResetUI();

            ProcessStartInfo psi = new(
                exePath,
                Ext.extProc_ColorSet_arg.Replace(
                    "{@ColorSetID}",
                    this.ColorSetInfo.ColorSetId.ToString().ToUpper()
                )
            );
            sender?.BorderBrush = Brushes.Goldenrod;
            await Ext.RunExternal(psi);
            sender?.BorderBrush = Brushes.ForestGreen;
            this.ExternalProc_GroupActive["stdEngProcs"] = false;
        }

        private void Btn_ExtProc_ImpMfg_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.Directory.Exists($"C:/{JobNbr}") is false)
            {
                MessageBox.Show($"Folder 'C:/{JobNbr}' not found", "Local file not found", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            this.ExtProc_Button_Click(Ext.extProc_BaseDir + Ext.extProc_New_ImportEngMfg, (Button)(sender ?? this.Btn_ExtProc_ImpMfg));
        }

        private void Btn_ExtProc_EngPrc_Click(object sender, RoutedEventArgs e)
        {
            this.ExtProc_Button_Click(Ext.extProc_BaseDir + Ext.extProc_New_ProcEngMfg, (Button)(sender ?? this.Btn_ExtProc_EngPrc));
        }

        private void Btn_ExtProc_Matl_Click(object sender, RoutedEventArgs e)
        {
            this.ExtProc_Button_Click(Ext.extProc_BaseDir + Ext.extProc_New_CalcEngMatl, (Button)(sender ?? this.Btn_ExtProc_Matl));
        }

        private void Btn_ExtProc_Cutlst_Click(object sender, RoutedEventArgs e)
        {
            this.ExtProc_Button_Click(Ext.extProc_BaseDir + Ext.extProc_New_CncCutList, (Button)(sender ?? this.Btn_ExtProc_Cutlst));
        }

        private void Btn_ExtProc_SlExp_Click(object sender, RoutedEventArgs e)
        {
            this.ExtProc_Button_Click(Ext.extProc_BaseDir + Ext.extProc_New_SymEngExp, (Button)(sender ?? this.Btn_ExtProc_SlExp));
        }

        private async void Btn_rowEdit_Lookup_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null || sender is not Button btn) return;
            if (btn.Tag?.ToString() is not string tag) return;
            await ItemInfoLookup(tag, e);
        }

        public async Task ItemInfoLookup(string type, RoutedEventArgs e)
        {
            using var lookup = new LookupFinder()
            {
                Owner = Ext.MainWindow
            };
            await lookup.LookupItem();
            if (lookup.ShowDialog() is not true || lookup.ViewModel?.ReturnObject is not DBModels.Product.IcItem obj) return;
            TextBox? txt = null;
            string? value = null;
            switch (type)
            {
                case "ItemNbr":
                    txt = this.Txt_RowEdit_ItemNbr;
                    value = obj.Item;
                    break;
                case "Description":
                    txt = this.Txt_RowEdit_Desc;
                    value = obj.Description;
                    break;
                default:
                    break;
            }
            if (txt is null || value is null) return;

            txt.Clear();
            txt.AppendText(value);
            txt.Select(txt.Text.Length, 0);
            txt.Focus();
            this.TextBox_TextChanged(txt, new TextChangedEventArgs(e.RoutedEvent, UndoAction.None));
        }


        public bool WaitingForTextChanged = false;


        public void HandleInputValueChanged(object sender, EventArgs e)
        {
            if (!WaitingForTextChanged) return;
            if (sender is not Control cntrl) return;
            var index = WPnl_EditInputs.Children.IndexOf(cntrl);
            var lbl = (Label)WPnl_EditLabels.Children[index];
            //if (cntrl.Background is not null && cntrl.Background.Opacity == 50 / 255.0) return;
            Debug.WriteLine("%");
            var foo = Ext.DpValueFromInputType(cntrl);
            if (foo is not (DependencyProperty, object) pair || pair.dp is null) return;
            //if (txt.Background is not null && txt.Background.Opacity is 50) return;
            if (this.datagrid_main.SelectedItem is not object obj ||
                cntrl.GetBindingExpression(pair.dp) is not BindingExpression be ||
                be.ParentBinding.Path is not PropertyPath pp ||
                obj.GetType().GetProperty(pp.Path) is not PropertyInfo pi)
            {
                return;
            }
            object? value = pi.GetValue(obj);
            Debug.WriteLine("%%");
            if (pair.value.ToString()?.Equals($"{value?.ToString()}", StringComparison.Ordinal) ?? false)
            {
                Debug.WriteLine("Same value as data context object");
                if (this.Pending_LineChanges.Contains(pi.Name))
                {
                    this.Pending_LineChanges.Remove(pi.Name);
                    lbl.Tag = null;
                }
                //cntrl.Background = null;
            }
            else
            {
                if (!this.Pending_LineChanges.Contains(pi.Name))
                {
                    this.Pending_LineChanges.Add(pi.Name);
                    lbl.Tag = "warn";
                }
            }
            OnPropertyChanged(nameof(Pending_LineChanges));
            Debug.WriteLine(this.Pending_LineChangesCount);
            //cntrl.Background = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
            => HandleInputValueChanged(sender, e);

        private void TextBox_TextChanged(object sender, SelectionChangedEventArgs e)
            => HandleInputValueChanged(sender, e);

        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
            => HandleInputValueChanged(sender, e);

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.WaitingForTextChanged = true;
        }
        private void TextBox_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            this.WaitingForTextChanged = true;
        }

        private void TextBox_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            this.WaitingForTextChanged = false;
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus is TextBox) return;
            this.WaitingForTextChanged = false;
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not ComboBox cmbx) return;
            if (cmbx.IsDropDownOpen) return;
            e.Handled = true;
            var newEventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };
            this.WPnl_DataEditRegion.RaiseEvent(newEventArgs);
        }
        private void datagrid_main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.WPnl_EditInputs.BindingGroup.BeginEdit();
            if (this.datagrid_main.SelectedItem is DBModels.Order.AIcManuf item)
            {
                CurrentItem = item;
            }
            RevertEditControls();
        }

        private void datagrid_main_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsPending_LineChanges)
            {
                Ext.MainWindow.MainToastContainer.CreateToast("Eng Order", "Unsaved changes in Row Edit Pane, Grid is locked", FeedbackToast.IconTypes.Warn).Show();
                e.Handled = true;
            }
        }

        public void FocusSelectedItem(ref DataGrid dg)
        {
            if (dg.SelectedCells.Count is 0) return;
            if (LastCell is null) return;
            LastCell?.Focus();
            /*
            if (dg.SelectedItems.Count is 0) return;
            int rowindex = dg.SelectedIndex;
            DataGridRow dataRow = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(rowindex);
            if (dataRow is null) return;
            FocusManager.SetFocusedElement(dg, dataRow as IInputElement);
            */
        }

        private static readonly Regex _regex_notnumeric = new("[^0-9.]+");
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //e.Handled = (e.Text is " " || _regex_notnumeric.IsMatch(e.Text));
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            var ctrl = ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
            if (!ctrl)
            {
                if (sender is not TextBox txt || txt.Tag is not string tag) return;
                var numeric = tag?.ToLower() is "numeric";
                if (numeric)
                {
                    switch (e.Key)
                    {
                        case Key.Left:
                        case Key.Right:
                        case Key.Tab:
                        case Key.Delete:
                        case Key.Back:
                        case Key.D1:
                        case Key.NumPad1:
                        case Key.D2:
                        case Key.NumPad2:
                        case Key.D3:
                        case Key.NumPad3:
                        case Key.D4:
                        case Key.NumPad4:
                        case Key.D5:
                        case Key.NumPad5:
                        case Key.D6:
                        case Key.NumPad6:
                        case Key.D7:
                        case Key.NumPad7:
                        case Key.D8:
                        case Key.NumPad8:
                        case Key.D9:
                        case Key.NumPad9:
                        case Key.D0:
                        case Key.NumPad0:
                            e.Handled = false;
                            break;
                        default:
                            e.Handled = true;
                            break;
                    }
                    return;
                }
                switch (e.Key)
                {
                    case Key.Escape:
                        break;
                    default:
                        e.Handled = false;
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.S:
                        this.Btn_AcceptItemLineEdits_Click(null, new RoutedEventArgs(null, null));
                        e.Handled = true;
                        break;
                    case Key.R:
                        this.Btn_RevertItemLineEdits_Click(null, new RoutedEventArgs(null, null));
                        e.Handled = true;
                        break;
                    case Key.E:
                        this.Btn_DeleteItemLine_Click(null, new RoutedEventArgs(null, null));
                        e.Handled = true;
                        break;
                    case Key.I:
                        this.Txt_RowEdit_ItemNbr.Focus();
                        this.Txt_RowEdit_ItemNbr.Select(this.Txt_RowEdit_ItemNbr.Text.Length, 0);
                        e.Handled = true;
                        break;
                    case Key.D:
                        this.Txt_RowEdit_Desc.Focus();
                        this.Txt_RowEdit_Desc.Select(this.Txt_RowEdit_Desc.Text.Length, 0);
                        e.Handled = true;
                        break;
                    default:
                        e.Handled = false;
                        break;
                }
            }
        }


        public DataGridCell? LastCell = null;
        private bool disposedValue;

        private void datagrid_main_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.datagrid_main.SelectedCells.Count is 0) return;
            if (Ext.GetCellFromDataGrid(ref datagrid_main, datagrid_main.SelectedCells.FirstOrDefault()) is not DataGridCell cell) return;
            LastCell = cell;
        }


        private void Btn_SavedFilters_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not string filterStr) return;
            bool add = false;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) ||  Keyboard.IsKeyDown(Key.RightCtrl))
            {
                add = true;
            }
            var existingText = new TextRange(this.Txt_Filter.Document.ContentStart, this.Txt_Filter.Document.ContentEnd).Text.Trim();
            FlowDocument flowDoc = new();
            this.Txt_Filter.Document.Blocks.Clear();
            if (add)
            {
                flowDoc.Blocks.Add(new Paragraph(new Run(
                    existingText +
                    "+" +
                    filterStr
                )));
            } else
            {
                flowDoc.Blocks.Add(new Paragraph(new Run(
                    filterStr
                )));
            }
            this.Txt_Filter.Document = flowDoc;
            this.DoMfgItemsFilter();
        }

        private void Page_EngOrder_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.F && (Keyboard.Modifiers & ModifierKeys.Control) is ModifierKeys.Control)
            {
                this.ToggleFiltersPanel();
                e.Handled = true;
            }
        }

        private void datagrid_main_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is null) return;
            Dispatcher.BeginInvoke(() =>
            {
                string str = "";
                byte i = 0;
                foreach (var col in this.datagrid_main.Columns.OrderBy(c => Ext.DataGrid_Manuf_ColumnsOrder.IndexOf(c.Header.ToString()?.ToLower() ?? "")))
                {
                    if (col.Header is not string headerName) continue;
                    if (Ext.DataGrid_Manuf_ColumnsOrder.IndexOf(headerName.ToLower()) is int idx && idx is not -1)
                    {
                        col.DisplayIndex = idx;
                        str += headerName.PadRight(24, ' ') + col.DisplayIndex.ToString().PadLeft(2, '0') + "\n";
                    }
                    else
                    {
                        col.DisplayIndex = Ext.DataGrid_Manuf_ColumnsOrder.Count + i++;
                    }
                }
                Clipboard.SetText(str);
                MessageBox.Show(str);
            }, DispatcherPriority.Loaded);
        }

        private void Btn_RevertItemLinePropEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.TemplatedParent is not Label lbl) return;
            this.RevertItemLineInput(lbl);
        }

        private void Btn_FilterSave_Click(object sender, RoutedEventArgs e)
        {
            if (Ext.EngOrder_Filters is null) return;
            if (new TextRange(this.Txt_Filter.Document.ContentStart, this.Txt_Filter.Document.ContentEnd).Text is not string filterContents ||
                filterContents.Trim() is "") return;
            string filterName = Microsoft.VisualBasic.Interaction.InputBox(
                "Name for filter",
                "Filter Creation Popup",
                $"Filter {Ext.EngOrder_Filters.filters.Count, 2}"
            );
            if (filterName.Trim() is "")
            {
                Ext.MainWindow.MainToastContainer.CreateToast(
                    "EngOrder",
                    $"Filter name cannot be blank",
                    FeedbackToast.IconTypes.Warn,
                    2500
                ).Show();
                return;
            }
            var existsOverride = true;
            if (Ext.EngOrder_Filters.filters.FindIndex(o => o.name.Equals(filterName, StringComparison.CurrentCultureIgnoreCase)) is int found && found is not -1)
            {
                var res = MessageBox.Show($"Filter with name '{filterName}' already exists, replace it? This action is not reversible.", "Filter Creation Popup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                existsOverride = (res is MessageBoxResult.Yes);
            }
            if (!existsOverride)
            {
                Ext.MainWindow.MainToastContainer.CreateToast(
                    "EngOrder",
                    $"Filter with name '{filterName}' already exists",
                    FeedbackToast.IconTypes.Error,
                    2500
                ).Show();
                return;
            }
            Ext.FilterSet? filter = null;
            if (found is not -1)
            {
                filter = Ext.EngOrder_Filters.filters[found];
                filter.name = filterName;
                filter.value = filterContents;
            } else
            {
                filter = new()
                {
                    name = filterName,
                    value = filterContents
                };
                Ext.EngOrder_Filters.filters.Add(filter);
            }
            Ext.EngOrder_Filters.filters = Ext.EngOrder_Filters.filters;
            Ext.Save(Ext.TomlConfigTypes.EngOrder_SavedFilters, Ext.EngOrder_Filters);
        }

        private void Btn_FilterHelp_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

    }
}
