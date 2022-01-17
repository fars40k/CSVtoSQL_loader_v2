using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public FooterViewModel(IContainerProvider containerProvider)
        {
            model = containerProvider.Resolve<Models.Model>();
            ErrorNotify.SetUINotifyMethod(ShowError);
        }

        public void OperationLaunchCommand()
        {

        }

        public void ShowError(string newError)
        {

        }
    }
}
