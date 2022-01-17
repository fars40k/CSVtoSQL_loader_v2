using WpfStarter.Data.Export;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfStarter.Data
{ 
    public class EntityWorker
    {
        private FileCSVtoSQL fileCSVtoSQL;
        private EntityToExcel entityToEXL;
        private EntityToXML entityToXML;

        public string SourceFilePath;
        public string FileToWritePath;

        public EntityWorker()
        {
            SetDefaultDataProcessingMethods();
            bool IsDatabaseAvalable = VerifyConnString("");
            
        }

        private void SetDefaultDataProcessingMethods()
        {
            entityToEXL = new EPPLusSaver(FileToWritePath);
            entityToXML = new XMLSaver(FileToWritePath, new Person());
        }

        /// <summary>
        /// Открывает контекст к базе данных для проверки правильного ввода строки подключения
        /// </summary>
        public bool VerifyConnString(string newConnStr)
        {
            try
            {
                using (PersonsContext pC = new PersonsContext())
                {
                    if (pC.Database.Exists()) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public void SetSourceFilePath(string newPath)
        {
            SourceFilePath = newPath;
        }

        public void SetFileToWritePath(string newPath)
        {
            FileToWritePath = newPath;
        }

        public string ReadCSVToDb(int maxRecords, CancellationToken cancellationToken)
        {
            fileCSVtoSQL = new CSVReader(SourceFilePath);
            fileCSVtoSQL.cancellationToken = cancellationToken;

            bool result = fileCSVtoSQL.Run(maxRecords);
            if (result)
            {
                if (fileCSVtoSQL.FailedToAllString().StartsWith("0 "))
                {
                    return "True";

                } else
                {
                    return fileCSVtoSQL.FailedToAllString();

                }
            } else
            return "False";             
        }

    }
}
