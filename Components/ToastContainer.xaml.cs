using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static OMPS.Components.FeedbackToast;

namespace OMPS.Components
{
    /// <summary>
    /// Interaction logic for ToastContainer.xaml
    /// </summary>
    public partial class ToastContainer : UserControl
    {
        public ToastContainer()
        {
            InitializeComponent();
        }

        public FeedbackToast CreateToast(string Title, string Desc = null, IconTypes icon = IconTypes.Info, ushort Duration = 3300)
        {
            var t = new FeedbackToast() { Title = Title, IconType = icon, ShownDurationMS = Duration };
            if (Desc is not null)
            {
                t.Body = Desc;
            }
            t.ExitAnim_Completed += ((object? sender, EventArgs e) =>
            {
                if (sender is not FeedbackToast ftoast) return;
                this.SPnl_Toasts.Children.Remove(ftoast);
            });
            this.SPnl_Toasts.Children.Add(t);
            return t;
        }
    }
}
