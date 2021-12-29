using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CSVtoSQL.Models.Operations;
using CSVtoSQL.Models.Operations.Converters;

namespace CSVtoSQL.Models.Operations
{
    /// <summary>
    /// Класс операции записи в файл
    /// </summary>
    public class DBSaveToFile : Operation, ISaveFile
    {
        private string targetFormat;
        private ICreateFile createFile;
        private NameGenerator nameGenerator;

        public DBSaveToFile(MainModel model,string newDescription,string Format) : base(model,newDescription)
        {
            this.targetFormat = Format;
            SetFormatDependent(targetFormat);
        }

        /// <summary>
        /// Задает классы содержащие методы записи
        /// </summary>
        /// <param name="targetFormat"></param>
        private void SetFormatDependent(string targetFormat)
        {
            switch (targetFormat)
            {
                case "xml":
                    {
                        createFile = new CreateXMLFile();
                        break;
                    }

                case "xlsx":
                default:
                    {
                        createFile = new CreateExcelFile();
                        break;
                    }

            }
            nameGenerator = new NameGenerator();
        }

        public override void Select(object sender, RoutedEventArgs e)
        {
           string savePath =  GetSavePath(targetFormat);
           if (savePath != "")
            {

            }
        }

        public string GetSavePath(string targetFormat)
        {
            SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = "." + targetFormat;
            dlg.FileName = nameGenerator.GenerateName();
            dlg.Filter = Localisation.Strings.FileSave + " (*." + targetFormat + ")|*" + targetFormat;
            dlg.ShowDialog();
            if (dlg.FileName != "")
            {

            }
            return ("");
        }
    }
}
