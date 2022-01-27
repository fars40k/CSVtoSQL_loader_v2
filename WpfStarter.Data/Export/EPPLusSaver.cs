using System.Resources;
using System.Linq.Dynamic.Core;
using OfficeOpenXml;
using Prism.Ioc;

namespace WpfStarter.Data.Export
{
    public class EPPLusSaver : Operation, IRequiringBuildLinq, IRequiringSavepathSelection
    {
        public string FilePath { get; private set; } = "";
        public string TargetFormat { get; set; }
        public string LinqExpression { get; set; } = "";

        public EPPLusSaver(IContainerExtension container)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var ResourceManager = container.Resolve<ResourceManager>();
            _description = ResourceManager.GetString("OpConvToXLSX") ?? "missing";
            TargetFormat = ".xlsx";
        }

        public override string Run()
        {
            try
            {
                int currentRow = 1;

                PersonsContext pC = new PersonsContext();

                ExcelPackage excelPackage = new ExcelPackage();

                excelPackage.Workbook.Properties.Author = Environment.UserName;
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Querry " + DateTime.Now.ToString());

                // Changes source of items if LINQ Expression contains filtering data conditions

                object list;
                if (LinqExpression == "")
                {
                    list = pC.Persons;
                }
                else
                {
                    list = pC.Persons
                                 .Where(LinqExpression)
                                 .ToList();
                }

                foreach (Person person in (IEnumerable<Person>)list)
                {
                    WritePersonToRowOfCells(++currentRow, excelWorksheet, person);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        excelWorksheet.Cells[currentRow + 2, 1].Value = "Canceled";
                        break;
                    }
                }

                excelWorksheet.Cells.AutoFitColumns();
                excelPackage.SaveAs(FilePath);

                if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();
                return (currentRow-1).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void WritePersonToRowOfCells(int row, ExcelWorksheet sheet, Person person)
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