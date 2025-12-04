using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

using OMPS.Components;
using OMPS.ViewModels;

using Tomlyn;
using Tomlyn.Model;

using static TOML_Helper.TOML_Handler.Config_Toml;

using SCH = SQL_And_Config_Handler;

namespace OMPS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex? _mutex = null;
        public const string appName = "ICC_OrderManufacturingProcessingSystem";

        protected override void OnStartup(StartupEventArgs e)
        {
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Error;

            // Handle Single Instance
            _mutex = new Mutex(true, appName, out bool createdNew);
            if (!createdNew)
            {
                // Another instance is already running
                MessageBox.Show("Another instance of the application is already running.", "Application Already Running", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Application.Current.Shutdown();
                return;
            }

            Ext.ValidateAppSettings();

            // Config & DB
            var res = SCH.SQLDatabaseConnection.Init();
            if (res.Item1 is false || res.Item2 is not null)
            {
                string msg = "Could not load Config:\n";
                if (res is (bool, Exception) && res.Item2 is not null)
                {
                    msg += res.Item2.Message + "\n" + res.Item2.StackTrace;
                }
                else
                {
                    msg += SCH.Global.Config.GetErrorString();
                }
                Ext.MainWindow.MainToastContainer.CreateToast("Application", msg, FeedbackToast.IconTypes.Error).Show();
                MessageBox.Show(msg);
                App.Current.Shutdown(-1);
            }

            Ext.LoadAllTomlConfigs();
            
            
            /*
            if (Ext.TryGet<EngOrder_FiltersConfig>(Ext.TomlConfigTypes.EngOrder_SavedFilters, "settings") is TomlTryValue<EngOrder_FiltersConfig> tv && tv is not null && tv.Passed is true && tv.Value is EngOrder_FiltersConfig obj)
            {
                MessageBox.Show(String.Join("\n", obj.filters.Select(i => $"{i.name}\t\t{i.value}")));
            } else
            {
                MessageBox.Show("Toml Failed");
            }
            */
            
            

            Ext.MainViewModel = new Main_ViewModel();
            base.OnStartup(e);
        }


        public static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_mutex is null) return;
            
            _mutex.ReleaseMutex();
            _mutex.Dispose();
            
            base.OnExit(e);
        }
    }

}
