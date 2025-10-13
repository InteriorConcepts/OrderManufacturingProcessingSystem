#pragma warning disable Experimental
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using static OMPS.Ext;

namespace OMPS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
#pragma warning disable WPF0001 
            ThemeMode = ThemeMode.None;
#pragma warning restore WPF0001
            base.OnStartup(e);
        }
    }

}
#pragma warning restore Experimental
