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
        private bool loginCompleted;

        public bool LoginCompleted
        {
            get => this.loginCompleted;
            set
            {
                loginCompleted = value;
                OnPropertyChanged();
            }
        }

        public Login_ViewModel()
        {

        }
    }

    public class Main_ViewModel : ObservableObject
    {
        public MainWindow ParentWin
        {
            get;
            set
            {
                field = value;
                this.OrderSearch_VM?.ParentWindow = value;
                //this.EngOrder_VM?.ParentWindow = value;
                this.Login_VM?.ParentWindow = value;
            }
        } = Ext.MainWindow;

        public bool WidgetMode {
            get;
            set
            {
                field = value;
                this.ParentWin.Spnl_FrameTabs.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                //this.ParentWin.statusbar.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                ((UIElement)this.ParentWin.statusbar.Parent).Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                this.ParentWin.Btn_Home.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                this.ParentWin.Btn_Back.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                this.ParentWin.Btn_Chat.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                this.ParentWin.Btn_Settings.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                this.ParentWin.Btn_ToggleSideNav.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                //this.ParentWin.MinHeight = 0;
                //this.ParentWin.MinWidth = 0;
                this.ParentWin.MinHeight = (value is false ? 600 : 0);
                this.ParentWin.MinWidth = (value is false ? 1025 : 0);
                this.ParentWin.Height = (value is false ? 620 : 620);
                this.ParentWin.Width = (value is false ? 1025 : 550);
                this.ParentWin.ResizeMode = (value is false ? ResizeMode.CanResizeWithGrip : ResizeMode.CanResize);
            }
        } = false;

        public double FontSize_MAX { get; } = 18;
        public double FontSize_MIN { get; } = 12;

        private double _fontSize_base = 12;
        public double FontSize_Base
        {
            get => this._fontSize_base;
            set
            {
                if (this._fontSize_base == value) return;
                if (value > FontSize_MAX || value < FontSize_MIN) return;
                this._fontSize_base = value;
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
        }

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
        public double FontSize_H1 { get => Math.Round(this._fontSize_base * GetFontScaler(), 0); }
        public double FontSize_H1_1 { get => Math.Round(this._fontSize_base * GetFontScaler(), 1); }
        public double FontSize_H1_2 { get => Math.Round(this._fontSize_base * GetFontScaler(), 1); }
        public double FontSize_H1_3 { get => Math.Round(this._fontSize_base * GetFontScaler(), 1); }
        public double FontSize_H2   { get => Math.Round(this._fontSize_base * GetFontScaler(), 0); }
        public double FontSize_H2_1 { get => Math.Round(this._fontSize_base * GetFontScaler(), 1); }
        public double FontSize_H2_2 { get => Math.Round(this._fontSize_base * GetFontScaler(), 1); }
        public double FontSize_H2_3 { get => Math.Round(this._fontSize_base * GetFontScaler(), 1); }
        public double FontSize_H3   { get => Math.Round(this._fontSize_base * GetFontScaler(), 0); }
        public double FontSize_H4   { get => Math.Round(this._fontSize_base * GetFontScaler(), 0); }
        public double FontSize_H5   { get => Math.Round(this._fontSize_base * GetFontScaler(), 0); }
        public double FontSize_H6   { get => Math.Round(this._fontSize_base * GetFontScaler(), 0); }
        public double FontSize_H7   { get => Math.Round(this._fontSize_base * GetFontScaler(), 0); }
        public double FontSize_H8   { get => Math.Round(this._fontSize_base * GetFontScaler(), 0); }
        public double FontSize_H9   { get => Math.Round(this._fontSize_base * GetFontScaler(), 0); }
        public double FontSize_H10  { get => Math.Round(this._fontSize_base * GetFontScaler(), 0); }

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


        public Home Home_VM { get; set; }
        public Login Login_VM { get; set; }
        public OrderSearch OrderSearch_VM { get; set; }
        public EngOrder EngOrder_VM { get; set; }
        public QuoteOrder QuoteOrder_VM { get; set; }
        public ProductCatalogSearch ProductCatalogSearch_VM { get; set; }
        public ProductCatalogDetails ProductCatalogDetails_VM { get; set; }

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
        public bool ProductCatalogSearch_IsEnabled { get => this.ProductCatalogSearch_VM is not null; }
        public bool ProductCatalogDetails_IsEnabled { get => this.ProductCatalogDetails_VM is not null; }

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
                this.WidgetMode = value is PageTypes.Login;
                ToggleCurrentContentControl(Visibility.Visible);
                RunPageDefaultFirstBehaviour(value);
                OnPropertyChanged(this.PreviousPage + "_IsSelected");
                OnPropertyChanged(this.PreviousPage + "_IsEnabled");
                OnPropertyChanged(value + "_IsSelected");
                OnPropertyChanged(value + "_IsEnabled");
            }
        } = PageTypes.None;

        public void RunPageDefaultFirstBehaviour(PageTypes pageType)
        {
            switch (pageType)
            {
                case PageTypes.Home:
                    break;
                case PageTypes.Login:
                    break;
                case PageTypes.OrderSearch:
                    Debug.WriteLine("Load Orders");
                    this.OrderSearch_VM.LoadRecentOrders();
                    break;
                case PageTypes.EngOrder:
                    break;
                case PageTypes.QuoteOrder:
                    break;
                case PageTypes.ProductCatalogSearch:
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
                PageTypes.Home => this.ParentWin.CC_Landing,
                PageTypes.Login => this.ParentWin.CC_Login,
                PageTypes.OrderSearch => this.ParentWin.CC_OrderSearch,
                PageTypes.EngOrder => this.ParentWin.CC_EngOrder,
                PageTypes.QuoteOrder => this.ParentWin.CC_QuoteOrder,
                PageTypes.ProductCatalogSearch => this.ParentWin.CC_ProductCatalogSearch,
                PageTypes.ProductCatalogDetails => this.ParentWin.CC_ProductCatalogDetails,
                _ => null
            })?.Visibility = state;
        }

        public Main_ViewModel()
        {
            //this.EngOrder_VM.JobNbr = "J000000123";
            this.WidgetMode = false;
            //this.CurrentPage = PageTypes.OrderSearch;
        }

        public void Init()
        {
            this.ParentWin = Ext.MainWindow;
            this.Home_VM = new(this.ParentWin);
            this.Login_VM = new(this.ParentWin);
            this.OrderSearch_VM = new() { ParentWindow = this.ParentWin };
            this.EngOrder_VM = new() { ParentWindow = this.ParentWin };
            this.QuoteOrder_VM = new(this.ParentWin);
            this.ProductCatalogSearch_VM = new(this.ParentWin);
            this.ProductCatalogDetails_VM = new(this.ParentWin);
        }
    }
}
