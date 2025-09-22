using MyApp.DataAccess.Generated;
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
        public static string StringFormat_Currency = "{}{0:C2.00}";
        public static string StringFormat_Text = "{}";

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
    }
}
