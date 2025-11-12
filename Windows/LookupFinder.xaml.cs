using OMPS.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Odbc;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
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
    public partial class LookupFinder : Window, IDisposable
    {
        public static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }
        internal static MainWindow ParentWindow { get => Ext.MainWindow; }
        public ObservableCollection<object> Data { get; set; } = [];

        public Dictionary<string, object?>? ReturnObject = null;

        public LookupFinder()
        {
            InitializeComponent();
            this.DataContext = this;
            //
            //this.LookupMaterials("%PLW%");
        }

        public async Task LookupItem(string? partFilter = null, string? descFilter = null, string defaultPartContraint = "%", string defaultDescConstraint = "%")
        {
            this.LastLookup = this.LookupItem;
            await Lookup(
                partFilter, descFilter,
                defaultPartContraint, defaultDescConstraint,
                "PartID asc",
                ["PartID", "PartDescription", "PartAdditionalDescription", "SellingUOM as UofM", "ProductCategory", "CreateDateTime", "ModifyDateTime"]
             );
        }

        public async Task LookupMaterials(string? partFilter = null, string? descFilter = null, string defaultPartContraint = "%", string defaultDescConstraint = "%")
        {
            this.LastLookup = this.LookupMaterials;
            await Lookup(
                partFilter, descFilter,
                defaultPartContraint, defaultDescConstraint,
                "PartDescription asc",
                ["PartID", "PartDescription"]
            );
        }

        public int Total = -1;
        public string[] LastLookupArgs = [];
        public Func<string, string, string, string, Task>? LastLookup = null;

        public async Task Lookup(string? partFilter = null, string? descFilter = null, string defaultPartContraint = "%", string defaultDescConstraint = "%", string orderBy = "PartID asc", string[]? fields = null, ushort limit = 25)
        {
            if (fields is null) return;
            this.LastLookupArgs = [
                partFilter ?? "", descFilter ?? "",
                defaultPartContraint, defaultDescConstraint
            ];
            this.Txt_FilterPart.Text = partFilter ?? "";
            this.Txt_FilterDesc.Text = descFilter ?? "";
            if (Total is -1)
            {
                var resTotal = await Query(
                    $@"
                       SELECT   COUNT(*) as TOTAL
                       FROM     eCRM_intcon2.dbo.aIC_Product
                       WHERE    (PartID like '{defaultPartContraint}' AND PartID like '{(partFilter is null ? "%" : $"%{partFilter}%")}') AND
                                (PartDescription like '{defaultDescConstraint}' AND PartDescription like '{(descFilter is null ? "%" : $"%{descFilter}%")}')
                    "
                    );
                if (resTotal is null) return;
                if (resTotal.FirstOrDefault() is not Dictionary<string, object?> first) return;
                if (first.TryGetValue("TOTAL", out object? val) is false || val is not int valInt) return;
                this.Total = valInt;
            }
            var res = await Query(
                @$"SELECT TOP {limit} ItemID, {string.Join(", ", fields)}
                       FROM eCRM_intcon2.dbo.aIC_Product
                       WHERE (PartID like '{defaultPartContraint}' AND PartID like '{(partFilter is null ? "%" : $"%{partFilter}%")}') AND
                             (PartDescription like '{defaultDescConstraint}' AND PartDescription like '{(descFilter is null ? "%" : $"%{descFilter}%")}')
                       ORDER BY {orderBy}
                    "
                );
            if (res is null) return;
            this.Data.Clear();
            this.DataGrid_Lookup.Columns.Clear();
            for (ushort i = 0; i < res.Count && i <= ushort.MaxValue; i++)
            {
                //Debug.WriteLine(i);
                Data.Add(new BindableDynamicDictionary(res[i]));
            }
            //this.DataGrid_Lookup.ItemsSource = Data;
            this.DataGrid_AutoGeneratingColumn(this.DataGrid_Lookup, null);
            this.Lbl_datagrid_count.Content = $"Showing {(res.Count < limit ? res.Count : limit)} of {this.Total} items";
        }

        public static async Task<List<Dictionary<string, object?>>?> Query(string sql)
        {
            var (con, cmd) =
                await EstablishConnection(
                    $"Driver={{SQL Server}};Server={SCH.Global.Config["sql.servers.OldCRM"]};Database={SCH.Global.Config["sql.databases.OldCRM"]};DSN={SCH.Global.Config["sql.databases.OldCRM"]};Trusted_Connection=Yes;Integrated Security=SSPI;",
                    sql
                );
            if (con is null || cmd is null) return null;
            return await RetrieveData(cmd) ?? [];
        }

        public static async Task<(OdbcConnection?, OdbcCommand?)> EstablishConnection(string constr, string sql)
        {
            try
            {
                var dbcon1 = new OdbcConnection(constr);
                var cmd1 = dbcon1.CreateCommand();
                cmd1.CommandText = sql;
                cmd1.CommandTimeout = 10000;
                cmd1.Connection = dbcon1;
                await dbcon1.OpenAsync();
                return (dbcon1, cmd1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return (null, null);
        }


        public static async Task<List<Dictionary<string, object?>>> RetrieveData(OdbcCommand cmd)
        {
            var reader = await cmd.ExecuteReaderAsync();
            List<Dictionary<string, object?>> res = [];
            while (await reader.ReadAsync())
            {
                Dictionary<string, object?> obj = [];
                for (int fieldIndex = 0; fieldIndex < reader.VisibleFieldCount; fieldIndex++)
                {
                    obj.Add(reader.GetName(fieldIndex), reader.GetValue(fieldIndex));
                }
                res.Add(obj);
            }
            if (res is null)
            {
                return [];
            }
            return res;
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not BindableDynamicDictionary dict) return;
            var passed = true;
            if (!string.IsNullOrWhiteSpace(this.Txt_FilterPart.Text))
                passed &= (dict["PartID"]?.ToString()?.Contains(this.Txt_FilterPart.Text, StringComparison.OrdinalIgnoreCase) ?? false);
            if (!string.IsNullOrWhiteSpace(this.Txt_FilterDesc.Text))
            passed &= (dict["PartDescription"]?.ToString()?.Contains(this.Txt_FilterDesc.Text, StringComparison.OrdinalIgnoreCase) ?? false);
            e.Accepted = passed;
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs? e)
        {
            //Debug.WriteLine("$$$");
            //Debug.WriteLine(1);
            if (sender is not DataGrid dg) return;
            //Debug.WriteLine(2);
            if (dg.ItemsSource.Cast<object>().FirstOrDefault() is not DynamicObject first) return;
            //Debug.WriteLine(3);
            if (first.GetDynamicMemberNames() is not IEnumerable<string> names) return;
            //Debug.WriteLine(4);
            foreach (var name in names)
            {
                var isShown = (name is not "ItemID");
                var colWidth = name switch
                {
                    "PartID" => new DataGridLength(2, DataGridLengthUnitType.Star),
                    _ => new DataGridLength(5, DataGridLengthUnitType.Star)
                };
                //Debug.WriteLine("+");
                dg.Columns.Add(new DataGridTextColumn {
                    Header = name,
                    Binding = new Binding(name),
                    Width = colWidth,
                    Visibility = isShown ? Visibility.Visible : Visibility.Collapsed,
                });
            }
        }

        private void Txt_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key is not System.Windows.Input.Key.Enter) return;
            /*
            var viewSource = (CollectionViewSource)Resources["DataViewSource"];
            viewSource?.View?.Refresh();
            */
            this.LastLookup?.DynamicInvoke(
                this.Txt_FilterPart.Text, this.Txt_FilterDesc.Text,
                this.LastLookupArgs[2], this.LastLookupArgs[3]
            );
        }

        public void Dispose()
        {
            Data.Clear();
            GC.SuppressFinalize(this);
        }

        private void DataGrid_Lookup_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.DataGrid_Lookup.SelectedItem is null) return;
            if ((BindableDynamicDictionary)this.DataGrid_Lookup.SelectedItem is not BindableDynamicDictionary bdd) return;
            this.ReturnObject = bdd.Source();
            this.DialogResult = true;
        }

        private void LookupFinderWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.ReturnObject is not null) return;
            this.DialogResult = false;
        }

        private void DataGrid_Lookup_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Padding = new Thickness(0);
        }
    }

    /// <summary>
    /// Bindable dynamic dictionary.
    /// </summary>
    public sealed class BindableDynamicDictionary : DynamicObject, INotifyPropertyChanged
    {
        /// <summary>
        /// The internal dictionary.
        /// </summary>
        private readonly Dictionary<string, object?> _dictionary;

        /// <summary>
        /// Creates a new BindableDynamicDictionary with an empty internal dictionary.
        /// </summary>
        public BindableDynamicDictionary()
        {
            _dictionary = new Dictionary<string, object?>();
        }

        /// <summary>
        /// Copies the contents of the given dictionary to initilize the internal dictionary.
        /// </summary>
        /// <param name="source"></param>
        public BindableDynamicDictionary(IDictionary<string, object?> source)
        {
            _dictionary = new Dictionary<string, object?>(source);
        }

        public Dictionary<string, object?> Source()
            => this._dictionary;

        /// <summary>
        /// You can still use this as a dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object? this[string key]
        {
            get
            {
                return _dictionary[key];
            }
            set
            {
                _dictionary[key] = value;
                RaisePropertyChanged(key);
            }
        }

        /// <summary>
        /// This allows you to get properties dynamically.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            return _dictionary.TryGetValue(binder.Name, out result);
        }

        /// <summary>
        /// This allows you to set properties dynamically.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            _dictionary[binder.Name] = value;
            RaisePropertyChanged(binder.Name);
            return true;
        }

        /// <summary>
        /// This is used to list the current dynamic members.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dictionary.Keys;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            var propChange = PropertyChanged;
            if (propChange == null) return;
            propChange(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
