using OMPS.Core;
using OMPS.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows;

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

    public class Main_ViewModel: ObservableObject
    {
        public MainWindow ParentWin
        {
            get;
            set
            {
                field = value;
                this.OrderSearch_VM?.ParentWindow = value;
                this.EngOrder_VM?.ParentWindow = value;
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
                //this.ParentWin.MinHeight = 0;
                //this.ParentWin.MinWidth = 0;
                this.ParentWin.MinHeight = (value is false ? 600 : 0);
                this.ParentWin.MinWidth = (value is false ? 1025 : 0);
                this.ParentWin.Height = (value is false ? 620 : 620);
                this.ParentWin.Width = (value is false ? 1025 : 550);
                this.ParentWin.ResizeMode = (value is false ? ResizeMode.CanResizeWithGrip : ResizeMode.CanResize);
            }
        } = false;

        public double FontSize_MAX { get; } = 20;
        public double FontSize_MIN { get; } = 12;

        private double _fontSize_dataGrid = 14;
        public double FontSize_DataGrid
        {
            get => this._fontSize_dataGrid;
            set
            {
                if (this._fontSize_dataGrid == value) return;
                if (value > FontSize_MAX || value < FontSize_MIN) return;
                this._fontSize_dataGrid = value;
                OnPropertyChanged();
                OnPropertyChanged("FontSize_CanBeSmaller");
                OnPropertyChanged("FontSize_CanBeLarger");
            }
        }

        public bool FontSize_CanBeSmaller
        {
            get
            {
                return this.FontSize_DataGrid > FontSize_MIN;
            }
        }

        public bool FontSize_CanBeLarger
        {
            get
            {
                return this.FontSize_DataGrid < FontSize_MAX;
            }
        }


        public Login Login_VM { get; set; }
        public OrderSearch OrderSearch_VM { get; set; }
        public EngOrder EngOrder_VM { get; set; }
        public QuoteOrder QuoteOrder_VM { get; set; }

        public bool Login_IsSelected { get => Current is not null && Current is Login; }
        public bool OrderSearch_IsSelected { get => Current is not null && Current is OrderSearch; }
        public bool EngOrder_IsSelected { get => Current is not null && Current is EngOrder; }
        public bool QuoteOrder_IsSelected { get => Current is not null && Current is QuoteOrder; }

        public bool EngOrder_IsEnabled { get => this.EngOrder_VM.JobNbr is not null or ""; }
        public bool OrderSearch_IsEnabled { get => true; }
        public bool QuoteOrder_IsEnabled { get => !(this.QuoteOrder_VM.Lbl_JobNbr.Content is null or ""); }

        public bool CanPrevious {
            get => this.Previous is not null;
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

        private object? _current = null;
        public object? Current
        {
            get => _current;
            set
            {
                if (value is null) return;
                if (value == _current) return;
                if (_current is not null && this.Previous != _current && this._current is not Login)
                {
                    this.Previous = _current;
                }
                _current = value;
                OnPropertyChanged();
                OnPropertyChanged(value.GetType().Name + "_IsSelected");
                OnPropertyChanged(value.GetType().Name + "_IsEnabled");
            }
        }

        public Main_ViewModel()
        {
            this.ParentWin = Ext.MainWindow;
            Login_VM = new() { ParentWindow = this.ParentWin };
            OrderSearch_VM = new(this.ParentWin);
            EngOrder_VM = new(this.ParentWin);
            QuoteOrder_VM = new(this.ParentWin);
            WidgetMode = false;
            _current = OrderSearch_VM;
        }
    }
}
