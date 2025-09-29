using OMPS.Core;
using OMPS.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

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

    public class Main_ViewModel: ObservableObject
    {
        private MainWindow _parentWin;
        public required MainWindow ParentWin
        {
            get { return this._parentWin; }
            set {
                this._parentWin = value;
                this.OrderSearch_VM.ParentWindow = value;
                this.EngOrder_VM.ParentWindow = value;
            }
        }

        public OrderSearch OrderSearch_VM { get; set; }
        public EngOrder EngOrder_VM { get; set; }

        public bool OrderSearch_IsSelected { get => Current is not null && Current is OrderSearch; }
        public bool EngOrder_IsSelected { get => Current is not null && Current is EngOrder; }

        private object? _current;
        public object? Current
        {
            get => _current;
            set
            {
                if (value is null)
                {
                    return;
                }
                _current = value;
                OnPropertyChanged();
                OnPropertyChanged("OrderSearch_IsSelected");
                OnPropertyChanged("EngOrder_IsSelected");
            }
        }

        public Main_ViewModel()
        {
            OrderSearch_VM = new(this.ParentWin);
            EngOrder_VM = new(this.ParentWin);
            _current = OrderSearch_VM;
        }
    }
}
