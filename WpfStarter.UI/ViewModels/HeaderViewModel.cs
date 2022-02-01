using Microsoft.Win32;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using WpfStarter.UI.Models;

namespace WpfStarter.UI.ViewModels
{
    internal class HeaderViewModel : BindableBase
    {
        private Model _model;
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
            _model = containerProvider.Resolve<Models.Model>();
            _model.AppStateChanged += ChangeUIControlStrings;
            ChangeUIControlStrings(_model.ApplicationGlobalState);
        }

        /// <summary>
        /// Changes user help string in accordance with an input string
        /// </summary>
        private void ChangeUIControlStrings(string newState)
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

        /// <summary>
        /// Select source file logic
        /// </summary>
        public void SelectFile()
        {
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = Localisation.Strings.OpenFileFormat + " (*.csv)|*csv";
            dlg.ShowDialog();
            if ((dlg.FileName != "") && (dlg.FileName.Contains(".csv")))
            {
                ErrorNotify.ClearError();

                _model.FileSelected(dlg.FileName);

                string str = dlg.FileName;
                str = " " + str.Substring((str.LastIndexOf(@"\") + 1), str.Length - (str.LastIndexOf(@"\") + 1));
                FileNameString = str;
            }

        }
    }
}
