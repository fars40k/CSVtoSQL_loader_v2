using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAssembly.Classes
{
    public class EPPLusSaver : EntityToExcel
    {
        public string filePath { get; private set; }

        public EPPLusSaver(string newFilePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            filePath = newFilePath;
        }

        public override bool Run()
        {
            throw new NotImplementedException();
        }
    }
}
