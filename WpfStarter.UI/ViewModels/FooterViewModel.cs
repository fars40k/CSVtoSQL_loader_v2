using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Threading;
using WpfStarter.UI.Models;

namespace WpfStarter.UI.ViewModels
{
    internal class FooterViewModel : BindableBase
    {
        private Model _model;
        private IContainerProvider _container;

        private string _errorString;
        public string ErrorString
        {
            get => _errorString;
            private set => SetProperty(ref _errorString, value);
        }

        public DelegateCommand OperationLaunchCommand { get; private set; }
        public DelegateCommand OperationCancelCommand { get; private set; }

        public FooterViewModel(IContainerProvider containerProvider)
        {
            _container = containerProvider;
            _model = containerProvider.Resolve<Models.Model>();

            ErrorNotify.SetUINotifyMethod(ShowError);

            OperationLaunchCommand = new DelegateCommand(LaunchOperation);
            OperationCancelCommand = new DelegateCommand(CancelOperations);

            _model.ApplicationStateChanged += ChangeUIControlStrings;
            ChangeUIControlStrings(_model.ApplicationGlobalState);
        }

        private void CancelOperations()
        {
            try
            {
                CancellationTokenSource source = _container.Resolve<CancellationTokenSource>("DataCancellationSource");
                source.Cancel();
            }
            catch (Exception ex)
            {

            }
        }

        public void LaunchOperation()
        {
            _model.BeginOperation.Invoke();
        }

        public void ShowError(string newError)
        {
            ErrorString = newError;
        }

        /// <summary>
        /// Changes operations information string in accordance with an input string
        /// </summary>
        private void ChangeUIControlStrings(string newState)
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
