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
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Error;
            Ext.ValidateAppSettings();
            base.OnStartup(e);
        }
    }

}
