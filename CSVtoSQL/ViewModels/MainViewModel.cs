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
using CSVtoSQL.Localisation;
using CSVtoSQL.Models;
using Microsoft.Win32;
using static CSVtoSQL.Models.MainModel;
using MainWindow = CSVtoSQL.Views.MainWindow;
using CSVtoSQL.Views.DialogViews;
using System.Configuration;

namespace CSVtoSQL.ViewModels
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
        private string databaseString;
        private string fileAnalysed;

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

        public string DataBaseString
        {
            get => databaseString; 
            set
            {
                databaseString = value;
                OnPropertyChanged("DataBaseString");
            }
        }
        
        public string FileAnalysed
        {
            get => fileAnalysed;
            set
            {
                fileAnalysed = value;
                OnPropertyChanged("FileAnalysedString");
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
            mainWindow.DatabaseButton.Command = ApplicationCommands.Print;
            commandBinding.Command = ApplicationCommands.Print;
            commandBinding.Executed += CommandBinding_Database;
            mainWindow.DatabaseButton.CommandBindings.Add(commandBinding);
        }

        /// <summary>
        /// Обработчик команды получения строки подключения
        /// </summary>
        private void CommandBinding_Database(object sender, ExecutedRoutedEventArgs e)
        {
            /*var dialog = new StringInput(Model.ExstractDBStringFromFile());
            dialog.ShowDialog();
            string returnedString = dialog.CurrentConnString.Replace(" ", string.Empty);*/
            Model.ConnectionStringAcquired("");
        }

        /// <summary>
        /// Обработчик команды открытия файла
        /// </summary>
        private void CommandBinding_Open(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Model.SetAppGlobalState(EnumGlobalState.Disabled);
            dlg.DefaultExt = ".csv";
            dlg.Filter = Localisation.Strings.OpenFileFormat + " (*.csv)|*csv";
            dlg.ShowDialog();
            ErrorNotify.ClearError();
            if (dlg.FileName != "")
            {
                Model.FileSelected(dlg.FileName);              
                string str = dlg.FileName;
                str = " " + str.Substring(str.LastIndexOf(@"\"), str.Length - str.LastIndexOf(@"\"));
                FileString = str;
            } else
            {
                Model.SetAppGlobalState(EnumGlobalState.AppLoaded);
            }

        }


        #endregion

        private MainWindow mainWindow;
        private MainModel? Model { get; set; }

        public Action UpdateFileInfoOnView;
        public Action UpdateOperationsList;
        public Action UpdateUserUI;
        public Action<AppError> ShowError;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(MainWindow main)
        {
            mainWindow = main;
            SetDefaultEventRouting();
            SetDefaultCommandBindings();           
        }

        /// <summary>
        /// Связывает делегаты,события и методы
        /// </summary>
        private void SetDefaultEventRouting()
        {
            UpdateUserUI += UpdateDynamicUIStrings;
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
            if (Model == null) Model = newModel;
            Model.SetAppGlobalState(MainModel.EnumGlobalState.AppLoaded);
        }

        /// <summary>
        /// Заплняет список доступных операций приложения
        /// </summary>
        private void SetOperationsToList()
        {
            if (Model.GetAppGlobalState() == "DbConnected")
            {
                if (mainWindow.FunctionsList.Children.Count != Model.Operations.Count)
                {
                    for (int OpId = 0; OpId < Model.Operations.Count; OpId++)
                    {
                        Button btn = new Button();
                        btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                        btn.Width = mainWindow.FunctionsList.Width;
                        btn.MinHeight *= 2;
                        Operation tmpOp = Model.Operations[OpId] as Operation;
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
                mainWindow.FunctionsList.Children.RemoveRange(0, Model.Operations.Count);
            }
        }

        /// <summary>
        /// Получает состояние приложения и изменяет доступность интерфейса
        /// </summary>
        private void UpdateUIElementsAvalability()
        {      
            switch (Model.GetAppGlobalState())
            {
                case "AppLoaded":
                    {
                        mainWindow.FileLoadButton.IsEnabled = true;
                        mainWindow.DatabaseButton.IsEnabled = false;
                        mainWindow.ExecuteButton.IsEnabled = false;
                        mainWindow.FilterButton.IsEnabled = false;
                        mainWindow.OpCancelButton.IsEnabled = false;
                        UpdateOperationsList.Invoke();
                        break;
                    }
                case "FileDecided":
                    {
                        mainWindow.FileLoadButton.IsEnabled = true;
                        mainWindow.DatabaseButton.IsEnabled = true;
                        mainWindow.ExecuteButton.IsEnabled = false;
                        mainWindow.FilterButton.IsEnabled = false;
                        mainWindow.OpCancelButton.IsEnabled = false;
                        break;
                    }
             
                case "DbConnected":
                    {
                        mainWindow.FileLoadButton.IsEnabled = true;
                        mainWindow.DatabaseButton.IsEnabled = true;
                        mainWindow.ExecuteButton.IsEnabled = false;
                        mainWindow.FilterButton.IsEnabled = true;
                        mainWindow.OpCancelButton.IsEnabled = false;
                        UpdateOperationsList.Invoke();
                        break;
                    }

                case "Disabled":
                    {
                        mainWindow.FileLoadButton.IsEnabled = false;
                        mainWindow.DatabaseButton.IsEnabled = false;
                        mainWindow.ExecuteButton.IsEnabled = false;
                        mainWindow.FilterButton.IsEnabled = false;
                        mainWindow.OpCancelButton.IsEnabled = false;
                        break;
                    }

                case "CriticalError":
                default:
                    {
                        mainWindow.FileLoadButton.IsEnabled = false;
                        mainWindow.DatabaseButton.IsEnabled = false;
                        mainWindow.ExecuteButton.IsEnabled = false;
                        mainWindow.FilterButton.IsEnabled = false;
                        mainWindow.OpCancelButton.IsEnabled = false;
                        break;
                    }
            }

        }
      
        /// <summary>
        /// Получает состояние приложения из Model и изменяет связанные строки
        /// </summary>
        private void UpdateDynamicUIStrings()
        {
            switch (Model.GetAppGlobalState())
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
                        DataBaseString = Localisation.Strings.DataBase;
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

        /// <summary>
        /// На основе изменений в связанных свойствах ViewModel изменяет текст View
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

    }
}
