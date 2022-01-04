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
        private NameGenerator nameGenerator;

        public DBSaveToFile(MainModel model,string newDescription,string Format) : base(model,newDescription)
        {
            this.targetFormat = Format;
            nameGenerator = new NameGenerator(); 
        }

        public override void Select(object sender, RoutedEventArgs e)
        {
           string savePath =  GetSavePath(targetFormat);
           mainModel.FileSavePathSelected.Invoke(savePath);
        }

        /// <summary>
        /// Вызывает диалог сохранения файла для загрузки из базы данных
        /// </summary>
        public string GetSavePath(string targetFormat)
        {
            SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = "." + targetFormat;
            dlg.FileName = nameGenerator.GenerateName();
            dlg.Filter = Localisation.Strings.FileSave + " (*." + targetFormat + ")|*" + targetFormat;
            dlg.ShowDialog();
            if (dlg.FileName != "")
            {
                return dlg.FileName;
            }
            return ("");
        }
    }
}
