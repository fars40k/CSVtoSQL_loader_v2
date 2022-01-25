using Microsoft.Win32;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using WpfStarter.UI.Models;

namespace WpfStarter.UI.ViewModels
{
    internal class HeaderViewModel : BindableBase
    {
        private Model model;
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

        public HeaderViewModel(IContainerProvider containerProvider)
        {
            model = containerProvider.Resolve<Models.Model>();
            model.AppStateChanged += Model_AppStateChanged;
            model.NotifyAppGlobalState();
        }

        /// <summary>
        /// Changes user help string in accordance with input string
        /// </summary>
        private void Model_AppStateChanged(string newState)
        {
            switch (newState)
            {
                case "DbConnectionFailed":
                    {
                        HelpString = Localisation.Strings.Help2;                        
                        break;
                    }
                case "DbConnected":
                    {
                        HelpString = Localisation.Strings.Help1;
                        SelectFileCommand = new DelegateCommand(SelectFile);
                        break;
                    }

                case "FileSelected":
                    {
                        HelpString = Localisation.Strings.Help4;
                        break;
                    }

                case "Disabled":
                    {
                        HelpString = Localisation.Strings.Help6;
                        break;
                    }

                case "CriticalError":
                default:
                    {
                        HelpString = Localisation.Strings.CriticalErrorHelp;
                        break;
                    }
            }
        }

        public DelegateCommand SelectFileCommand { get; private set; }

        public void SelectFile()
        {
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = Localisation.Strings.OpenFileFormat + " (*.csv)|*csv";
            dlg.ShowDialog();
            if (dlg.FileName != "")
            {
                ErrorNotify.ClearError();
                model.FileSelected(dlg.FileName);
                string str = dlg.FileName;
                str = " " + str.Substring((str.LastIndexOf(@"\") + 1), str.Length - (str.LastIndexOf(@"\") + 1));
                FileNameString = str;
            }

        }
    }
}
