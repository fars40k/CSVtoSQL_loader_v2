using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSVtoSQL.Models.Operations;
using CSVtoSQL.ViewModels;

namespace CSVtoSQL.Models
{
    public partial class MainModel
    {
        private static EnumGlobalState AppGlobalState { get; set; }

        public Action OperationCancelCall;
        public Action OperationsListRequest;
        public Action<string> LoadFromFileRequested;
        public Action<string> FileSelected;
        public Action<string> ConnectionStringAcquired;
        public Action<string> FileSavePathSelected;
        public event Action<AppError> ErrorHappened;
        public event Action AppStateChanged;

        private FileAnalyser fileAnalyser;
        private DBWorker dbWorker;
        private CancellationTokenSource cancelTokenSource;
        public CancellationToken cancellationToken;

        public MainViewModel? mainViewModel { get; private set; }
        public Dictionary<int, object> Operations { get; private set; } = new Dictionary<int, object>();

        public bool IsAssyncRunned { get; private set; } = false;

        public MainModel(MainViewModel model)
        {
            mainViewModel = model;
            cancelTokenSource = new CancellationTokenSource();
            cancellationToken = cancelTokenSource.Token;
            ApplyDefaultEventRouting();
            FillOperationsList();
            ErrorNotify.SetUINotifyMethod(mainViewModel.ShowError);
        }

        /// <summary>
        /// Связывает делегаты,события и методы
        /// </summary>
        public void ApplyDefaultEventRouting()
        {
            AppStateChanged += mainViewModel.UpdateUserUI;
            FileSelected += (value) =>
            {
                fileAnalyser = new FileAnalyser(this, value);
            };
            ConnectionStringAcquired += (value) =>
            {
                dbWorker = new DBWorker(this, value);
                FileSavePathSelected += dbWorker.SetWriteToFilePath;
                LoadFromFileRequested += dbWorker.BeginLoadToDatabase;
                dbWorker.SetFileSourcePath(fileAnalyser.GetPath());
            };
            OperationCancelCall += () =>
            {
                if (IsAssyncRunned)
                {
                    cancelTokenSource.Cancel();
                }

            };
        }

        /// <summary>
        /// Заполняет список доступных операций
        /// </summary>
        private void FillOperationsList()
        {
            if (Operations.Count == 0)
            {
                Operations.Add(0, new DBSaveToFile(this, Localisation.Strings.OpConvToXML, "xml"));
                Operations.Add(1, new DBSaveToFile(this, Localisation.Strings.OpConvToXLSX, "xlsx"));
                Operations.Add(2, new DBLoadFromFile(this, Localisation.Strings.OpCSVtoSQLExist, "Old"));
            }
        }

        /// <summary>
        /// Возвращает текущее состояние приложения
        /// </summary>
        public string GetAppGlobalState()
        {
            return AppGlobalState.ToString();
        }

        /// <summary>
        /// Устанавливает новое состояние приложения
        /// </summary>
        /// <param name="newState"></param>
        public void SetAppGlobalState(EnumGlobalState newState)
        {
            AppGlobalState = newState;
            AppStateChanged.Invoke();
        }

        /// <summary>
        /// Получает строку подключения из файла
        /// </summary>
        public string ExstractDBStringFromFile()
        {
            string path = Environment.CurrentDirectory + @"\Connection.txt";
            if (!File.Exists(path))
            {
                File.Create(path);
                return "";
            }
            using (FileStream fstream = File.OpenRead(path))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                return textFromFile;
            }
        }

        /// <summary>
        /// Записывает строку подключения в файл
        /// </summary>
        public void WriteDBStringToFile(string newDBstring)
        {
            string path = Environment.CurrentDirectory + @"\Connection.txt";
            File.WriteAllText(path,String.Empty);
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(newDBstring);
                fstream.Write(array,0, newDBstring.Length);
            }           

        }

        
    } 
}
