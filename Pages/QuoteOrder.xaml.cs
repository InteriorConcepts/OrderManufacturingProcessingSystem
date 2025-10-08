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
using OMPS.Windows;

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
        }

        public QuoteOrder(MainWindow parentWin)
        {
            InitializeComponent();
            this.ParentWindow = parentWin;
        }

        internal MainWindow? ParentWindow { get; set; }
    }
}
