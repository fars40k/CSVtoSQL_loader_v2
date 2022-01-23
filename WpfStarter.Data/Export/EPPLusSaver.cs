using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Resources;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Prism.Ioc;

namespace WpfStarter.Data.Export
{
    public class EPPLusSaver : Operation, ILinqBuildRequired, ISavePathSelectionRequired
    {
        public EPPLusSaver(IContainerExtension container)
        {
            var ResourceManager = container.Resolve<ResourceManager>();
            Description = ResourceManager.GetString("OpConvToXLSX") ?? "missing";
            targetFormat = ".xlsx";
        }

        public string filePath { get; private set; }
        public string targetFormat { get; set; }
        public string LINQExpression { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Run()
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
                        WritePersonToRowOfCells(++currentRow, excelWorksheet, person);
                    }

                    // Checks if file exist and save in incremental marked file
                    string nonDuplicatefilePath = filePath;
                    if (File.Exists(filePath))
                    {
                        int increment = 0;

                        while (File.Exists(nonDuplicatefilePath))
                        {
                            increment++;
                            nonDuplicatefilePath = filePath.Replace(".", ("_" + increment.ToString() + "."));
                        }
                    }
                    excelPackage.SaveAs(nonDuplicatefilePath);
                }
            }
            return "true";
        }

        public void WritePersonToRowOfCells(int row,ExcelWorksheet sheet,Person person)
        {
            sheet.Cells[row, 1].Value = person.ID;
            sheet.Cells[row, 2].Value = person.Date;
            sheet.Cells[row, 3].Value = person.FirstName;
            sheet.Cells[row, 4].Value = person.LastName;
            sheet.Cells[row, 5].Value = person.SurName;
            sheet.Cells[row, 6].Value = person.City;
            sheet.Cells[row, 7].Value = person.Country;
        }

        public void SetSavePath(string newFilePath)
        {
            filePath = newFilePath;
        }
    }
}