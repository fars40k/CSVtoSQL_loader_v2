using WpfStarter.Data.Export;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Prism.Ioc;

namespace WpfStarter.Data
{
    public class EntityWorker
    {
        public bool DoesDatabaseConnectionInitialized { get; private set; } = false;
        private IContainerExtension _container;

        private ICSVtoDatabase fileCSVtoSQL;
        private IEntityToExcel entityToEXL;
        private IEntityToXML entityToXML;

        public EntityWorker(IContainerExtension container)
        {
            _container = container;

            SetDefaultDataProcessingMethods();
            VerifyConnection("");
        }

        private void SetDefaultDataProcessingMethods()
        {
          //  entityToEXL = new EPPLusSaver(FileToWritePath);
          //  entityToXML = new XMLSaver(FileToWritePath, new Person());
        }

        public void VerifyConnection(string newConnStr)
        {
            try
            {
                using (PersonsContext pC = new PersonsContext())
                {
                    pC.Database.Connection.Open();
                    pC.Database.Connection.Close();
                    this.DoesDatabaseConnectionInitialized = true;
                }
            }
            catch (Exception ex)
            {
                this.DoesDatabaseConnectionInitialized = false;
            }
        }

        public void SetSourceFilePath(string newPath)
        {
          //  SourceFilePath = newPath;
        }

        public void SetFileToWritePath(string newPath)
        {
          //  FileToWritePath = newPath;
        }

        public string ReadCSVToDb(int maxRecords)
        {
            /* fileCSVtoSQL = new CSVReader(SourceFilePath);


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
            */
            return "";
        }

    }
}
