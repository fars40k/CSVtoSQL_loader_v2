using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAssembly.Classes
{
    internal class CSVHelperLoader : FileCSVtoSQL
    {
        public string filePath { get; private set; }

        public CSVHelperLoader(string newfilePath)
        {
            this.filePath = newfilePath;
        }

        public override bool Run()
        {
            throw new NotImplementedException();
        }
    }
}
