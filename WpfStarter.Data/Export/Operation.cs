using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfStarter.Data.Export
{
    public class Operation : IDatabaseAction
    {
        public string Description { get; set; }

        public Operation(string newDescription)
        {
            if (Description == null) Description = newDescription;
        }

        public Operation()
        {

        }

        public override string ToString()
        {
            return Description ?? " ";
        }

        public virtual string Run()
        {
            MessageBox.Show("Empty base class running requested");
            return "";
        }
    }
}
