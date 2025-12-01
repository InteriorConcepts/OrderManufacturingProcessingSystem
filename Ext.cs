using Humanizer;
using Microsoft.EntityFrameworkCore;
using MyApp.DataAccess.Generated;
using OMPS.DBModels.Order;
using OMPS.Pages;
using OMPS.ViewModels;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using static OMPS.Pages.EngOrder;
using static System.Net.WebRequestMethods;


namespace OMPS
{
    public enum PageTypes
    {
        None = -1,
        Home = 0,
        Login = 1,
        OrderSearch = 2,
        EngOrder = 3,
        QuoteOrder = 4,
        ProductCatalogSearch = 5,
        ProductCatalogDetails = 6
    }

    public static class Ext
    {

        public static Color AccentColor = SystemParameters.WindowGlassColor;
        public static SolidColorBrush AccentColorBrush => new SolidColorBrush(AccentColor);
        public static SolidColorBrush AccentColorBrush50 => new SolidColorBrush(Color.FromArgb(127, AccentColor.R, AccentColor.G, AccentColor.B));

        #region "App.config"
        public enum AppConfigKey
        {
            FontSize,
            Notify_local,
            Notify_engcheck1,
            Notify_engcheck2,
            Notify_engrelease,
            Notify_cncwks,
            Notify_cncpnl,
            EngOrder_VertBorder,
            Shortcuts,
        }
        internal static void ValidateAppSettings()
        {
            try
            {
                NameValueCollection? appSettings = ConfigurationManager.AppSettings;
                if (appSettings is null)
                {
                    MessageBox.Show("AppSettings not loaded correctly.");
                    return;
                }
                if (appSettings.Count is 0)
                {
                    MessageBox.Show("AppSettings is empty.");
                    return;
                }
#if false
                for (byte i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
                {
                    var cs = ConfigurationManager.ConnectionStrings[i];
                    MessageBox.Show("Key: {0} Value: {1}".FormatWith(cs.Name, cs.ConnectionString));
                }
                foreach (var key in appSettings.AllKeys)
                {
                    MessageBox.Show("Key: {0} Value: {1}".FormatWith(key, appSettings[key]));
                }
#endif
            }
            catch (ConfigurationErrorsException)
            {
                MessageBox.Show("Error reading AppSettings");
            }
        }

        internal static (bool success, T? value) ReadSetting<T>(AppConfigKey key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings[key.ToString()] is not string str)
                {
                    return (false, default(T));
                }
                else
                {
                    if (Convert.ChangeType(str, typeof(T)) is not object converted || converted is null)
                    {
                        return (false, default(T));
                    }
                    return (true, (T)converted);
                }
            }
            catch (ConfigurationErrorsException)
            {
                return (false, default(T));
            }
        }
        internal static (bool success, string value) ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings[key] is not string str)
                {
                    return (false, "");
                }
                else
                    return (true, str);
            }
            catch (ConfigurationErrorsException)
            {
                return (false, "Error reading app settings");
            }
        }

        internal static (bool success, bool value) ReadSettingAsBool(string key)
        {

            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "false";
                if (appSettings[key] is not string str)
                {
                    return (false, false);
                }
                else
                    return (true, bool.TryParse(str, out bool val) ? val : false);
            }
            catch (ConfigurationErrorsException)
            {
                return (false, false);
            }
        }

        internal static (bool success, double value) ReadSettingAsDouble(string key)
        {

            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "false";
                if (appSettings[key] is not string str)
                {
                    return (false, 0.0);
                }
                else
                    return (true, double.TryParse(str, out double val) ? val : 0.0);
            }
            catch (ConfigurationErrorsException)
            {
                return (false, 0.0);
            }
        }

        internal static (bool success, int value) ReadSettingAsInt(string key)
        {

            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "false";
                if (appSettings[key] is not string str)
                {
                    return (false, -1);
                }
                else
                    return (true, int.TryParse(str, out int val) ? val : -1);
            }
            catch (ConfigurationErrorsException)
            {
                return (false, -1);
            }
        }

        internal static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                MessageBox.Show("Error writing AppSettings");
            }
        }
        #endregion


        #region "Files / Directories"
        internal static IEnumerable<FileInfo> MultiEnumerateFiles(this DirectoryInfo di, string patterns, SearchOption sOpts = SearchOption.TopDirectoryOnly)
        {
            foreach (var pattern in patterns.Split('|'))
                foreach (var file in di.EnumerateFiles(pattern, sOpts))
                    yield return file;
        }

        internal static IEnumerable<DirectoryInfo> MultiEnumerateDirectories(this DirectoryInfo di, string patterns, SearchOption sOpts = SearchOption.TopDirectoryOnly)
        {
            foreach (var pattern in patterns.Split('|'))
                foreach (var file in di.EnumerateDirectories(pattern, sOpts))
                    yield return file;
        }
        internal static IEnumerable<DirectoryInfo> JobFoldersC { get => new DirectoryInfo("C:\\").MultiEnumerateDirectories("*", SearchOption.TopDirectoryOnly); }
        internal static IEnumerable<DirectoryInfo> JobFoldersH { get => new DirectoryInfo("H:\\Engineering").MultiEnumerateDirectories("*", SearchOption.AllDirectories); }

        public static string TruncatePath(string path, ushort maxLength = 20)
        {
            string[] splits = [.. path.Split("\\").SkipLast(1)];
            var count = (ushort)splits.Sum(s => s.Length);
            if (count <= maxLength)
            {
                return path;
            }
            var start = new StringBuilder(255, 1024);
            ushort curStartLen = 0,
                   startSplitsUsed = 0;
            foreach (var split in splits)
            {
                if (curStartLen + split.Length > (maxLength))
                {
                    break;
                }
                if (start.Length is not 0)
                {
                    start.Append('\\');
                }
                start.Append(split);
                startSplitsUsed++;
                curStartLen += (ushort)split.Length;
            }
            if (splits.Length == startSplitsUsed)
            {
                return start.ToString();
            }
            else if (splits.Length - startSplitsUsed == 1)
            {
                return start.ToString() + "\\" + splits[^1];
            }
            else
            {
                return start + "\\...\\" + splits[^1];
            }
        }
        #endregion


        #region "Main Window"
        internal static MainWindow MainWindow;

        internal static void SetWindowTitle(string title)
        {
            if (string.IsNullOrEmpty(title.Trim()))
            {
                Ext.MainWindow.Title = $"{Ext.MainWindow.Title_Main_Prefix}";
            }
            else
            {
                Ext.MainWindow.Title = $"{Ext.MainWindow.Title_Main_Prefix} | {title}";
            }
        }

        /*
        // Redo so text isn't cut off with bigger font
        //  and use page enum to determine which tab element to modify.
        internal static void SetTabTitle(string title)
        {
            Ext.MainWindow.TabBtn_LineItemInfo.Content =
                Ext.MainWindow.TabBtn_LineItemInfo.Content.ToString()?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[0] +
                Environment.NewLine +
                title;
        }
        */
        #endregion


        #region "Main View Model"
        internal static Main_ViewModel MainViewModel = new();

        internal static void SetUrlRelPath(string path)
        {
            Ext.MainViewModel.SetUrlRelPath(path);
        }
        #endregion


        public const string StringFormat_Currency = "{}{0:C2.00}";
        public const string StringFormat_Text = "{}";
        /*
        internal static readonly UISettings UISettings = new();
        internal static bool IsDark()
        {
            var fg = UISettings.GetColorValue(UIColorType.Foreground);
            return (((5 * fg.G) + (2 * fg.R) + fg.B) > (8 * 128));
        }

#pragma warning disable WPF0001 
        internal static bool CurrentThemeIsDark() =>
            IsDark();
#pragma warning restore WPF0001
        */

        internal static readonly example_queriesQueries Queries = new();


        #region "Job Format"
        internal static bool IsJobNumValid(string job)
        {
            if (job[0] is not 'J' && job[0] is not 'S') return false;
            if (!job[1..].All(c => int.TryParse(c.ToString(), out int _))) return false;
            return true;
        }

        internal static void FormatJobNum(ref string job)
        {
            if (job.Length >= (job[0] is 'J' ? 6 : 5) && job.Length is not 10)
            {
                job = $"{job[0]}{job[(job.Length - 5)..(job.Length)].PadLeft(9, '0')}";
            }
        }
        #endregion


        #region "Storyboard / Animations"

        internal static T SetTarget<T>(this T anim, DependencyProperty prop, DependencyObject obj) where T : AnimationTimeline
        {
            return Ext.SetTarget(anim, new PropertyPath(prop), obj);
        }

        internal static T SetTarget<T>(this T anim, PropertyPath path, DependencyObject obj) where T : AnimationTimeline
        {
            _ = Ext.SetTargetObject(anim, obj);
            _ = Ext.SetTargetProperty(anim, path);
            return anim;
        }

        internal static T SetTargetObject<T>(this T anim, DependencyObject obj) where T : AnimationTimeline
        {
            Storyboard.SetTarget(anim, obj);
            return anim;
        }

        internal static T SetTargetProperty<T>(this T anim, PropertyPath path) where T : AnimationTimeline
        {
            Storyboard.SetTargetProperty(anim, path);
            return anim;
        }

        #endregion



        internal static PageTypes PageToType<T>(this T page) where T : UserControl
        {
            return page switch
            {
                Login => PageTypes.Login,
                OrderSearch => PageTypes.OrderSearch,
                EngOrder => PageTypes.EngOrder,
                QuoteOrder => PageTypes.QuoteOrder,
                _ => PageTypes.None,
            };
        }

        internal static MessageBoxResult PopupConfirmation(string text, string caption, MessageBoxButton btns, MessageBoxImage img)
        {
            return MessageBox.Show(text, caption, btns, img);
        }

        internal static DataTable ConvertListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            // Get properties of the class
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            dataTable.BeginLoadData();
            // Create columns in the DataTable based on class properties
            foreach (PropertyInfo prop in properties)
            {
                // Handle Nullable types
                Type? columnType = prop.PropertyType;
                if (columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = Nullable.GetUnderlyingType(columnType);
                }
                if (columnType is null) continue;
                dataTable.Columns.Add(prop.Name, columnType);
            }

            // Populate the DataTable with data from the list
            foreach (T item in items)
            {
                DataRow row = dataTable.NewRow();
                foreach (PropertyInfo prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value; // Handle null values
                }
                dataTable.Rows.Add(row);
            }
            dataTable.EndLoadData();

            return dataTable;
        }

        #region Filters

        public static readonly ImmutableList<string> DataGrid_Manuf_ColumnsExcludedHidden = ImmutableList.Create([
                "ManufId", "ColorSetId", "ProductId",
                "ProductLinkId", "ItemId", "JobNbr", "QuoteNbr", "CustOrderNbr",
                "BpartnerAvailable", "CustomerAvailable", "CreatedById",
                "ChangedbyId", "ChangebyIdOffline"
            ]).Select(s => s.ToLower()).ToImmutableList();
        public static readonly ImmutableList<string> DataGrid_Manuf_ColumnsReadonly = ImmutableList.Create([
                "QuoteNbr", "JobNbr", "CustOrderNbr",
                "Usertag1", "Multiplier", "Area", "CreationDate", "ChangeDate"
            ]).Select(s => s.ToLower()).ToImmutableList();
        public static readonly ImmutableList<string> DataGrid_Manuf_ColumnsOrder = ImmutableList.Create([
                "PartNbr", "ItemNbr", "CatalogNbr", "Qty", "Multiplier",
                "Description", "UofM", "Type", "SubType", "Idnbr", "Explode",
                "Assembled", "AssyNbr", "TileIndicator", "ItemFin", "ColorBy",
                "WorkCtr", "ItemCore", "Dept", "ScrapFactor", "SizeDivisor",
                "CoreSize", "Depth", "Width", "Height", "FabWidth", "FabHeight",
                "Option01", "Option02", "Option03", "Option04", "Option05",
                "Option06", "Option07", "Option08", "Option09", "Option10"
            ]).Select(s => s.ToLower()).ToImmutableList();

        public static readonly ImmutableList<PropertyInfo> MfgItem_FilterProps = [..
                typeof(DBModels.Order.AIcManuf).
                    GetProperties().
                    Where(p => p.PropertyType != typeof(Guid) &&
                                         p.PropertyType != typeof(DateTime) &&
                                         !DataGrid_Manuf_ColumnsExcludedHidden.Contains(p.Name.ToLower())
                    )
            ];
        public static string[] MfgItem_FilterGroups = [];
        public static string[][] MfgItem_GroupFilters = [];
        public static bool MfgItems_Filter_Desc(DBModels.Order.AIcManuf item, string filterText)
        {
            return ((item.Description ?? "").Contains(filterText, StringComparison.CurrentCultureIgnoreCase));
        }
        public static bool MfgItems_Filter(DBModels.Order.AIcManuf item, string filterText)
        {
            byte i = 0;
            for (byte j = 0; j < MfgItem_GroupFilters.Length; j++)
            {
                var group = MfgItem_GroupFilters[j];
                List<string> groupReqd = [.. group];
                for (byte k = 0; k < group.Length; k++)
                {
                    var filter = group[k];
                    if (!groupReqd.Contains(filter)) continue;
                    if (filter.Contains('='))
                    {
                        //Debug.WriteLine("PropName");
                        var split = filter.Split('=');
                        string pName = split[0],
                                pValue = split[1];
                        //Debug.WriteLine(pName);
                        if (item.GetType().GetProperty(pName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase) is not PropertyInfo pInfo) continue;
                        //Debug.WriteLine("has prop");
                        if (pInfo.GetValue(item) is not object obj || obj.ToString() is not string value) continue;
                        //Debug.WriteLine("has prop val");
                        if (!value.Contains(pValue, StringComparison.CurrentCultureIgnoreCase)) continue;
                        //Debug.WriteLine("has val");
                        groupReqd.Remove(filter);
                        if (groupReqd.Count is not 0) continue;
                        return true;
                    }
                    else
                    {
                        for (byte l = 0; l < MfgItem_FilterProps.Count; l++)
                        {
                            var property = MfgItem_FilterProps[l];
                            if (property.GetValue(item) is not object obj || obj.ToString() is not string value) continue;
                            //Debug.WriteLine(filter + " == " + value);
                            if (!value.Contains(filter, StringComparison.CurrentCultureIgnoreCase)) continue;
                            groupReqd.Remove(filter);
                            if (groupReqd.Count is not 0) continue;
                            return true;
                        }
                    }
                    i++;
                }
            }

            // No match found across columns
            //e.Accepted = false;
            return false;
        }
        #endregion


        #region "Properties"
        public static T? GetValue<T>(object value) where T : struct
        {
            if (value == null || value is DBNull) return null;
            if (value is T) return (T)value;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        internal static (string, PropUpdateResult, Type?, object?) UpdateProperty(object dataItem, string propertyName, object value)
        {
            var property = dataItem.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                try
                {
                    var prevValue = property.GetValue(dataItem);

                    object? convertedValue = ChangeType(value, property.PropertyType);

                    //Debug.WriteLine($"{prevValue} | {convertedValue}");
                    if (!((object?)prevValue as object)?.Equals((object?)convertedValue as object) ?? true)
                    {
                        property.SetValue(dataItem, convertedValue);
                        return (propertyName, PropUpdateResult.Worked, property.PropertyType, convertedValue);
                    }
                    else
                    {
                        return (propertyName, PropUpdateResult.SameValue, property.PropertyType, null);
                    }
                }
                catch (InvalidCastException ex)
                {
                    // Handle conversion errors
                    Debug.WriteLine($"Error updating property: {ex.Message}");
                    return (propertyName, PropUpdateResult.ConversionFailed, property.PropertyType, null);
                }
                catch (Exception ex)
                {
                    // Handle errors
                    Debug.WriteLine($"Error updating property: {ex.Message}");
                    return (propertyName, PropUpdateResult.Error, property.PropertyType, null);
                }
            }
            else
            {
                return (propertyName, PropUpdateResult.NoPropOrCantWrite, null, null);
            }
        }

        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        /// <summary>
        /// [ <c>public static Type GetNullableType(Type TypeToConvert)</c> ]
        /// <para></para>
        /// Convert any Type to its Nullable&lt;T&gt; form, if possible
        /// </summary>
        /// <param name="TypeToConvert">The Type to convert</param>
        /// <returns>
        /// The Nullable&lt;T&gt; converted from the original type, the original type if it was already nullable, or null 
        /// if either <paramref name="TypeToConvert"/> could not be converted or if it was null.
        /// </returns>
        /// <remarks>
        /// To qualify to be converted to a nullable form, <paramref name="TypeToConvert"/> must contain a non-nullable value 
        /// type other than System.Void.  Otherwise, this method will return a null.
        /// </remarks>
        /// <seealso cref="Nullable&lt;T&gt;"/>
        public static Type? GetNullableType(Type TypeToConvert)
        {
            // Abort if no type supplied
            if (TypeToConvert == null)
                return null;

            // If the given type is already nullable, just return it
            if (IsTypeNullable(TypeToConvert))
                return TypeToConvert;

            // If the type is a ValueType and is not System.Void, convert it to a Nullable<Type>
            if (TypeToConvert.IsValueType && TypeToConvert != typeof(void))
                return typeof(Nullable<>).MakeGenericType(TypeToConvert);

            // Done - no conversion
            return null;
        }

        /// <summary>
        /// [ <c>public static bool IsTypeNullable(Type TypeToTest)</c> ]
        /// <para></para>
        /// Reports whether a given Type is nullable (Nullable&lt; Type &gt;)
        /// </summary>
        /// <param name="TypeToTest">The Type to test</param>
        /// <returns>
        /// true = The given Type is a Nullable&lt; Type &gt;; false = The type is not nullable, or <paramref name="TypeToTest"/> 
        /// is null.
        /// </returns>
        /// <remarks>
        /// This method tests <paramref name="TypeToTest"/> and reports whether it is nullable (i.e. whether it is either a 
        /// reference type or a form of the generic Nullable&lt; T &gt; type).
        /// </remarks>
        /// <seealso cref="GetNullableType"/>
        public static bool IsTypeNullable(Type TypeToTest)
        {
            // Abort if no type supplied
            if (TypeToTest == null)
                return false;

            // If this is not a value type, it is a reference type, so it is automatically nullable
            //  (NOTE: All forms of Nullable<T> are value types)
            if (!TypeToTest.IsValueType)
                return true;

            // Report whether an underlying Type exists (if it does, TypeToTest is a nullable Type)
            return Nullable.GetUnderlyingType(TypeToTest) != null;
        }

        public static (DependencyProperty dp, object value)? DpValueFromInputType<T>(T value) where T : Control
        {
            if (value is TextBox txt)
            {
                return (TextBox.TextProperty, txt.Text);
            }
            if (value is ComboBox cmbx)
            {
                return (ComboBox.SelectedValueProperty, cmbx.SelectedValue);
            }
            if (value is CheckBox chkbx)
            {
                return (CheckBox.IsCheckedProperty, chkbx.IsChecked ?? false);
            }
            return null;
        }
        #endregion


        #region "Assembly Version"
        internal static Version? ApplicationVersion
        {
            get
            {
                // Get the executing assembly
                Assembly assembly = Assembly.GetExecutingAssembly();

                if (assembly.GetName() is not AssemblyName an) return null;
                return an.Version;
            }
        }

        internal static string ApplicationVersionStr
        {
            get =>
                (ApplicationVersion is not Version ver) ? "??" : $"{ver}";
        }

        internal static DateTime? ApplicationBuildDate
        {
            get
            {
                var dtRef = new DateTime(2000, 1, 1);
                if (ApplicationVersion is not Version ver) return null;
                var daysSince2000 = ver.Build;
                var timeSinceMidnight = ver.Revision * 2;
                return dtRef.AddDays(daysSince2000).AddSeconds(timeSinceMidnight);
            }
        }

        internal static string? ApplicationBuildDateStr
        {
            get
                => ApplicationBuildDate.ToString() ?? "??";
        }
        #endregion


        #region "Item Line Modification"
        /*
        internal static bool UpdateItemLineData(Guid manufid, example_queries_GetItemLinesByJobResult item)
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
        */

        internal async static Task<bool> CopyItemLineData(Guid manufid)
        {
            using (var ctx = new OrderDbCtx())
            {
                var dbline = await ctx.AIcManufs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.ManufId == manufid);
                if (dbline is null) return false;
                dbline.Description += " (COPY)";
                dbline.ManufId = Guid.Empty;
                await ctx.AIcManufs.AddAsync(dbline);
                return true;
            }
        }

        internal static async Task<bool> DeleteItemLine(Guid manufid, string job)
        {
            using (var ctx = new DBModels.Order.OrderDbCtx())
            {
                var txt = await ctx.AIcManufs
                    .Where(p => p.ManufId == manufid && p.JobNbr == job)
                    .AsSingleQuery()
                    .ExecuteDeleteAsync();
                if (txt is not 1) return false;
                return true;
            }
        }
        #endregion

        #region DataGrid
        public static DataGridCell? GetCellFromDataGrid(ref DataGrid dg, DataGridCellInfo cellInfo)
        {
            dg.ScrollIntoView(cellInfo.Item);
            var cellCont = cellInfo.Column.GetCellContent(cellInfo.Item);
            if (cellCont is null) return null;
            var cell = (DataGridCell?)cellCont.Parent;
            return cell;
        }

        internal static Point GetColumnPositionSimple(DataGrid grid, int columnIndex)
        {
            var header = FindVisualChild<DataGridColumnHeadersPresenter>(grid)
                ?.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridColumnHeader;

            return header?.TransformToAncestor(grid).Transform(new Point(0, 0)) ?? new Point(0, 0);
        }

        internal static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result) return result;
                var found = FindVisualChild<T>(child);
                if (found != null) return found;
            }
            return null;
        }
        #endregion



        public const string extProc_ImportEngMfg_exe = "P:\\!CRM\\IceMfgImport.exe";
        public const string extProc_ProcEngMfg_exe = "P:\\!CRM\\IceMfgProcess.exe";
        public const string extProc_CalcEngMatl_exe = "P:\\!CRM\\IceMatlCalc.exe";
        public const string extProc_CncCutList_exe = "P:\\!CRM\\IceCNCCutList.exe";
        public const string extProc_SymEngExp_exe = "P:\\!CRM\\SymIceExp.exe";

        public const string extProc_BaseDir = "P:\\_IC_EMQ\\Apps-IC_EMQ\\IC_Mfg\\";
        public const string extProc_New_ImportEngMfg = "IC_MfgImport.exe";
        public const string extProc_New_ProcEngMfg = "IC_MfgProcess.exe";
        public const string extProc_New_CalcEngMatl = "IC_MatlCalc.exe";
        public const string extProc_New_CncCutList = "IC_CNCCutList.exe";
        public const string extProc_New_SymEngExp = "IC_MfgImport.exe";

        public const string extProc_ColorSet_arg = "iColorSetID={{@ColorSetID}}";

        public static async void ExtProc_Button(string exeName, string colorSetId)
        {
            ProcessStartInfo psi = new(
                extProc_BaseDir + exeName,
                extProc_ColorSet_arg.Replace(
                    "{@ColorSetID}",
                    colorSetId.ToUpper()
                )
            );
            await RunExternal(psi);
        }

        public static async Task RunExternal(ProcessStartInfo startInfo)
        {
            //MessageBox.Show(startInfo.Arguments);
            CancellationToken t = new();
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
            /*
            proc.Exited += (ss, ee) =>
            {

            };
            */
            await proc.WaitForExitAsync(t);
            proc.Dispose();
        }
    }
}
