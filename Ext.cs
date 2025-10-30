using Humanizer;
using MyApp.DataAccess.Generated;
using OMPS.Models;
using OMPS.Pages;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;


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
        public static void ValidateAppSettings()
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
        public static (bool success, string value) ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings[key] is not string str)
                {
                    return (false, "");
                } else
                return (true, str);
            }
            catch (ConfigurationErrorsException)
            {
                return (false, "Error reading app settings");
                MessageBox.Show("Error reading app settings");
            }
        }

        public static (bool success, bool value) ReadSettingAsBool(string key)
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
                MessageBox.Show("Error reading app settings");
            }
        }

        public static (bool success, double value) ReadSettingAsDouble(string key)
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
                MessageBox.Show("Error reading app settings");
            }
        }

        public static (bool success, int value) ReadSettingAsInt(string key)
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
                MessageBox.Show("Error reading app settings");
            }
        }

        public static void AddUpdateAppSettings(string key, string value)
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

        public static IEnumerable<FileInfo> MultiEnumerateFiles(this DirectoryInfo di, string patterns, SearchOption sOpts = SearchOption.TopDirectoryOnly)
        {
            foreach (var pattern in patterns.Split('|'))
                foreach (var file in di.EnumerateFiles(pattern, sOpts))
                    yield return file;
        }
        public static IEnumerable<DirectoryInfo> MultiEnumerateDirectories(this DirectoryInfo di, string patterns, SearchOption sOpts = SearchOption.TopDirectoryOnly)
        {
            foreach (var pattern in patterns.Split('|'))
                foreach (var file in di.EnumerateDirectories(pattern, sOpts))
                    yield return file;
        }

        public static MainWindow MainWindow;

        public static string StringFormat_Currency = "{}{0:C2.00}";
        public static string StringFormat_Text = "{}";
        /*
        public static readonly UISettings UISettings = new();
        public static bool IsDark()
        {
            var fg = UISettings.GetColorValue(UIColorType.Foreground);
            return (((5 * fg.G) + (2 * fg.R) + fg.B) > (8 * 128));
        }

#pragma warning disable WPF0001 
        public static bool CurrentThemeIsDark() =>
            IsDark();
#pragma warning restore WPF0001
        */

        public static readonly example_queriesQueries Queries = new();

        public static bool IsJobNumValid(string job)
        {
            if (job[0] is not 'J' && job[0] is not 'S') return false;
            if (!job[1..].All(c => int.TryParse(c.ToString(), out int _))) return false;
            return true;
        }

        public static void FormatJobNum(ref string job)
        {
            if (job.Length >= (job[0] is 'J' ? 6 : 5) && job.Length is not 10)
            {
                job = $"{job[0]}{job[(job.Length - 5)..(job.Length)].PadLeft(9, '0')}";
            }
        }

        #region "Storyboard / Animations"

        public static T SetTarget<T>(this T anim, DependencyProperty prop, DependencyObject obj) where T : AnimationTimeline
        {
            return Ext.SetTarget(anim, new PropertyPath(prop), obj);
        }

        public static T SetTarget<T>(this T anim, PropertyPath path, DependencyObject obj) where T : AnimationTimeline
        {
            _ = Ext.SetTargetObject(anim, obj);
            _ = Ext.SetTargetProperty(anim, path);
            return anim;
        }

        public static T SetTargetObject<T>(this T anim, DependencyObject obj) where T : AnimationTimeline
        {
            Storyboard.SetTarget(anim, obj);
            return anim;
        }

        public static T SetTargetProperty<T>(this T anim, PropertyPath path) where T : AnimationTimeline
        {
            Storyboard.SetTargetProperty(anim, path);
            return anim;
        }

        #endregion



        public static PageTypes PageToType<T>(this T page) where T : UserControl
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

        public static MessageBoxResult PopupConfirmation(string text, string caption, MessageBoxButton btns, MessageBoxImage img)
        {
            return MessageBox.Show(text, caption, btns, img);
        }
    }
}
