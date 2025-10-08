using MyApp.DataAccess.Generated;
using OMPS.Pages;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Windows.UI;
using Windows.UI.ViewManagement;
using OMPS.Windows;


namespace OMPS
{
    public enum PageTypes
    {
        None = -1,
        Login = 0,
        OrderSearch = 1,
        EngOrder = 2,
        QuoteOrder = 3
    }

    public static class Ext
    {
        public static MainWindow MainWindow;

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
