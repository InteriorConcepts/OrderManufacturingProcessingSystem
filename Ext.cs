using Humanizer;
using MyApp.DataAccess.Generated;
using OMPS.Models;
using OMPS.Pages;
using OMPS.viewModel;
using OMPS.Windows;
using System;
using System.Collections.Generic;
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
using static OMPS.Pages.EngOrder;


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
            EngOrder_
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
        public static bool MfgItems_Filter(example_queries_GetItemLinesByJobResult item, string filterText)
        {

            var properties =
                typeof(example_queries_GetItemLinesByJobResult).GetProperties().
                    Where(p => p.PropertyType != typeof(Guid));

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
                        if (value is not null && value.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
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
        #endregion


        #region "Properties"
        internal static (string, PropUpdateResult, Type?, object?) UpdateProperty(object dataItem, string propertyName, object value)
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
                        return (propertyName, PropUpdateResult.Worked, property.PropertyType, convertedValue);
                    }
                    else
                    {
                        return (propertyName, PropUpdateResult.SameValue, property.PropertyType, null);
                    }
                }
                catch (Exception ex)
                {
                    // Handle conversion errors
                    Debug.WriteLine($"Error updating property: {ex.Message}");
                    return (propertyName, PropUpdateResult.Error, property.PropertyType, null);
                }
            }
            else
            {
                return (propertyName, PropUpdateResult.NoPropOrCantWrite, null, null);
            }
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

        internal static bool CopyItemLineData(Guid manufid, example_queries_GetItemLinesByJobResult item)
        {
            if (item is null) return false;
            var res = Ext.Queries.CopyManufItemLineByManufID(" (COPY)", manufid);
            if (res is null) return false;
            return true;
        }

        internal static async Task<bool> DeleteItemLine(Guid manufid, string job)
        {
#if NEWDBSQL
            using (var ctx = new Models.Order.OrderDbCtx())
            {
                var txt = await ctx.AIcIceManufs
                    .Where(p => p.IceManufId == manufid && p.JobNbr == job)
                    .AsSingleQuery()
                    .ExecuteDeleteAsync();
                if (txt is not 1) return false;
                return true;
            }
#else
            return await Task.Run<bool>(() =>
            {
                var res = Ext.Queries.DeleteManufItemLineByManufID(job, manufid);
                return (res is not null);
            });
#endif
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

        public static async Task RunExternal(ProcessStartInfo startInfo)
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
    }
}
