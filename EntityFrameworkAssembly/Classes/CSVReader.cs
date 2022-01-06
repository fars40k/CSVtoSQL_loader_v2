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
    internal class CSVReader : FileCSVtoSQL
    {
        // sql bulk copy и создание новой таблицы
        public string filePath { get; private set; }

        public CSVReader(string newfilePath)
        {
            filePath = newfilePath;
        }

        public override bool Run(int maxRecords)
        {
            int IdIncrement = 1;
            int RecordsRead = 0;
            string Line;
            string[] SplitBuffer;

            //Чтение из файла записей порциями ограниченными maxRecords и добавление в БД
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
                        // Цикл для парсинга и добавления файла порциями
                        while (RecordsRead < maxRecords)
                        {
                            Person person = new Person() { FirstName = "", SurName = "", LastName = "", City = "", Country = "" };
                            Line = sr.ReadLine();
                            // Если конец файла выйти из цикла
                            if (Line == null) break;                           
                            RecordsRead++;

                            pC.Configuration.AutoDetectChangesEnabled = false;

                            try
                            {
                                SplitBuffer = Line.Replace(" ", "").Split(';');                                
                                // заполняет поля Person или записывает строку в файл ошибок
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
                                    if ((SplitBuffer[i].Length > 50) && (SplitBuffer[i].Length <= 1))
                                    {
                                        throw new FormatException();
                                    }
                                }

                                person.FirstName = SplitBuffer[1];
                                person.SurName = SplitBuffer[2];
                                person.LastName = SplitBuffer[3];
                                person.City = SplitBuffer[4];
                                person.Country = SplitBuffer[5];
                                person.ID = IdIncrement++;

                                pC.Persons.Add(person);
                            }
                            catch (FormatException ex)
                            {
                                if (errorsFilePath == "")
                                {
                                    errorsFilePath = CreateErrorFile(filePath);
                                }
                                Add1RecordToErrorFile(errorsFilePath, Line);
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
        /// Создать файл для ошибочных записей
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
        /// Добавляет не распознанную запись в файл с ошибочными записями
        /// </summary>
        private void Add1RecordToErrorFile(string targetPath,string line)
        {
            using (StreamWriter sw = new StreamWriter(targetPath, true, System.Text.Encoding.Default))
            {               
                sw.WriteLine(line);
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
