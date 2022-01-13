using System;
using System.IO;
using System.Text;
using System.Windows;
using static WpfStarter.UI.Models.MainModel;

namespace WpfStarter.UI.Models
{
    internal class FileAnalyser
    {
        MainModel Model;
        public FileAnalyser(object sender,string value)
        {
            Model = sender as MainModel;
            Path = value;
            Analyse();
        }

        public string Path { get; set; }

        /// <summary>
        /// Получает колличество строк и определяет полон ли файл
        /// </summary>
        public int Analyse()
        {
            try
             {
                using (FileStream fileStream = File.OpenRead(Path))
                {
                    byte[] array = new byte[fileStream.Length];
                    fileStream.Read(array, 0, array.Length);
                    string? textFromFile = Encoding.Default.GetString(array);
                    if (textFromFile == "")
                    {
                        
                        throw new ArgumentException(Localisation.Strings.ErrorFileEmpty);
                    } else
                    {
                        Model.SetAppGlobalState(GlobalState.FileSelected);
                    }
                }
             }
             catch (ArgumentException ex)
             {
                ErrorNotify.NewError(new AppError(Localisation.Strings.ErrorFileEmpty, ""));
                Model.SetAppGlobalState(GlobalState.AppLoaded);
            }
             catch (Exception e)
             {
                ErrorNotify.NewError(new AppError(Localisation.Strings.ErrorFileRead, e.ToString()));
             }
             
            return (0);
        }
       
        /// <summary>
        /// Возвращает переданный путь к файлу 
        /// </summary>
       public string GetPath()
        {
            return Path;
        }
    }
}