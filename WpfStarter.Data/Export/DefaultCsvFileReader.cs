using Prism.Ioc;
using System.Resources;

namespace WpfStarter.Data.Export
{
    public class DefaultCsvFileReader : Operation, IRequiringSourceFileSelection, IParametrisedAction<Inference>
    {
        public int BatchLimit { get; set; }
        public string SourceFilePath { get; set; }
        public Inference Settings { get; set; }

        private int _totalRecords;
        private int _failedRecords;
        private string _lineFromFile;

        public DefaultCsvFileReader(IContainerExtension container)
        {
            var ResourceManager = container.Resolve<ResourceManager>();
            _description = ResourceManager.GetString("OpCSVtoSQLExist") ?? "missing";
            BatchLimit = 10000;
        }

        public override string Run()
        {
            int beforeBatchID = 0;
            int beforeOperationID = -1;
            int recordsReadInThisBatch = 0;
            string errorsFilePath = "";

            _lineFromFile = string.Empty;
            _failedRecords = 0;
            _totalRecords = 0;

            // Reading records from file in batches
            using (StreamReader rStream = new StreamReader(SourceFilePath))
            {
                while (true) 
                {                    
                    // If end-of-file breaks iteration
                    if (rStream.Peek() == -1) break;

                    recordsReadInThisBatch = 0;
                    beforeBatchID = 0;

                    PersonsContext pC = new PersonsContext();                    
                    pC.Configuration.AutoDetectChangesEnabled = false;

                    // Getting maximal ID value
                    try
                    {
                        beforeBatchID = pC.Persons.Max(e => e.ID);
                    }
                    catch (Exception ex)
                    {
                    }

                    // Setting initial IDs before starting operation and batch 
                    if (beforeOperationID == -1) beforeOperationID = beforeBatchID;
                    int IncrimentalID = beforeBatchID;

                    // Loop for parsing and adding records from a file in batches
                    while (recordsReadInThisBatch < BatchLimit)
                    {
                        _lineFromFile = rStream.ReadLine();

                        // If end-of-file breaks cycle
                        if (_lineFromFile == null) break;

                        recordsReadInThisBatch++;

                        try
                        {
                            IncrimentalID++;
                            pC.Persons.Add(ParseLineToPerson(_lineFromFile, IncrimentalID));
                            if (IncrimentalID % 100 == 0) _progress.Report(_totalRecords + " / ???");
                            if (_cancelToken.IsCancellationRequested) throw new OperationCanceledException();
                        }
                        catch (OperationCanceledException)
                        {
                            RevertChanges(pC, beforeBatchID);
                            throw new OperationCanceledException();
                        }
                        catch (FormatException ex)
                        {
                            IncrimentalID--;

                            // Saving line with errors to a file

                            if (errorsFilePath == "")
                            {
                                errorsFilePath = CreateErrorFile(SourceFilePath);
                            }
                            AddRecordToErrorFile(errorsFilePath, _lineFromFile);
                            _failedRecords++;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    _totalRecords += recordsReadInThisBatch;

                    pC.Configuration.AutoDetectChangesEnabled = true;
                    pC.ChangeTracker.DetectChanges();
                    pC.SaveChanges();
                    pC.Dispose();

                }
            }
            if (Settings != null)
            {
                Settings.TotalProcessed = _totalRecords;
                Settings.TotalFailed = _failedRecords;
            }            
            return "";
        }

        /// <summary>
        ///  Blanks a file or creates a new one in the path based on the argument string
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
            string[] splitBuffer;
            Person person = new Person() 
            { 
                FirstName = "", 
                SurName = "", 
                LastName = "", 
                City = "", 
                Country = "" 
            };

            splitBuffer = this._lineFromFile.Replace(" ", "")
                              .Split(';');

            // Fills properties of "Person" or writes that line to an error file
            if ((ParseDateToSqlType(splitBuffer[0]) != DateTime.MinValue))
            {
                person.Date = ParseDateToSqlType(splitBuffer[0]);
            }
            else
            {
                throw new FormatException();
            }

            for (int i = 1; i < splitBuffer.Length; i++)
            {
                if ((splitBuffer[i].Length > 50) || (splitBuffer[i].Length <= 1)
                   || (splitBuffer[i] == null))
                {
                    throw new FormatException();
                }
            }

            person.FirstName = splitBuffer[1].Trim();
            person.SurName = splitBuffer[2].Trim();
            person.LastName = splitBuffer[3].Trim();
            person.City = splitBuffer[4].Trim();
            person.Country = splitBuffer[5].TrimEnd();
            person.ID = newPersonID;

            return person;
        }

        /// <summary>
        /// Adds an unrecognized entry to a file with err entries
        /// </summary>
        private void AddRecordToErrorFile(string targetPath,string line)
        {
            using (StreamWriter sw = new StreamWriter(targetPath, true, System.Text.Encoding.Default))
            {               
                sw.WriteLine(line);
            }            
        }

        /// <summary>
        /// Converts a string to a DateTime type suituable for adding to a SQL DateTime type,
        /// or returns the minimum value
        /// </summary>
        private DateTime ParseDateToSqlType(string inputString)
        {
            try
            {
                if (Int32.Parse(inputString.Substring(inputString.LastIndexOf("/") + 1, 4)) < 1753)
                {
                    return DateTime.MinValue;
                }

                return DateTime.Parse(inputString);
            } 
            catch (Exception e)
            {
                return DateTime.MinValue;
            }     
        }
        
        private void RevertChanges(PersonsContext context, int deleteFrom)
        {
            var list = context.Persons
                              .Where(p => p.ID > deleteFrom);

            context.Configuration.AutoDetectChangesEnabled = false;
            context.Persons.RemoveRange(list);
            context.Configuration.AutoDetectChangesEnabled = true;
            context.SaveChanges();
        }
    }
}
