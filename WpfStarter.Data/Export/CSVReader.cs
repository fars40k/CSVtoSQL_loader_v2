using Prism.Ioc;
using System.Resources;

namespace WpfStarter.Data.Export
{
    internal class CSVReader : Operation, ISourceFileSelectionRequired
    {      

        private int RecordsRead;
        private int FailedRecords;
        string Line;

        public CSVReader(IContainerExtension container)
        {
            var ResourceManager = container.Resolve<ResourceManager>();
            Description = ResourceManager.GetString("OpCSVtoSQLExist") ?? "missing";
            BatchLimit = 10000;
        }

        public int BatchLimit { get; private set; }
        public string SourceFilePath { get; set; }

        public override string Run()
        {
            RecordsRead = 0;
            FailedRecords = 0;

            // Reading records from file in batches
            using (StreamReader sr = new StreamReader(SourceFilePath))
            {
                while (true) 
                {                    
                    string errorsFilePath = "";
                    // If end-of-file leaves iteration
                    if (sr.Peek() == -1) break;
                    RecordsRead = 0;

                    PersonsContext pC = new PersonsContext();                    
                    pC.Configuration.AutoDetectChangesEnabled = false;

                    // Getting maximal ID value
                    int OldID = 1;
                    try
                    {
                        OldID = pC.Persons.Max(e => e.ID);
                    }
                    catch (Exception ex)
                    {
                    }
                    int IncrID = OldID;

                    // Loop for parsing and adding records from a file in batches
                    while (RecordsRead < BatchLimit)
                    {
                        Line = sr.ReadLine();

                        // If end-of-file breaks cycle
                        if (Line == null) break;

                        RecordsRead++;

                        try
                        {
                            pC.Persons.Add(ParseLineToPerson(Line, IncrID));
                            IncrID++;
                        }
                        catch (FormatException ex)
                        {
                            // Saving Line with errors to file
                            if (errorsFilePath == "")
                            {
                                errorsFilePath = CreateErrorFile(SourceFilePath);
                            }
                            Add1RecordToErrorFile(errorsFilePath, Line);
                            FailedRecords++;
                        }
                        catch (Exception ex)
                        {
                            return "false";
                        }
                    }

                    pC.Configuration.AutoDetectChangesEnabled = true;
                    pC.ChangeTracker.DetectChanges();
                    pC.SaveChanges();
                    pC.Dispose();

                }
            }
            return FailedToAllStringFraction();
        }

        /// <summary>
        ///  Writies to a file or creates a new one in the path based on the argument string
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
        /// Converts an input string with an ID to a Person instance or throw a FormatExeption
        /// </summary>
        private Person ParseLineToPerson(string line, int newPersonID)
        {
            string[] SplitBuffer;
            Person person = new Person() { FirstName = "", SurName = "", LastName = "", City = "", Country = "" };

            SplitBuffer = Line.Replace(" ", "")
                              .Split(';');

            // Fills properties of "Person" or writes that line to an error file
            if ((ParseDateToSqlType(SplitBuffer[0]) != DateTime.MinValue))
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
            person.ID = newPersonID;

            return person;
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
        /// Converts a string to a DateTime type suituable for adding to a SQL DateTime type, or returns the minimum value
        /// </summary>
        private DateTime ParseDateToSqlType(string inputString)
        {
            try
            {
                if (Int32.Parse(inputString.Substring(0, 4)) < 1753) return DateTime.MinValue;

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
        public string FailedToAllStringFraction()
        {
            return FailedRecords.ToString() + @" / " + RecordsRead.ToString();
        }

        
    }
}
