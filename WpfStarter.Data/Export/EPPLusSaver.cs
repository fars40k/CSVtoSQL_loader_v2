using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace WpfStarter.Data.Export
{
    public class EPPLusSaver : ILinqBuildRequire
    {
        public string filePath { get; private set; }

        public EPPLusSaver(string newFilePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            filePath = newFilePath;
        }

        public bool Run()
        {
            int currentRow = 1;
            using (PersonsContext pC = new PersonsContext())
            {
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Author = Environment.UserName;

                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Querry " + DateTime.Now.ToString());

                    foreach (Person person in pC.Persons)
                    {
                        WritePersonToStartCells(++currentRow, excelWorksheet, person);
                    }

                    excelPackage.SaveAs(filePath);
                }
            }
            return true;
        }

        public void WritePersonToStartCells(int row,ExcelWorksheet sheet,Person person)
        {
            sheet.Cells[row, 1].Value = person.ID;
            sheet.Cells[row, 2].Value = person.Date;
            sheet.Cells[row, 3].Value = person.FirstName;
            sheet.Cells[row, 4].Value = person.LastName;
            sheet.Cells[row, 5].Value = person.SurName;
            sheet.Cells[row, 6].Value = person.City;
            sheet.Cells[row, 7].Value = person.Country;
        }

    }
}