namespace OMPS.ViewModels
{
    public class Login_ViewModel : ViewModels.ViewModelBase
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
}
