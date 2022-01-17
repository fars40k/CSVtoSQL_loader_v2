﻿using Microsoft.Win32;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            HelpString = Localisation.Strings.Help1;
            SelectFileCommand = new DelegateCommand(SelectFile);
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
                model.SourceFile = dlg.FileName;
                string str = dlg.FileName;
                str = " " + str.Substring((str.LastIndexOf(@"\") + 1), str.Length - (str.LastIndexOf(@"\") + 1));
                FileNameString = str;
            }
            else
            {
               
            }
        }
    }
}
