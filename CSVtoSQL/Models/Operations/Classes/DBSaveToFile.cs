using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfStarter.UI.Models.Operations;
using WpfStarter.UI.Models.Operations.Converters;

namespace WpfStarter.UI.Models.Operations
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
        /// Shows savefile dialog window and returns savepath or empty string ("") 
        /// </summary>
        public string GetSavePath(string targetFormat)
        {
            SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = "." + targetFormat;
            dlg.FileName = nameGenerator.GenerateName();
            dlg.Filter = Localisation.Strings.FileSave + " (*." + targetFormat + ")|*" + targetFormat;
            dlg.ShowDialog();
            return dlg.FileName;
        }
    }
}
