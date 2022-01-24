using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.Data.Export
{
    internal interface ISavePathSelectionRequired : IDatabaseAction
    {
        public string TargetFormat { get; set; } 

        public void SetSavePath(string newFilePath);
    }
}
