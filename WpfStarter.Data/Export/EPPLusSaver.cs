using System.Resources;
using System.Linq.Dynamic.Core;
using OfficeOpenXml;
using Prism.Ioc;

namespace WpfStarter.Data.Export
{
    public class EPPLusSaver : Operation, ILinqBuildRequired, ISavePathSelectionRequired
    {
        public EPPLusSaver(IContainerExtension container)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var ResourceManager = container.Resolve<ResourceManager>();
            Description = ResourceManager.GetString("OpConvToXLSX") ?? "missing";
            TargetFormat = ".xlsx";
        }

        public string FilePath { get; private set; } = "";
        public string TargetFormat { get; set; }
        public string LINQExpression { get; set; } = "";

        public override string Run()
        {
            try
            {
                int currentRow = 1;
                using (PersonsContext pC = new PersonsContext())
                {
                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = Environment.UserName;
                        ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Querry " + DateTime.Now.ToString());

                        // Changes source of items if LINQ Expression contains filtering data conditions

                        object list;
                        if (LINQExpression == "")
                        {
                            list = pC.Persons;
                        }
                        else
                        {
                            list = pC.Persons
                                         .Where(LINQExpression)
                                         .ToList();
                        }

                        foreach (Person person in (IEnumerable<Person>)list)
                        {
                            WritePersonToRowOfCells(++currentRow, excelWorksheet, person);
                        }
                        excelWorksheet.Cells.AutoFitColumns();

                        excelPackage.SaveAs(FilePath);
                    }
                }

                return "true";
            }
            catch (Exception ex)
            {
                return "false";
            }
        }

        public void WritePersonToRowOfCells(int row,ExcelWorksheet sheet,Person person)
        {
            sheet.Cells[row, 1].Value = person.ID;
            sheet.Cells[row, 2].Value = person.Date.ToString();
            sheet.Cells[row, 3].Value = person.FirstName;
            sheet.Cells[row, 4].Value = person.LastName;
            sheet.Cells[row, 5].Value = person.SurName;
            sheet.Cells[row, 6].Value = person.City;
            sheet.Cells[row, 7].Value = person.Country;
        }

        public void SetSavePath(string newFilePath)
        {
            FilePath = newFilePath;
        }
    }
}