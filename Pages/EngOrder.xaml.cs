using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MyApp.DataAccess.Generated;
using OMPS.Components;
using OMPS.DBModels;
using OMPS.DBModels.Order;
using OMPS.DBModels.Product;
using OMPS.ViewModels;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Windows.ApplicationModel.Background;
using Windows.System.RemoteSystems;
using SCH = SQL_And_Config_Handler;

namespace OMPS.Pages
{
    /// <summary>
    /// Interaction logic for EngOrder.xaml
    /// </summary>
    public partial class EngOrder : UserControl
    {
        public EngOrder()
        {
            InitializeComponent();
            this.dpnl_DataFilter.Visibility = Visibility.Collapsed;
            //this.FrmFin.ItemSource = Finishes_Default;
        }

    }
}
