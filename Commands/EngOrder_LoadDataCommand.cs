using OMPS.Pages;
using OMPS.viewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OMPS.Commands
{
    public class EngOrder_LoadDataCommand : CommandBase
    {
        private readonly EngOrder_ViewModel _engOrder_ViewModel;
        private readonly EngOrder _engOrder;

        public EngOrder_LoadDataCommand(EngOrder_ViewModel engOrder_ViewModel, EngOrder engOrder)
        {
            this._engOrder_ViewModel = engOrder_ViewModel;
            this._engOrder = engOrder;
            this._engOrder_ViewModel.PropertyChanged += _engOrder_ViewModel_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return !string.IsNullOrEmpty(_engOrder_ViewModel.JobNbr) && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {

        }

        private void _engOrder_ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EngOrder_ViewModel.JobNbr))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
