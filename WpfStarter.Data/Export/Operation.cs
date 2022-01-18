using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.Data.Export
{
    public class Operation : IDatabaseAction
    {
        public string Description { get; private set; }

        public Operation(string newDescription)
        {
            Description = newDescription;
        }

        public override string ToString()
        {
            return Description;
        }

        public virtual string ShowSaveFileDialog(string targetFormat)
        {
            SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            Random random = new Random();
            dlg.DefaultExt = "." + targetFormat;
            dlg.FileName = random.Next(0, 9000).ToString();
            dlg.Filter = "Save as" + " (*." + targetFormat + ")|*" + targetFormat;
            dlg.ShowDialog();
            if (dlg.FileName != "")
            {
                return dlg.FileName;
            }
            return "";
        }
    }
}
