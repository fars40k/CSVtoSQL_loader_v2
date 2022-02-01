using System.Resources;
using System.Linq.Dynamic.Core;
using OfficeOpenXml;
using Prism.Ioc;

namespace WpfStarter.Data.Export
{
    public class EPPLusSaver : Operation, IRequiringBuildLinq, IRequiringSavepathSelection, IParametrisedAction<Inference>
    {
        public string FilePath { get; private set; } = "";
        public string TargetFormat { get; set; }
        public string LinqExpression { get; set; } = "";
        public Inference Settings { get; set; }

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
                ExcelWorksheet excelWorksheet = 
                    excelPackage.Workbook.Worksheets.Add("Querry " + DateTime.Now.ToString());

                // Changes source of items if the LINQ Expression contains filtering data
                // conditions and gets the total value of the records
                int totalEntries = 0;
                object recordsList;
                if (LinqExpression == "")
                {
                    recordsList = pC.Persons;
                    totalEntries = pC.Persons.Count();
                }
                else
                {
                    recordsList = pC.Persons.Where(LinqExpression)
                                     .ToList();

                    List<Person> persons = (List<Person>)recordsList;
                    totalEntries = persons.Count();
                }

                WriteHeadLine(excelWorksheet);

                foreach (Person person in (IEnumerable<Person>)recordsList)
                {
                    WritePersonToRowOfCells(++currentRow, excelWorksheet, person);

                    if (_cancelToken.IsCancellationRequested)
                    {
                        excelWorksheet.Cells[currentRow + 2, 1].Value = "Canceled";
                        break;
                    }

                    if (currentRow % 100 == 0)
                    {
                        _progress.Report((currentRow) + " / " + totalEntries);
                    }
                }

                pC.Dispose();

                excelWorksheet.Cells.AutoFitColumns();
                excelPackage.SaveAs(FilePath);

                if (Settings != null) Settings.TotalProcessed = totalEntries;

                if (_cancelToken.IsCancellationRequested) throw new OperationCanceledException();
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Writes table header
        /// </summary>
        public void WriteHeadLine(ExcelWorksheet sheet)
        {
            List<string> ContextPropertyNames = new List<string>()
                             {
                                 nameof(Person.ID),
                                 nameof(Person.Date),
                                 nameof(Person.FirstName),
                                 nameof(Person.SurName),
                                 nameof(Person.LastName),
                                 nameof(Person.City),
                                 nameof(Person.Country)
                             };
            int column = 1;
            foreach(string Property in ContextPropertyNames)
            {
                sheet.Cells[1, column++].Value = Property;
            }
        }

        public void WritePersonToRowOfCells(int row, ExcelWorksheet sheet, Person person)
        {
            sheet.Cells[row, 1].Value = person.ID;
            sheet.Cells[row, 2].Value = person.Date.ToString();
            sheet.Cells[row, 3].Value = person.FirstName.Trim();
            sheet.Cells[row, 4].Value = person.SurName.Trim();
            sheet.Cells[row, 5].Value = person.LastName.Trim();
            sheet.Cells[row, 6].Value = person.City.Trim();
            sheet.Cells[row, 7].Value = person.Country.Trim();
        }

        public void SetSavePath(string newFilePath)
        {
            FilePath = newFilePath;
        }
    }
}