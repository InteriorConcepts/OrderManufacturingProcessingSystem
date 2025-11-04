using OMPS.Components;
using OMPS.viewModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using SCH = SQL_And_Config_Handler;

namespace OMPS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Error;
            Ext.ValidateAppSettings();

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

            Ext.MainViewModel = new Main_ViewModel();
            base.OnStartup(e);
        }
    }

}
