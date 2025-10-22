using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace OMPS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Ext.ValidateAppSettings();
            base.OnStartup(e);
        }
    }

}
