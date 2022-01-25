using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using WpfStarter.UI.Models;

namespace WpfStarter.UI.ViewModels
{
    internal class FooterViewModel : BindableBase
    {
        private Model model;

        private string _errorString;

        public string ErrorString
        {
            get => _errorString;
            private set => SetProperty(ref _errorString, value);
        }

        public DelegateCommand OperationLaunchCommand { get; private set; }

        public FooterViewModel(IContainerProvider containerProvider)
        {
            model = containerProvider.Resolve<Models.Model>();
            ErrorNotify.SetUINotifyMethod(ShowError);
            OperationLaunchCommand = new DelegateCommand(OperationLaunch);
            model.AppStateChanged += Model_AppStateChanged;
        }

        public void OperationLaunch()
        {
            model.BeginOperation.Invoke();
        }

        public void ShowError(string newError)
        {
            ErrorString = newError;
        }
        private void Model_AppStateChanged(string newState)
        {
            switch (newState)
            {
                case "DbConnectionFailed":
                    {
                        ErrorNotify.NewError(Localisation.Strings.DBError);
                        break;
                    }
                case "DbConnected":
                    {
                        ErrorNotify.NewError(string.Empty);
                        break;
                    }

                case "FileSelected":
                    {
                        break;
                    }

                case "Disabled":
                    {     
                        break;
                    }

                case "CriticalError":
                default:
                    {
                        ErrorNotify.NewError(Localisation.Strings.CriticalError);
                        break;
                    }
            }
        }
    }
}
