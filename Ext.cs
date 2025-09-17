using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows;
using Windows.UI;
using Windows.UI.ViewManagement;


namespace OMPS
{
    public static class Ext
    {
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
    }
}
