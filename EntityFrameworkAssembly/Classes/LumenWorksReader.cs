using LumenWorks.Framework.IO;
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
        // sql bulk copy и создание новой таблицы
        public string filePath { get; private set; }

        public LumenWorksReader(string newfilePath)
        {
            string newPath = newfilePath.Replace(@"\\", @"\");
            filePath = newPath;
        }

        public override bool Run(int maxRecords)
        {
            using (var csv = new CachedCsvReader(new StreamReader(@"C:\Users\ArtyShock\Documents\TestCSV.csv"), false, ';'))
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
                while (RecordsRead < maxRecords) // fix
                {
                    string errorsFilePath = "";

                    using (PersonsContext pC = new PersonsContext())
                    {
                        pC.Configuration.AutoDetectChangesEnabled = false;
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
                                if (errorsFilePath == "") 
                                { 
                                    errorsFilePath = CreateErrorFile(filePath); 
                                }
                                Add1RecordToErrorFile(errorsFilePath, csv);
                            }
                        }
                        pC.Configuration.AutoDetectChangesEnabled = true;
                        pC.SaveChanges();
                    }

                }
            }
        return false;
        }

        /// <summary>
        /// Создать файл для ошибочных записей
        /// </summary>
        private string CreateErrorFile(string targetPath)
        {
            string path = targetPath.Substring(0, targetPath.LastIndexOf(".")-1) + "_Errors.csv";
            if (File.Exists(path))
            {
                File.WriteAllText(path, String.Empty);
            }
            else
            {
                using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
                {
                }
            }
            return path;
        }


        /// <summary>
        /// Добавляет не распознанную запись в файл с ошибочными записями
        /// </summary>
        private void Add1RecordToErrorFile(string targetPath, CachedCsvReader csv)
        {
            string AssembledString = "";
            for (int i = 0; i < csv.FieldCount; i++)
            {
                AssembledString += AssembledString + ';';
            }
            AssembledString += @"\r\n";
            using (StreamWriter sw = new StreamWriter(targetPath, true, System.Text.Encoding.Default))
            {               
                sw.Write(AssembledString);
            }            
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
