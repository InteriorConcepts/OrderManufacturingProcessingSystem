using OMPS.ViewModels;
using OMPS.Windows;
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

namespace OMPS.Pages
{
    /// <summary>
    /// Interaction logic for QuoteOrder.xaml
    /// </summary>
    public partial class QuoteOrder : UserControl
    {

        public QuoteOrder()
        {
            InitializeComponent();
            //
        }


        internal static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }
        internal static MainWindow ParentWindow { get => Ext.MainWindow; }
        public string? QuoteNbr { get; set; }
    }
}
