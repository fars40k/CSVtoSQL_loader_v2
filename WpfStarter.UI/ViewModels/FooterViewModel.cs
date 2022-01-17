using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.UI.ViewModels
{
    internal class FooterViewModel : BindableBase
    {
        private string _errorString;

        public string ErrorString
        {
            get => _errorString;
            private set => SetProperty(ref _errorString, value);
        }

        public FooterViewModel()
        {

        }

        public void OperationLaunchCommand()
        {

        }
    }
}
