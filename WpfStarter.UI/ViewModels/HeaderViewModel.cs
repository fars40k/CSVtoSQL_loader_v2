using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.UI.ViewModels
{
    internal class HeaderViewModel : BindableBase
    {
        private string _helpString;
        private string _fileNameString;

        public string HelpString
        {
            get => _helpString;
            private set => SetProperty(ref _helpString, value);
        }

        public string FileNameString
        {
            get => _fileNameString;
            private set => SetProperty(ref _fileNameString, value);
        }

        public void SelectFileCommand()
        {

        }
    }
}
