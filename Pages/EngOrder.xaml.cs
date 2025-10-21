using Microsoft.VisualBasic;
using MyApp.DataAccess.Generated;
using OMPS.Components;
using OMPS.viewModel;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
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
using Windows.AI.MachineLearning;
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
            this.ColorSetInfo.PropertyChanged += this.ColorSetInfo_PropertyChanged;
        }

        private void ColorSetInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (this.IsLoadingJobData) return;
            if (e.PropertyName is not string propName) return;
            if (sender is not example_queries_GetColorSetResult obj) return;
            if (obj.GetType().GetProperty(propName) is not PropertyInfo propInfo) return;
            if (propInfo.GetValue(obj) is not object val) return;
            this.ColorSetInfo_Changes[propName] = val;
            this.Btn_SaveHeader.IsEnabled = true;
            Debug.WriteLine($"{propName} -> {val}");
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #region Events
        public event EventHandler<string> JobNbrChanged;
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion


        #region Properties
        public double DataGridFontSize
        {
            get => MainViewModel.FontSize_Base;
        }
        public Main_ViewModel MainViewModel
        {
            get => Ext.MainWindow.MainViewModel;
        }
        public MainWindow? ParentWindow
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

        public const short DELAY_HEADER_REFRESH = 5000;

        public const string extProc_ImportEngMfg_exe = "P:\\!CRM\\IceMfgImport.exe";
        public const string extProc_ProcEngMfg_exe = "P:\\!CRM\\IceMfgProcess.exe";
        public const string extProc_CalcEngMatl_exe = "P:\\!CRM\\IceMatlCalc.exe";
        public const string extProc_CncCutList_exe = "P:\\!CRM\\IceCNCCutList.exe";
        public const string extProc_SymEngExp_exe = "P:\\!CRM\\SymIceExp.exe";

        public const string extProc_ColorSet_arg = "iColorSetID={{@ColorSetID}}";

        public Dictionary<string, bool> ExternalProc_GroupActive = new()
        {
            { "stdEngProcs", false }
        };

        public bool IsLoadingJobData { get; set; } = false;
        private DateTime? Last_ManufData = null;

        private bool Pending_LineChanges
        {
            get => (bool)GetValue(Pending_LineChangesProperty);
            set => SetValue(Pending_LineChangesProperty, (bool)value);
        }
        public bool NoPending_LineChanges { get => !this.Pending_LineChanges; }

        internal DataGrid? CurrentGrid { get; set; }
        public Dictionary<string, string[]> ItemLineFilers { get; set; } = [];
        public string[] Finishes_Default { get; } = ["NA", "CH", "DB", "GY", "PL", "TP"];
        public string[] Finishes_BS { get; } = ["NA", "SL", "BK"];

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
        public example_queries_GetColorSetResult ColorSetInfo { get; set; } = new();
        public Dictionary<string, object> ColorSetInfo_Changes { get; set; } = [];
        public ObservableCollection<example_queries_GetItemLinesByJobResult> MfgItemLines { get; set; } = [];
        #endregion


        #region Fields

        public int RowSpan = 1;
        private readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsExcludedHidden =
            ["IceManufID", "ColorSetID", "ProductID",
            "ProductLinkID", "ItemID", "QuoteNbr", "CustOrderNbr",
            "BpartnerAvailable", "CustomerAvailable", "CreatedByID",
            "ChangedbyID", "ChangebyIDOffline"];
        private readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsReadonly =
            ["QuoteNbr", "JobNbr", "CustOrderNbr",
            "Usertag1", "Multiplier", "Area", "CreationDate", "ChangeDate"];
        private readonly ReadOnlyCollection<string> DataGrid_IceManuf_ColumnsOrder = [
            "JobNbr", "PartNbr", "ItemNbr", "CatalogNbr", "Qty", "Multiplier",
            "Description", "UofM", "Type", "SubType", "IDNbr", "Explode",
            "Assembled", "AssyNbr", "TileIndicator", "ItemFin", "ColorBy",
            "WorkCtr"
            ];
        #endregion


        #region Methods
        public async void LoadDataForJob(string job)
        {
            string tab = "";
            try
            {
                IsLoadingJobData = true;
                this.CurrentGrid?.Visibility = Visibility.Collapsed;
                await this.LoadColorSetData(job);
                if (this.RadioBtn_View_QPO.IsChecked is true)
                {
                    tab = this.RadioBtn_View_QPO.Tag.ToString() ?? "";
                    this.CurrentGrid = this.datagrid_QPO;
                    await this.LoadQPartsOrdered(job);
                }
                else if (this.RadioBtn_View_QDO.IsChecked is true)
                {
                    tab = this.RadioBtn_View_QDO.Tag.ToString() ?? "";
                    this.CurrentGrid = this.datagrid_QIO;
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
                    this.CurrentGrid = this.datagrid_MP;
                    await this.LoadManufParts(job);
                }
                this.CurrentGrid?.Visibility = Visibility.Visible;
                this.ParentWindow?.SetUrlRelPath($"?job={job}&tab={tab}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            IsLoadingJobData = false;
        }

        public async Task LoadColorSetData(string job)
        {
            await Task.Run(() =>
            {
                Debug.WriteLine("Loading Color Set Data");
                var data_info = Ext.Queries.GetColorSet(job).First();
                PropertyCopier<example_queries_GetColorSetResult>.Copy(data_info, this.ColorSetInfo);
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
            if (this.Last_ManufData is not null && (DateTime.Now - this.Last_ManufData.Value).TotalSeconds is double sec && sec < 10)
            {
                Debug.WriteLine($"Last update was {sec} sec ago");
                return;
            }
            Debug.WriteLine("Loading Manuf Data");
            this.MfgItemLines.Clear();
            this.progbar_itemlines.Value = 50;
            this.progbar_itemlines.IsEnabled = true;
            this.progbar_itemlines.Visibility = Visibility.Visible;
            List<example_queries_GetItemLinesByJobResult> data_mfglines = [];
            await Task.Run(() =>
            {
                data_mfglines = Ext.Queries.GetItemLinesByJob(job);
                this.Dispatcher.BeginInvoke(() =>
                {
                    //this.datagrid_main.BeginEdit();
                    for (int i = 0; i < data_mfglines.Count; i++)
                    {
                        Debug.WriteLine(i);
                        this.MfgItemLines.Add(data_mfglines[i]);
                    }
                    if (this.datagrid_main.Items.Count is not 0)
                    {
                        this.datagrid_main.ScrollIntoView(this.datagrid_main.Items[0]);
                    }
                    //this.datagrid_main.EndInit();
                    Ext.MainWindow.SetTabTitle($"{this.JobNbr}");
                    this.progbar_itemlines.Value = 0;
                    this.progbar_itemlines.IsEnabled = false;
                    this.progbar_itemlines.Visibility = Visibility.Collapsed;
                });
            });
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

        public bool MfgItems_Filter(example_queries_GetItemLinesByJobResult item, string filterText)
        {

            var properties = typeof(example_queries_GetItemLinesByJobResult).GetProperties();

            string[] filterGroups = filterText.Split(' ');
            string[][] groupFilters = [.. filterGroups.Select(g => g.Split('+', StringSplitOptions.RemoveEmptyEntries))];

            int i = 0;
            foreach (var group in groupFilters)
            {
                List<string> groupReqd = [.. group];
                foreach (var filter in group)
                {
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(item)?.ToString();
                        //Debug.WriteLine(filter + " == " + value);
                        if (value is not null && value.ToLower().Contains(filter))
                        {
                            groupReqd.Remove(filter);
                            if (groupReqd.Count is 0)
                            {
                                //e.Accepted = true;
                                return true;
                            }
                        }
                    }
                    i++;
                }
            }

            // No match found across columns
            //e.Accepted = false;
            return false;
        }

        private (string, bool, Type?, object?) UpdateProperty(object dataItem, string propertyName, string value)
        {
            var property = dataItem.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                try
                {
                    var prevValue = property.GetValue(dataItem);
                    var convertedValue = Convert.ChangeType(value, property.PropertyType);
                    //Debug.WriteLine($"{prevValue} | {convertedValue}");
                    if (!((object?)prevValue as object)?.Equals((object?)convertedValue as object) ?? true)
                    {
                        property.SetValue(dataItem, convertedValue);
                        return (propertyName, true, property.PropertyType, convertedValue);
                    } else
                    {
                        return (propertyName, false, property.PropertyType, null);
                    }
                }
                catch (Exception ex)
                {
                    // Handle conversion errors
                    System.Diagnostics.Debug.WriteLine($"Error updating property: {ex.Message}");
                    return (propertyName, false, property.PropertyType, null);
                }
            }
            return (propertyName, false, null, null);
        }

        public bool UpdateItemLineData(Guid manufid, example_queries_GetItemLinesByJobResult item)
        {
            if (item is null) return false;
            var res = Ext.Queries.SetItemLineByJobAndManufID(
                item.PartNbr, item.ItemNbr, item.IDNbr, item.CatalogNbr, item.Qty, item.Type,
                item.SubType, item.Description, item.UofM, item.ItemFin, item.ItemCore, item.ColorBy,
                item.Dept, item.WorkCtr, item.ScrapFactor, item.SizeDivisor, item.Depth, item.Width, item.Fabwidth,
                item.Height, item.FabHeight, item.Assembled, item.AssyNbr, item.TileIndicator, item.Explode,
                item.Option01, item.Option02, item.Option03, item.Option04, item.Option05, item.Option06,
                item.Option07, item.Option08, item.Option09, item.Option10, item.Usertag1, item.CoreSize,
                item.Multiplier, item.Area, item.JobNbr, manufid
            );
            if (res is null) return false;
            return true;
        }

        public bool CopyItemLineData(Guid manufid, example_queries_GetItemLinesByJobResult item)
        {
            if (item is null) return false;
            var res = Ext.Queries.CopyManufItemLineByManufID(" (COPY)", manufid);
            if (res is null) return false;
            return true;
        }

        public bool DeleteItemLine(Guid manufid, string job)
        {
            var res = Ext.Queries.DeleteManufItemLineByManufID(job, manufid);
            if (res is null) return false;
            return true;
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

        public async Task RunExternal(ProcessStartInfo startInfo)
        {
            //MessageBox.Show(startInfo.Arguments);
            var proc = Process.Start(startInfo);
            if (proc is null) return;
            proc.ErrorDataReceived += (ss, ee) =>
            {
                Debug.WriteLine("ERR:\n" + ee.Data);
            };
            proc.OutputDataReceived += (ss, ee) =>
            {
                Debug.WriteLine("OUT:\n" + ee.Data);
            };
            proc.Exited += (ss, ee) =>
            {

            };
            CancellationToken t = new();
            await proc.WaitForExitAsync(t);
            proc.Dispose();
        }
        #endregion


        #region EventHandlers

        private void MainViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not "FontSize_Base") return;
            OnPropertyChanged(nameof(DataGridFontSize));
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton) return;
            if (this.JobNbr is null) return;
            Debug.WriteLine("Load Job Data");
            this.LoadDataForJob(this.JobNbr);
        }

        private void EngOrder_JobNbrChanged(object? sender, string e)
        {
            this.ResetUI();
            this.LoadDataForJob(e);
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = new TextBlock()
            {
                Text = e.Row.GetIndex().ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = 32 - 8 - 1,
                TextAlignment = TextAlignment.Right,
                Margin = new (0),
                Padding = new (0)
            };
        }

        public void DataGrid_IceManuf_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() is not string headerName)
            {
                return;
            }
            if (this.DataGrid_IceManuf_ColumnsOrder.Contains(headerName))
            {
                e.Column.DisplayIndex = this.DataGrid_IceManuf_ColumnsOrder.IndexOf(headerName);
            }
            if (this.DataGrid_IceManuf_ColumnsExcludedHidden.Contains(headerName)) {
                e.Cancel = true;
            }
            e.Column.Visibility =
                this.DataGrid_IceManuf_ColumnsExcludedHidden.Contains(headerName) ?
                Visibility.Collapsed :
                Visibility.Visible;
            e.Column.IsReadOnly = this.DataGrid_IceManuf_ColumnsReadonly.Contains(headerName);
        }

        private void dataSideGridScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            //this.datagrid_side.AddHandler(MouseWheelEvent, new RoutedEventHandler(DataGridMouseWheelHorizontal), true);
        }

        private void datagrid_main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Enter)
            {
                if (this.pnl_dock.Visibility is Visibility.Collapsed)
                    ToggleSideGrid();
                e.Handled = true;
                var colIdx = this.datagrid_main.CurrentCell.Column.DisplayIndex;
                this.pnl_dock.Focus();
                //this.grid_dataeditregion.Focus();
                this.WPnl_DataEditRegion.Focus();
                //if (this.grid_dataeditregion.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
                if (this.WPnl_EditInputs.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
                Debug.WriteLine(colIdx);
                if (txts.ElementAt(colIdx) is not TextBox txt) return;
                txt.Focus();
                txt.Select(txt.Text.Length, 0);
                return;
            }
            if (e.Key is Key.F && (Keyboard.Modifiers & ModifierKeys.Control) is ModifierKeys.Control)
            {
                this.ToggleFiltersPanel();
                e.Handled = true;
            }
        }

        private void Page_EngOrder_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.F && (Keyboard.Modifiers & ModifierKeys.Control) is ModifierKeys.Control)
            {
                this.ToggleFiltersPanel();
                e.Handled = true;
            }
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
            var viewSource = (CollectionViewSource)Resources["MfgItemsViewSource"];
            viewSource?.View?.Refresh();
        }

        private void MfgItemsViewSource_Filter(object sender, FilterEventArgs e)
        {
            // Assuming 'MyDataItem' is the type of objects in collection
            if (e.Item is not example_queries_GetItemLinesByJobResult item)
            {
                return;
            }

            // Get text from TextBox
            var filterText = Txt_Filter.Text.ToLower();
            // If the filter text is empty, accept all items
            if (filterText is null || string.IsNullOrWhiteSpace(filterText))
            {
                //e.Accepted = true;
                e.Accepted = true;
                return;
            }
            e.Accepted = this.MfgItems_Filter(item, filterText);            
        }

        private void Btn_FilterClose_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleFiltersPanel();
            e.Handled = true;
        }

        public static readonly DependencyProperty Pending_LineChangesProperty =
            DependencyProperty.Register(
                "Pending_LineChanges", typeof(bool), typeof(EngOrder),
                new PropertyMetadata(false)
            );

        private void datagrid_main_Loaded(object sender, RoutedEventArgs e)
        {
            //this.grid_dataeditregion.Children.Clear();
            //this.grid_dataeditregion.RowDefinitions.Clear();
            this.WPnl_EditLabels.Children.Clear();
            this.WPnl_EditInputs.Children.Clear();
            if (typeof(example_queries_GetItemLinesByJobResult).GetProperties() is not PropertyInfo[] props) return;
            if (props.Length is 0) return;
            List<string> cols = [.. this.datagrid_main.Columns.OrderBy(c => c.DisplayIndex).Select(c => c.Header.ToString())];
            props = [.. props.OrderBy(p => cols.IndexOf(p.Name))];
            var rowIdx = 0;
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i] is not PropertyInfo prop) return;
                if (this.DataGrid_IceManuf_ColumnsExcludedHidden.Contains(prop.Name)) continue;
                var lbl = new Label()
                {
                    Width = 150,
                    Content = prop.Name,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Height = 30,
                };
                var txt = new TextBox()
                {
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    IsReadOnly = this.DataGrid_IceManuf_ColumnsReadonly.Contains(prop.Name),
                    Visibility = Visibility.Visible,
                    Width = 414 - 150 - 5,
                    Height = 30,
                };
                /*
                Binding fontSizeBind = new("FontSize_Base")
                {
                    Path = new("MainViewModel.FontSize_Base"),
                    Source = this,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay,
                    TargetNullValue = 22d,
                    FallbackValue = 22d,
                    Converter = (ScalingConverter)Resources["ScalingConverter1"],
                    ConverterParameter = 5
                };
                txt.SetBinding(TextBox.HeightProperty, fontSizeBind);
                lbl.SetBinding(Label.HeightProperty, fontSizeBind);
                */
                Binding bind = new(prop.Name)
                {
                    Path = new PropertyPath($"SelectedItem.{prop.Name}"),
                    Source = this.datagrid_main,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay, TargetNullValue = "", FallbackValue = ""
                };
                //var datatrigger = new DataTrigger() { Binding = bind, Value = null };
                //datatrigger.Setters.Add(new Setter(TextBox.VisibilityProperty, Visibility.Collapsed));

                var defaultStyle = (Style)FindResource(typeof(TextBox));
                Style style = new(typeof(TextBox));
                if (defaultStyle is not null)
                {
                    style.BasedOn = defaultStyle;
                }
                //style.Triggers.Add(datatrigger);
                txt.Style = style;
                txt.PreviewKeyDown += this.Txt_PreviewKeyDown;
                txt.SetBinding(TextBox.TextProperty, bind);
                //this.grid_dataeditregion.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32, GridUnitType.Pixel) });
                //this.grid_dataeditregion.Children.Add(lbl);
                //this.grid_dataeditregion.Children.Add(txt);
                this.WPnl_EditLabels.Children.Add(lbl);
                this.WPnl_EditInputs.Children.Add(txt);
                /*
                Grid.SetRow(lbl, rowIdx);
                Grid.SetRow(txt, rowIdx);
                Grid.SetColumn(lbl, 0);
                Grid.SetColumn(txt, 1);
                */
                rowIdx++;
            }
        }

        private void datagrid_main_Unloaded(object sender, RoutedEventArgs e)
        {
            //this.grid_dataeditregion.Children.Clear();
            //this.grid_dataeditregion.RowDefinitions.Clear();
        }

        private void Txt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            this.Pending_LineChanges = true;
        }

        private void Btn_AcceptItemLineEdits_Click(object sender, RoutedEventArgs e)
        {
            if (Ext.PopupConfirmation("Accept changes made to item line? This action cannot be undone.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) is not MessageBoxResult.Yes) return;
            //if (this.grid_dataeditregion.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
            if (this.WPnl_EditInputs.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
            List<(string, bool, Type?, object?)> changes = [];
            if (this.datagrid_main.SelectedItem is not example_queries_GetItemLinesByJobResult line) return;
            foreach (var item in txts)
            {
                var binding = item.GetBindingExpression(TextBox.TextProperty);
                var propertyName = binding.ParentBinding.Path.Path.Split('.').Last();
                if (propertyName is null ||
                    this.DataGrid_IceManuf_ColumnsExcludedHidden.Contains(propertyName) ||
                    this.DataGrid_IceManuf_ColumnsReadonly.Contains(propertyName))
                {
                    continue;
                }
                this.datagrid_main.BeginEdit();
                var propRes = UpdateProperty(line, propertyName, item.Text);
                this.datagrid_main.CommitEdit();
                if (propRes.Item2 is true)
                {
                    changes.Add(propRes);
                }
                // Trigger Cell & Row Edit events
                /*
                item.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode = BindingMode.OneWayToSource;
                item.Text = item.Text;
                item.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode = BindingMode.OneWay;
                */
            }
            if (changes.Count is 0) return;
            this.UpdateItemLineData(line.IceManufID, line);
            Debug.WriteLine(string.Join("\n", changes.Select(c => $"{c.Item1}: {c.Item3} = {c.Item4}")));
            this.Pending_LineChanges = false;
        }

        private void Btn_RevertItemLineEdits_Click(object sender, RoutedEventArgs e)
        {
            if (Ext.PopupConfirmation("Discard changes made to item line? All unsaved changes will be lost.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) is not MessageBoxResult.Yes) return;
            //if (this.grid_dataeditregion.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
            if (this.WPnl_EditInputs.Children.OfType<TextBox>() is not IEnumerable<TextBox> txts) return;
            foreach (var item in txts)
            {
                var binding = item.GetBindingExpression(TextBox.TextProperty);
                //var propertyName = binding.ParentBinding.Path.Path.Split('.').Last();
                binding.UpdateTarget();
                // Trigger Cell & Row Edit events
                /*
                item.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode = BindingMode.OneWayToSource;
                item.Text = item.Text;
                item.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode = BindingMode.OneWay;
                */
            }
            this.Pending_LineChanges = false;
        }

        private void Btn_DeleteItemLine_Click(object sender, RoutedEventArgs e)
        {
            /*
            Ext.PopupConfirmation("Astr", "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            Ext.PopupConfirmation("Errr", "", MessageBoxButton.YesNo, MessageBoxImage.Error);
            Ext.PopupConfirmation("Excl", "", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            Ext.PopupConfirmation("Hand", "", MessageBoxButton.YesNo, MessageBoxImage.Hand);
            Ext.PopupConfirmation("Info", "", MessageBoxButton.YesNo, MessageBoxImage.Information);
            Ext.PopupConfirmation("None", "", MessageBoxButton.YesNo, MessageBoxImage.None);
            Ext.PopupConfirmation("Ques", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            Ext.PopupConfirmation("Stop", "", MessageBoxButton.YesNo, MessageBoxImage.Stop);
            Ext.PopupConfirmation("Warn", "", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            */
            if (Ext.PopupConfirmation("Are you sure you want to delete this item line? This action cannot be undone.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Stop) is not MessageBoxResult.Yes) return;
            if (this.datagrid_main.SelectedItem is not example_queries_GetItemLinesByJobResult line) return;
            var res = this.DeleteItemLine(line.IceManufID, line.JobNbr);
            if (res)
                this.MfgItemLines.Remove(line);
            else
                MessageBox.Show("Deletion failed");
        }

        private void Btn_NewItemLine_Click(object sender, RoutedEventArgs e)
        {
            if (Ext.PopupConfirmation("Are you sure you'd like to create a new line using the current line's values? New line will have \"(COPY)\" append to the end of its description", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) is not MessageBoxResult.Yes) return;
            if (this.datagrid_main.SelectedItem is not example_queries_GetItemLinesByJobResult line) return;
            this.CopyItemLineData(line.IceManufID, line);
            if (this.JobNbr is null) return;
            this.LoadManufData(this.JobNbr);
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            /*
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Cell changes are committed automatically with ObservableCollection
                // track changes here if needed
                //TrackChange(e.Row.Item);
                Debug.WriteLine((e.Row.Item as example_queries_GetItemLinesByJobResult).ItemNbr);
            }
            */
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Row changes committed
            }
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

        private void Btn_FilterClose_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private async void LabelInputLookupPair_LookupInputChanged(object sender, LabelInputLookupPair.Lookup_EventArgs e)
        {
            if (e.Lookup is "00000000-0000-0000-0000-000000000000") return;
            if (SCH.Global.Config is null || !SCH.Global.Config.InitializationSuccessfull) return;
            Debug.WriteLine($"Try Query GUID lookup ({e.Lookup})");
            var dbcon1 = new System.Data.Odbc.OdbcConnection($"Driver={{SQL Server}};Server={SCH.Global.Config["sql.servers.OldCRM"]};Database={SCH.Global.Config["sql.databases.OldCRM"]};DSN={SCH.Global.Config["sql.databases.OldCRM"]};Trusted_Connection=Yes;Integrated Security=SSPI;");
            var cmd1 = dbcon1.CreateCommand();
            cmd1.CommandText = $"SELECT TOP 1 PartID, PartDescription    FROM eCRM_intcon2.dbo.aIC_Product    WHERE (ItemID = '{e.Lookup}')";
            cmd1.CommandTimeout = 10000;
            cmd1.Connection = dbcon1;
            try
            {
                await dbcon1.OpenAsync();
                var reader = await cmd1.ExecuteReaderAsync();
                List<Dictionary<string, object>> res = [];
                while (await reader.ReadAsync())
                {
                    Debug.WriteLine("\tRow");
                    Dictionary<string, object> obj = [];
                    for (int fieldIndex = 0; fieldIndex < reader.FieldCount; fieldIndex++)
                    {
                        obj.Add(reader.GetName(fieldIndex), reader.GetValue(fieldIndex));
                    }
                    res.Add(obj);
                }
                Debug.WriteLine("Query GUID lookup");
                if (res is null || res.Count is 0 || res[0] is not Dictionary<string, object> item)
                {
                    e.Source.InputLookupValue = "-";
                    return;
                }
                e.Source.InputLookupValue = $"{item["PartID"]} - {item["PartDescription"]}" ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error\n" + ex.Message + "\n" + ex.StackTrace, "Error Encountered", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            await cmd1.DisposeAsync();
            await dbcon1.CloseAsync();
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
            using var lookup = new LookupFinder(this.MainViewModel)
            {
                MainVM = this.MainViewModel,
                Owner = Ext.MainWindow
            };
            await lookup.LookupMaterials(partFilter, descFilter, partConstraint, descConstraint);
            if (lookup.ShowDialog() is not true || lookup.ReturnObject is not Dictionary<string, object> obj) return;
            e.Source.InputLookup = obj["ItemID"]?.ToString() ?? "-";
            //MessageBox.Show(lookup.ReturnObject["ItemID"].ToString());
        }

        private async void Btn_RefreshHeader_Click(object sender, RoutedEventArgs e)
        {
            if (this.JobNbr is null) return;
            this.Btn_RefreshHeader.IsEnabled = false;
            await this.LoadColorSetData(this.JobNbr);
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
            if (this.ColorSetInfo.ColorSetID.ToString().Length < 8) return;
            var dbcon1 = new System.Data.Odbc.OdbcConnection($"Driver={{SQL Server}};Server={SCH.Global.Config["sql.servers.OldCRM"]};Database={SCH.Global.Config["sql.databases.OldCRM"]};DSN={SCH.Global.Config["sql.databases.OldCRM"]};Trusted_Connection=Yes;Integrated Security=SSPI;");
            var cmd1 = dbcon1.CreateCommand();
            var changes = this.ColorSetInfo_Changes.ToArray();
            cmd1.CommandText =
                @$"
                    UPDATE  eCRM_intcon2.dbo.[aIC_ColorSet]
                    SET     {string.Join(",", changes.Select(c => $"[{c.Key}]=?"))}
                    WHERE   (ColorSetID = '{ColorSetInfo.ColorSetID}');
                ".Trim();
            cmd1.CommandTimeout = 10000;
            cmd1.Connection = dbcon1;
            foreach (var item in changes)
            {
                cmd1.Parameters.AddWithValue($"@{item.Key}", item.Value ?? (object)DBNull.Value);
            }
            Debug.WriteLine(cmd1.CommandText);
            this.ColorSetInfo_Changes.Clear();
            //return;
            try
            {
                await dbcon1.OpenAsync();
                var res = await cmd1.ExecuteNonQueryAsync();
                MessageBox.Show(res.ToString());
            } catch
            {

            }
        }

        private async void Btn_ExtProc_ImpMfg_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (System.IO.Directory.Exists($"C:/{JobNbr}") is false)
            {
                MessageBox.Show($"Folder 'C:/{JobNbr}' not found", "Local file not found", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            */
            if (this.ExternalProc_GroupActive["stdEngProcs"] is true)
                return;
            else
                this.ExternalProc_GroupActive["stdEngProcs"] = true;

            this.ResetUI();
            ProcessStartInfo psi = new(
                extProc_ImportEngMfg_exe,
                extProc_ColorSet_arg.Replace(
                    "{@ColorSetID}",
                    this.ColorSetInfo.ColorSetID.ToString().ToUpper()
                )
            );
            ((Button)sender).BorderBrush = Brushes.Goldenrod;
            await RunExternal(psi);
            ((Button)sender).BorderBrush = Brushes.ForestGreen;
            this.ExternalProc_GroupActive["stdEngProcs"] = false;
        }

        private async void Btn_ExtProc_EngPrc_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExternalProc_GroupActive["stdEngProcs"] is true)
                return;
            else
                this.ExternalProc_GroupActive["stdEngProcs"] = true;

            ProcessStartInfo psi = new(
                extProc_ProcEngMfg_exe,
                extProc_ColorSet_arg.Replace(
                    "{@ColorSetID}",
                    this.ColorSetInfo.ColorSetID.ToString().ToUpper()
                )
            );
            ((Button)sender).BorderBrush = Brushes.Goldenrod;
            await RunExternal(psi);
            ((Button)sender).BorderBrush = Brushes.ForestGreen;
            this.ExternalProc_GroupActive["stdEngProcs"] = false;
        }
        #endregion

        private async void Btn_ExtProc_Matl_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExternalProc_GroupActive["stdEngProcs"] is true)
                return;
            else
                this.ExternalProc_GroupActive["stdEngProcs"] = true;

            ProcessStartInfo psi = new(
                extProc_CalcEngMatl_exe,
                extProc_ColorSet_arg.Replace(
                    "{@ColorSetID}",
                    this.ColorSetInfo.ColorSetID.ToString().ToUpper()
                )
            );
            ((Button)sender).BorderBrush = Brushes.Goldenrod;
            await RunExternal(psi);
            ((Button)sender).BorderBrush = Brushes.ForestGreen;
            this.ExternalProc_GroupActive["stdEngProcs"] = false;
        }

        private async void Btn_ExtProc_Cutlst_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExternalProc_GroupActive["stdEngProcs"] is true)
                return;
            else
                this.ExternalProc_GroupActive["stdEngProcs"] = true;

            ProcessStartInfo psi = new(
                extProc_CncCutList_exe,
                extProc_ColorSet_arg.Replace(
                    "{@ColorSetID}",
                    this.ColorSetInfo.ColorSetID.ToString().ToUpper()
                )
            );
            ((Button)sender).BorderBrush = Brushes.Goldenrod;
            await RunExternal(psi);
            ((Button)sender).BorderBrush = Brushes.ForestGreen;
            this.ExternalProc_GroupActive["stdEngProcs"] = false;
        }

        private async void Btn_ExtProc_SlExp_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExternalProc_GroupActive["stdEngProcs"] is true)
                return;
            else
                this.ExternalProc_GroupActive["stdEngProcs"] = true;

            ProcessStartInfo psi = new(
                extProc_SymEngExp_exe,
                extProc_ColorSet_arg.Replace(
                    "{@ColorSetID}",
                    this.ColorSetInfo.ColorSetID.ToString().ToUpper()
                )
            );
            ((Button)sender).BorderBrush = Brushes.Goldenrod;
            await RunExternal(psi);
            ((Button)sender).BorderBrush = Brushes.ForestGreen;
            this.ExternalProc_GroupActive["stdEngProcs"] = false;
        }
    }
}
