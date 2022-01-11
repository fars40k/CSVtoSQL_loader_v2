using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.Data.Export
{
    public class EPPLusSaver : EntityToExcel
    {
        public string filePath { get; private set; }

        public EPPLusSaver(string newFilePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            filePath = newFilePath;
        }

        public override bool Run()
        {
            using (PersonsContext pC = new PersonsContext())
            {
                var maxId = pC.Database.SqlQuery<Person>("SELECT MAX(ID) FROM dbo.Persons","");
                
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Querry" + DateTime.Now.ToString());

                }
            }
            return true;

        }

    }
}