using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntityAssembly.Classes
{
    internal class LumenWorksReader : FileCSVtoSQL
    {
        public string filePath { get; private set; }

        public LumenWorksReader(string newfilePath)
        {
            this.filePath = newfilePath;
        }

        public override bool Run(int maxRecords)
        {
            using (var csv = new CachedCsvReader(new StreamReader("data.csv"), false, ';'))
            {
                // Настройка шаблона обработки CSV

                int RecordsRead = 0;
                Person person;
                List<string> personFields = new List<string>()
                {
                        "FirstName",
                        "SurName",
                        "LastName",
                        "City",
                        "Country"
                };
                csv.Columns.Add(new Column { Name = "Date", Type = typeof(DateTime) });
                csv.Columns.Add(new Column { Name = "FirstName", Type = typeof(string) });
                csv.Columns.Add(new Column { Name = "LastName", Type = typeof(string) });
                csv.Columns.Add(new Column { Name = "SurName", Type = typeof(string) });
                csv.Columns.Add(new Column { Name = "City", Type = typeof(string) });
                csv.Columns.Add(new Column { Name = "Country", Type = typeof(string) });

                //Чтение из файла записей порциями ограниченными maxRecords и добавление в БД
                while (csv.ReadNextRecord())
                {                    
                    using (PersonsContext pC = new PersonsContext())
                    {
                        while ((csv.ReadNextRecord()) && (RecordsRead < maxRecords))
                        {

                            person = new Person();
                            RecordsRead++;
                            try
                            {
                                if (ParseDateToSqlType(csv[0]) != DateTime.MinValue)
                                {
                                    person.Date = ParseDateToSqlType(csv[0]);
                                }
                                else
                                {
                                    throw new Exception();
                                }
                                for (int i = 1; i < csv.FieldCount; i++)
                                {
                                    if ((csv[i].Length <= 50) && (csv[1].Length > 1))
                                    {
                                        FieldInfo fi = typeof(Person).GetField(personFields[i - 1]);
                                        fi.SetValue(person, csv[i]);
                                    }
                                    else
                                    {
                                        throw new Exception();
                                    }
                                }
                            pC.Persons.Add(person);
                            }
                            catch (Exception ex)
                            {
                                Add1RecordToErrorFile(csv);
                            }
                        }
                        pC.SaveChanges();
                    }

                }
            }
        return false;
        }

        /// <summary>
        /// Добавляет не распознанную запись в файл с ошибочными записями
        /// </summary>
        private void Add1RecordToErrorFile(CachedCsvReader csv)
        {
            // Если файла нет то создать
            //if (filePath.LastIndexOf(".")-6)
            string AssembledString = "";
            for(int i = 0; i < csv.FieldCount; i++)
            {
                AssembledString += AssembledString + ';';
            }
            AssembledString += @"\r\n";
        }
        

        /// <summary>
        /// Конвертирует строку в DateTime тип или возвращает минимальное значение
        /// </summary>
        private DateTime ParseDateToSqlType(string inputString)
        {
            try
            {
                return DateTime.Parse(inputString);
            } 
            catch (Exception e)
            {
                return DateTime.MinValue;
            }     
        }
    }
}
