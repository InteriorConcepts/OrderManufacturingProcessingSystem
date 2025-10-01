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
        private MainWindow _parentWin;
        public required MainWindow ParentWin
        {
            get { return this._parentWin; }
            set {
                this._parentWin = value;
                this.OrderSearch_VM?.ParentWindow = value;
                this.EngOrder_VM?.ParentWindow = value;
                this.Login_VM?.ParentWindow = value;
            }
        }

        private bool _widgetMode = false;
        public bool WidgetMode {
            get => this._widgetMode;
            set
            {
                this._widgetMode = value;
                this.ParentWin.Spnl_FrameTabs.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                this.ParentWin.statusbar.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                this.ParentWin.Btn_Home.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                this.ParentWin.Btn_Back.Visibility = (value is false ? Visibility.Visible : Visibility.Collapsed);
                //this.ParentWin.MinHeight = 0;
                //this.ParentWin.MinWidth = 0;
                this.ParentWin.MinHeight = (value is false ? 600 : 0);
                this.ParentWin.MinWidth = (value is false ? 1025 : 0);
                this.ParentWin.Height = (value is false ? 620 : 620);
                this.ParentWin.Width = (value is false ? 1025 : 550);
                this.ParentWin.ResizeMode = (value is false ? ResizeMode.CanResizeWithGrip : ResizeMode.NoResize);
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

        private bool _canPrevious = false;
        public bool CanPrevious {
            get => this._previous is not null;
            set
            {
                this._canPrevious = value;
                OnPropertyChanged();
            }
        }

        private object? _previous = null;
        public object? Previous
        {
            get => this._previous;
            set
            {
                this._previous = value;
                OnPropertyChanged();
                CanPrevious = (value != null);
            }
        }

        private object? _current = null;
        public object? Current
        {
            get => _current;
            set
            {
                if (value is null) return;
                if (value == _current) return;
                if (_current is not null && this._previous != _current)
                {
                    this.Previous = _current;
                }
                Debug.WriteLine("Current set");
                _current = value;
                Debug.WriteLine(value.GetType().Name);
                //WidgetMode = this.Login_IsSelected;
                OnPropertyChanged();
                OnPropertyChanged("Login_IsSelected");
                OnPropertyChanged("OrderSearch_IsSelected");
                OnPropertyChanged("EngOrder_IsSelected");
                OnPropertyChanged("QuoteOrder_IsSelected");
            }
        }

        public Main_ViewModel()
        {
            Login_VM = new() { ParentWindow = this.ParentWin };
            OrderSearch_VM = new(this.ParentWin);
            EngOrder_VM = new(this.ParentWin);
            QuoteOrder_VM = new(this.ParentWin);
            _current = OrderSearch_VM;
        }

        public Main_ViewModel(MainWindow parentWin)
        {
            this.ParentWin = parentWin;
            Login_VM = new() { ParentWindow = this.ParentWin };
            OrderSearch_VM = new(this.ParentWin);
            EngOrder_VM = new(this.ParentWin);
            QuoteOrder_VM = new(this.ParentWin);
            WidgetMode = false;
            _current = OrderSearch_VM;
        }
    }
}
