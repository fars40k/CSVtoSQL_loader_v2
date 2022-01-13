using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.UI.Models.Operations.Converters
{
    internal interface ISaveFile
    {
        public string GetSavePath(string targetFormat);

    }
}
