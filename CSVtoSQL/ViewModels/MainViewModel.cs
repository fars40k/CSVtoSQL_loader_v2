using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfStarter.UI.Localisation;
using WpfStarter.UI.Models;
using Microsoft.Win32;
using static WpfStarter.UI.Models.MainModel;
using MainWindow = WpfStarter.UI.Views.MainWindow;
using WpfStarter.UI.Views.DialogViews;
using System.Configuration;

namespace WpfStarter.UI.ViewModels
{
    /// <summary>
    /// Содержит модель представления для основного окна
    /// </summary>
    /// 

    public class MainViewModel : INotifyPropertyChanged
    {
        #region UI_Binded_Strings

        private string userHelpString;
        private string errorString;
        private string fileString;

        public string UserHelpString
        {
            get => userHelpString;
            set
            {
                userHelpString = value;
                OnPropertyChanged("UserHelpString");
            }
        }
        public string ErrorString
        {
            get => errorString;
            set
            {
                errorString = value;
                OnPropertyChanged("ErrorString");
            }
        }
        public string FileString
        {
            get => fileString;
            set
            {
                fileString = value;
                OnPropertyChanged("FileString");
            }
        }

        /// <summary>
        /// On changes in binded string properties changes the text of the View
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region UI_Commands

        private void SetDefaultCommandBindings()
        {
            mainWindow.FileLoadButton.Command = ApplicationCommands.Open;
            CommandBinding commandBinding = new CommandBinding();
            commandBinding.Command = ApplicationCommands.Open;
            commandBinding.Executed += CommandBinding_Open;
            mainWindow.FileLoadButton.CommandBindings.Add(commandBinding);

            commandBinding = new CommandBinding();
            mainWindow.OpCancelButton.Command = ApplicationCommands.CancelPrint;
            commandBinding.Command = ApplicationCommands.CancelPrint;
            commandBinding.Executed += CommandBinding_CancelOp;
            mainWindow.OpCancelButton.CommandBindings.Add(commandBinding);
        }

        /// <summary>
        /// Обработчик команды получения строки подключения
        /// </summary>
        private void CommandBinding_Database(object sender, ExecutedRoutedEventArgs e)
        {
            /*var dialog = new StringInput(Model.ExstractDBStringFromFile());
            dialog.ShowDialog();
            string returnedString = dialog.CurrentConnString.Replace(" ", string.Empty);*/
            mainModel.ConnectionStringAcquired("");
        }

        /// <summary>
        /// Open file command handler
        /// </summary>
        private void CommandBinding_Open(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            mainModel.SetAppGlobalState(GlobalState.Disabled);
            dlg.DefaultExt = ".csv";
            dlg.Filter = Localisation.Strings.OpenFileFormat + " (*.csv)|*csv";
            dlg.ShowDialog();
            ErrorNotify.ClearError();
            if (dlg.FileName != "")
            {
                mainModel.FileSelected(dlg.FileName);              
                string str = dlg.FileName;
                str = " " + str.Substring(str.LastIndexOf(@"\"), str.Length - str.LastIndexOf(@"\"));
                FileString = str;
            } else
            {
                mainModel.SetAppGlobalState(GlobalState.AppLoaded);
            }

        }

        private void CommandBinding_CancelOp(object sender, ExecutedRoutedEventArgs e)
        {
            mainModel.OperationCancelCall.Invoke();
        }

        #endregion

        private MainWindow mainWindow;
        private MainModel mainModel { get; set; }

        public Action UpdateFileInfoOnView;
        public Action UpdateOperationsList;
        public Action UpdateUserUI;
        public Action<AppError> ShowError;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(MainWindow main)
        {
            mainWindow = main;
            SetDefaultReferences();
            SetDefaultCommandBindings();           
        }

        /// <summary>
        /// Binds delegates, events and methods
        /// </summary>
        private void SetDefaultReferences()
        {
            UpdateUserUI += UpdateUIBindedStrings;
            UpdateUserUI += UpdateUIElementsAvalability;
            UpdateOperationsList += SetOperationsToList;
            ShowError += ReadErrorsAndShow;
        }

        /// <summary>
        /// Обновляет поле вывода ошибок в окне
        /// </summary>        
        private void ReadErrorsAndShow(AppError obj)
        {
            string errorAppend = "";
            if (obj.Code == "Empty")
            {
                ErrorString = "";
            }
            else
            {
                if (obj.Code != "")
                {
                    errorAppend = obj.Code + "\n";
                }
                if (obj.Message != "")
                {
                    errorAppend += obj.Message;
                }
                ErrorString = errorAppend;
            }
        }

        /// <summary>
        /// Принимает ссылку на Model
        /// </summary>
        /// <param name="newModel"></param>
        public void SetModelReference(MainModel newModel)
        {
            if (mainModel == null) mainModel = newModel;
            mainModel.SetAppGlobalState(MainModel.GlobalState.AppLoaded);
        }

        /// <summary>
        /// Заплняет список доступных операций приложения
        /// </summary>
        private void SetOperationsToList()
        {
            if (mainModel.GetAppGlobalState() == "FileDecided")
            {
                if (mainWindow.FunctionsList.Children.Count != mainModel.Operations.Count)
                {
                    for (int OpId = 0; OpId < mainModel.Operations.Count; OpId++)
                    {
                        Button btn = new Button();
                        btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                        btn.Width = mainWindow.FunctionsList.Width;
                        btn.MinHeight *= 2;
                        Operation tmpOp = mainModel.Operations[OpId] as Operation;
                        btn.Content = tmpOp.Description;
                        btn.Click += tmpOp.Select;
                        if (!mainWindow.FunctionsList.Children.Contains(btn))
                        {
                            mainWindow.FunctionsList.Children.Add(btn);
                        }
                    }
                }
            }
            else
            {
                mainWindow.FunctionsList.Children.RemoveRange(0, mainModel.Operations.Count);
            }
        }

        /// <summary>
        /// Receives application global state from Model and accordingly changes 
        /// IsEnabled property of UI elements 
        /// </summary>
        private void UpdateUIElementsAvalability()
        {      
            switch (mainModel.GetAppGlobalState())
            {
                case "AppLoaded":
                    {
                        mainWindow.FileLoadButton.IsEnabled = true;
                        mainWindow.OpCancelButton.IsEnabled = false;
                        UpdateOperationsList.Invoke();
                        break;
                    }
                case "FileDecided":
                    {
                        mainWindow.FileLoadButton.IsEnabled = true;
                        mainWindow.OpCancelButton.IsEnabled = false;
                        break;
                    }
             
                case "DbConnected":
                    {
                        mainWindow.FileLoadButton.IsEnabled = true;
                        mainWindow.OpCancelButton.IsEnabled = true;
                        UpdateOperationsList.Invoke();
                        break;
                    }

                case "Disabled":
                    {
                        mainWindow.FileLoadButton.IsEnabled = false;
                        mainWindow.OpCancelButton.IsEnabled = true;
                        break;
                    }

                case "CriticalError":
                default:
                    {
                        mainWindow.FileLoadButton.IsEnabled = false;
                        mainWindow.OpCancelButton.IsEnabled = false;
                        break;
                    }
            }

        }

        /// <summary>
        /// Receives application global state from Model and accordingly changes 
        /// binded strings
        /// </summary>
        private void UpdateUIBindedStrings()
        {
            switch (mainModel.GetAppGlobalState())
            {
                case "AppLoaded":
                    {
                        UserHelpString = Localisation.Strings.Help1;
                        FileString = Localisation.Strings.File;
                        break;
                    }

                case "FileDecided":
                    {
                        UserHelpString = Localisation.Strings.Help2;
                        break;
                    }
                case "DbConnected":
                    {
                        UserHelpString = Localisation.Strings.Help4;
                        break;
                    }

                case "Disabled":
                    {
                        UserHelpString = Localisation.Strings.Help6;
                        break;
                    }

                case "CriticalError":
                default:
                    {
                        UserHelpString = Localisation.Strings.CriticalErrorHelp;
                        ErrorString = Localisation.Strings.CriticalError;
                        break;
                    }
            }



        }

        

    }
}
