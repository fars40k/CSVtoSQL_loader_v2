using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WpfStarter.Data.Export
{
    internal class CSVReader : Operation
    {
        // sql bulk copy и создание новой таблицы
        public string filePath { get; private set; }

        private int RecordsRead = 0;
        private int FailedRecords = 0;

        public CSVReader(IContainerExtension container)
        {
            DataViewsLocalisation dwl = container.Resolve<DataViewsLocalisation>();
            Description = dwl._dataViewsStrings["Operation 1"];
        }

        public bool Run()
        {
            int maxRecords = 1;
            string Line;
            string[] SplitBuffer;

            // Reading records from file in batches
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (true) 
                {                    
                    string errorsFilePath = "";
                    // Если конец файла выйти из цикла
                    if (sr.Peek() == -1) break;
                    RecordsRead = 0;

                    using (PersonsContext pC = new PersonsContext())
                    {
                        int OldmaxID = pC.Database.ExecuteSqlCommand("SELECT MAX(ID) FROM dbo.Persons;");
                        int IncrID = OldmaxID++;

                        // Loop for parsing and adding records from a file in batches
                        while (RecordsRead < maxRecords)
                        {
                            Person person = new Person() { FirstName = "", SurName = "", LastName = "", City = "", Country = "" };
                            Line = sr.ReadLine();

                            // If end-of-file breaks cycle
                            if (Line == null) break;

                            RecordsRead++;

                            pC.Configuration.AutoDetectChangesEnabled = false;

                            try
                            {
                                SplitBuffer = Line.Replace(" ", "").Split(';');

                                // Fills properties of "Person" or writes that line to an error file
                                if (ParseDateToSqlType(SplitBuffer[0]) != DateTime.MinValue)
                                {
                                    person.Date = ParseDateToSqlType(SplitBuffer[0]);
                                }
                                else
                                {
                                    throw new FormatException();
                                }

                                for (int i = 1; i < SplitBuffer.Length; i++)
                                {
                                    if ((SplitBuffer[i].Length > 50) || (SplitBuffer[i].Length <= 1)
                                       || (SplitBuffer[i] == null))
                                    {
                                        throw new FormatException();
                                    }
                                }

                                person.FirstName = SplitBuffer[1];
                                person.SurName = SplitBuffer[2];
                                person.LastName = SplitBuffer[3];
                                person.City = SplitBuffer[4];
                                person.Country = SplitBuffer[5];
                                person.ID = IncrID++;

                                pC.Persons.Add(person);
                            }
                            catch (FormatException ex)
                            {
                                if (errorsFilePath == "")
                                {
                                    errorsFilePath = CreateErrorFile(filePath);
                                }
                                Add1RecordToErrorFile(errorsFilePath, Line);
                                FailedRecords++;
                            }
                            catch (Exception ex)
                            {
                                return false;
                            }
                        }                     

                        pC.Configuration.AutoDetectChangesEnabled = true;
                        pC.SaveChanges();
                    }

                }
            }
            return true;
        }



        /// <summary>
        ///  Writies to file or creates a new one to write line with error 
        /// </summary>
        private string CreateErrorFile(string targetPath)
        {
            string path = targetPath.Substring(0, targetPath.LastIndexOf(".")) + "_Errors.csv";
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
        /// Adds an unrecognized entry to a file with err entries
        /// </summary>
        private void Add1RecordToErrorFile(string targetPath,string line)
        {
            using (StreamWriter sw = new StreamWriter(targetPath, true, System.Text.Encoding.Default))
            {               
                sw.WriteLine(line);
            }            
        }


        /// <summary>
        /// Converts a string to DateTime type or returns the minimum value
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

        /// <summary>
        /// Returns a fraction representing the ratio of read errors to all rows
        /// </summary>
        public string FailedToAllString()
        {
            return FailedRecords.ToString() + @" / " + RecordsRead.ToString();
        }
    }
}
