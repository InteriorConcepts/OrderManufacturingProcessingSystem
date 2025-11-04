using OMPS.Core;
using OMPS.Pages;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace OMPS.viewModel
{

    public class OrderSearch_ViewModel : ObservableObject
    {
        public OrderSearch_ViewModel()
        {

        }
    }

    public class EngOrder_ViewModel : ObservableObject
    {
        public EngOrder_ViewModel()
        {

        }
    }

    public class Login_ViewModel : ObservableObject
    {
        public bool LoginCompleted
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public Login_ViewModel()
        {

        }
    }

    public class Main_ViewModel : ObservableObject
    {

        public bool WidgetMode {
            get;
            set
            {
                if (field == value) return;
                field = value;
                Ext.MainWindow.Spnl_FrameTabs.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                //Ext.MainWindow.statusbar.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                ((UIElement)Ext.MainWindow.statusbar.Parent).Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                Ext.MainWindow.Btn_Home.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                Ext.MainWindow.Btn_Back.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                Ext.MainWindow.Btn_Chat.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                Ext.MainWindow.Btn_Settings.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                Ext.MainWindow.Btn_ToggleSideNav.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                //Ext.MainWindow.MinHeight = 0;
                //Ext.MainWindow.MinWidth = 0;
                Ext.MainWindow.MinHeight = (value is false ? 600 : 0);
                Ext.MainWindow.MinWidth = (value is false ? 1025 : 0);
                Ext.MainWindow.Height = (value is false ? 620 : 620);
                Ext.MainWindow.Width = (value is false ? 1025 : 550);
                Ext.MainWindow.ResizeMode = (value is false ? ResizeMode.CanResizeWithGrip : ResizeMode.CanResize);
            }
        } = false;

        public double FontSize_MAX { get; } = 18;
        public double FontSize_MIN { get; } = 12;

        public double FontSize_Base
        {
            get;
            set
            {
                if (field == value) return;
                if (value > FontSize_MAX || value < FontSize_MIN) return;
                field = value;
                Ext.AddUpdateAppSettings("FontSize", value.ToString());
                OnPropertyChanged();
                OnPropertyChanged(nameof(FontSize_H1));
                OnPropertyChanged(nameof(FontSize_H1_1));
                OnPropertyChanged(nameof(FontSize_H1_2));
                OnPropertyChanged(nameof(FontSize_H1_3));
                OnPropertyChanged(nameof(FontSize_H1));
                OnPropertyChanged(nameof(FontSize_H2));
                OnPropertyChanged(nameof(FontSize_H2_1));
                OnPropertyChanged(nameof(FontSize_H2_2));
                OnPropertyChanged(nameof(FontSize_H2_3));
                OnPropertyChanged(nameof(FontSize_H3));
                OnPropertyChanged(nameof(FontSize_H4));
                OnPropertyChanged(nameof(FontSize_H5));
                OnPropertyChanged(nameof(FontSize_H6));
                OnPropertyChanged(nameof(FontSize_H7));
                OnPropertyChanged(nameof(FontSize_H8));
                OnPropertyChanged(nameof(FontSize_H9));
                OnPropertyChanged(nameof(FontSize_H10));
                OnPropertyChanged("FontSize_CanBeSmaller");
                OnPropertyChanged("FontSize_CanBeLarger");
            }
        } = ((Ext.ReadSettingAsDouble("FontSize") is (true, double) res && res.success) ? res.value : 12);

        private readonly double FontSize_H1_scale   = (2 / Math.Sqrt(1));
        private readonly double FontSize_H1_1_scale = (2 / Math.Sqrt(1.25));
        private readonly double FontSize_H1_2_scale = (2 / Math.Sqrt(1.50));
        private readonly double FontSize_H1_3_scale = (2 / Math.Sqrt(1.75));
        private readonly double FontSize_H2_scale = (2 / Math.Sqrt(2));
        private readonly double FontSize_H2_1_scale = (2 / Math.Sqrt(2.25));
        private readonly double FontSize_H2_2_scale = (2 / Math.Sqrt(2.50));
        private readonly double FontSize_H2_3_scale = (2 / Math.Sqrt(2.75));
        private readonly double FontSize_H3_scale   = (2 / Math.Sqrt(3));
        private readonly double FontSize_H4_scale   = (2 / Math.Sqrt(4));
        private readonly double FontSize_H5_scale   = (2 / Math.Sqrt(5));
        private readonly double FontSize_H6_scale   = (2 / Math.Sqrt(6));
        private readonly double FontSize_H7_scale   = (2 / Math.Sqrt(7));
        private readonly double FontSize_H8_scale   = (2 / Math.Sqrt(8));
        private readonly double FontSize_H9_scale   = (2 / Math.Sqrt(9));
        private readonly double FontSize_H10_scale  = (2 / Math.Sqrt(10));
        public double FontSize_H1 { get => Math.Round(FontSize_Base * GetFontScaler(), 0); }
        public double FontSize_H1_1 { get => Math.Round(FontSize_Base * GetFontScaler(), 1); }
        public double FontSize_H1_2 { get => Math.Round(FontSize_Base * GetFontScaler(), 1); }
        public double FontSize_H1_3 { get => Math.Round(FontSize_Base * GetFontScaler(), 1); }
        public double FontSize_H2   { get => Math.Round(FontSize_Base * GetFontScaler(), 0); }
        public double FontSize_H2_1 { get => Math.Round(FontSize_Base * GetFontScaler(), 1); }
        public double FontSize_H2_2 { get => Math.Round(FontSize_Base * GetFontScaler(), 1); }
        public double FontSize_H2_3 { get => Math.Round(FontSize_Base * GetFontScaler(), 1); }
        public double FontSize_H3   { get => Math.Round(FontSize_Base * GetFontScaler(), 0); }
        public double FontSize_H4   { get => Math.Round(FontSize_Base * GetFontScaler(), 0); }
        public double FontSize_H5   { get => Math.Round(FontSize_Base * GetFontScaler(), 0); }
        public double FontSize_H6   { get => Math.Round(FontSize_Base * GetFontScaler(), 0); }
        public double FontSize_H7   { get => Math.Round(FontSize_Base * GetFontScaler(), 0); }
        public double FontSize_H8   { get => Math.Round(FontSize_Base * GetFontScaler(), 0); }
        public double FontSize_H9   { get => Math.Round(FontSize_Base * GetFontScaler(), 0); }
        public double FontSize_H10  { get => Math.Round(FontSize_Base * GetFontScaler(), 0); }

        private const double FONTSIZE_DEFAULT_SCALE = 1.0;
        private double GetFontScaler([CallerMemberName] string? propName = null)
        {
            try
            {
                if (propName is null or "") return FONTSIZE_DEFAULT_SCALE;
                var name = $"{propName}_scale";
                var bindFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var field = this.GetType().GetField(name, bindFlags);
                if (field is null) return FONTSIZE_DEFAULT_SCALE;
                if (field.FieldType != typeof(double)) return FONTSIZE_DEFAULT_SCALE;
                if (field.GetValue(this) is not double val) return FONTSIZE_DEFAULT_SCALE;
                return val;
            } catch
            {
                return FONTSIZE_DEFAULT_SCALE;
            }
        }

        public bool FontSize_CanBeSmaller
        {
            get
            {
                return this.FontSize_Base > FontSize_MIN;
            }
        }

        public bool FontSize_CanBeLarger
        {
            get
            {
                return this.FontSize_Base < FontSize_MAX;
            }
        }



        public TimeZoneInfo CurrentTimezone
        {
            get;
            set {
                field = value;
                OnPropertyChanged();
            }
        } = TimeZoneInfo.Local;

        public DateTime CurrentDatetime
        {
            get => TimeZoneInfo.ConvertTime(DateTime.Now, CurrentTimezone);
        }

        public const string UrlBase = "pbridge://";
        public string UrlPath
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "";
        public string Url {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = UrlBase;

        public Home? Home_VM { get; set; }
        public Login? Login_VM { get; set; }
        public OrderSearch? OrderSearch_VM { get; set; }
        public EngOrder? EngOrder_VM { get; set; }
        public QuoteOrder? QuoteOrder_VM { get; set; }
        public ProductCatalogSearch? ProductCatalogSearch_VM { get; set; }
        public ProductCatalogDetails? ProductCatalogDetails_VM { get; set; }

        public bool Home_IsSelected { get => this.CurrentPage is PageTypes.Home; }
        public bool Login_IsSelected { get => this.CurrentPage is PageTypes.Login; }
        public bool OrderSearch_IsSelected { get => this.CurrentPage is PageTypes.OrderSearch; }
        public bool EngOrder_IsSelected { get => this.CurrentPage is PageTypes.EngOrder; }
        public bool QuoteOrder_IsSelected { get => this.CurrentPage is PageTypes.QuoteOrder; }
        public bool ProductCatalogSearch_IsSelected { get => this.CurrentPage is PageTypes.ProductCatalogSearch; }
        public bool ProductCatalogDetails_IsSelected { get => this.CurrentPage is PageTypes.ProductCatalogDetails; }

        public bool EngOrder_IsEnabled { get => this.EngOrder_VM is not null && this.EngOrder_VM.JobNbr is not null or ""; }
        public bool OrderSearch_IsEnabled { get => this.OrderSearch_VM is not null && (this.OrderSearch_VM.ColorSetInfos.Count is not 0 || OrderSearch_IsSelected); }
        public bool QuoteOrder_IsEnabled { get => this.QuoteOrder_VM is not null && this.QuoteOrder_VM.QuoteNbr is not null or ""; }
        public bool ProductCatalogSearch_IsEnabled { get => this.ProductCatalogSearch_VM is not null && this.ProductCatalogSearch_VM.IsLoaded; }
        public bool ProductCatalogDetails_IsEnabled { get => this.ProductCatalogDetails_VM is not null && this.ProductCatalogDetails_VM.IsLoaded; }

        public bool CanPrevious {
            get => this.PreviousPage is not PageTypes.None or PageTypes.Login;
            set
            {
                OnPropertyChanged();
            }
        }

        public object? Previous
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
                CanPrevious = (value != null);
            }
        } = null;


        public PageTypes PreviousPage
        {
            get;
            set
            {
                if (value is PageTypes.None) return;
                field = value;
                OnPropertyChanged();
            }
        } = PageTypes.None;

        public PageTypes CurrentPage
        {
            get;
            set
            {
                if (value == field) return;
                if (field is not PageTypes.None)
                {
                    this.ToggleCurrentContentControl(Visibility.Collapsed);
                    this.Previous = field;
                }
                field = value;
                OnPropertyChanged();
                this.UpdateUrl();
                this.WidgetMode = (value is PageTypes.Login);
                ToggleCurrentContentControl(Visibility.Visible);
                RunPageDefaultFirstBehaviour(value);
                OnPropertyChanged(this.PreviousPage + "_IsSelected");
                OnPropertyChanged(this.PreviousPage + "_IsEnabled");
                OnPropertyChanged(value + "_IsSelected");
                OnPropertyChanged(value + "_IsEnabled");
            }
        } = PageTypes.None;

        public void UpdateUrl()
        {
            this.Url = $"{UrlBase}{GetPageUrlPath(this.CurrentPage)}{GetPageRelUrlPath(this.CurrentPage)}";
        }

        public void SetUrlRelPath(string rel = "")
        {
            this.UrlPath = rel;
            UpdateUrl();
        }

        public static string GetPageUrlPath(PageTypes pageType)
        {
            return (pageType switch
            {
                PageTypes.Home => "home",
                PageTypes.Login => "login",
                PageTypes.OrderSearch => "order-search",
                PageTypes.EngOrder => "eng-order",
                PageTypes.QuoteOrder => "quote-order",
                PageTypes.ProductCatalogSearch => "product-catalog",
                PageTypes.ProductCatalogDetails => "product-catalog",
                _ => ""
            });
        }

        public string GetPageRelUrlPath(PageTypes pageType)
        {
            return (pageType switch
            {
                PageTypes.Home => "",
                PageTypes.Login => "",
                PageTypes.OrderSearch => "",
                PageTypes.EngOrder => $"?job={this.EngOrder_VM?.JobNbr}&tab={this.EngOrder_VM?.dpnl_ViewsBar.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked is true)?.Tag}",
                PageTypes.QuoteOrder => "",
                PageTypes.ProductCatalogSearch => "",
                PageTypes.ProductCatalogDetails => "",
                _ => ""
            });
        }

        public void RunPageDefaultFirstBehaviour(PageTypes pageType)
        {
            switch (pageType)
            {
                case PageTypes.Home:
                    if (this.Home_VM?.NewOrders?.Count is not 0) return;
                    this.Home_VM?.LoadData();
                    Debug.WriteLine("Load Home");
                    break;
                case PageTypes.Login:
                    break;
                case PageTypes.OrderSearch:
                    if (this.OrderSearch_VM?.ColorSetInfos?.Count is not 0) return;
                    Debug.WriteLine("Load Orders");
                    this.OrderSearch_VM?.LoadRecentOrders();
                    break;
                case PageTypes.EngOrder:
                    break;
                case PageTypes.QuoteOrder:
                    break;
                case PageTypes.ProductCatalogSearch:
                    if (this.ProductCatalogSearch_VM?.Products?.Count is not 0) return;
                    Debug.WriteLine("Load Products");
                    this.ProductCatalogSearch_VM?.LoadProducts();
                    break;
                case PageTypes.ProductCatalogDetails:
                    break;
                default:
                    break;
            }
        }

        public void ToggleCurrentContentControl(Visibility state)
        {
            (this.CurrentPage switch
            {
                PageTypes.Home => Ext.MainWindow.CC_Landing,
                PageTypes.Login => Ext.MainWindow.CC_Login,
                PageTypes.OrderSearch => Ext.MainWindow.CC_OrderSearch,
                PageTypes.EngOrder => Ext.MainWindow.CC_EngOrder,
                PageTypes.QuoteOrder => Ext.MainWindow.CC_QuoteOrder,
                PageTypes.ProductCatalogSearch => Ext.MainWindow.CC_ProductCatalogSearch,
                PageTypes.ProductCatalogDetails => Ext.MainWindow.CC_ProductCatalogDetails,
                _ => null
            })?.Visibility = state;
        }

        public Main_ViewModel()
        {
            this.WidgetMode = false;
        }

        public void Init()
        {
            this.Home_VM = new();
            this.Login_VM = new();
            this.OrderSearch_VM = new() { };
            this.EngOrder_VM = new() { };
            this.QuoteOrder_VM = new();
            this.ProductCatalogSearch_VM = new() { };
            this.ProductCatalogDetails_VM = new();
        }
    }
}
