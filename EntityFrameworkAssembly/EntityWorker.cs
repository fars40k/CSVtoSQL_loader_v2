using EntityAssembly.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntityAssembly
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
        }

        private void SetDefaultDataProcessingMethods()
        {
            fileCSVtoSQL = new CSVHelperLoader(SourceFilePath);
            entityToEXL = new EPPLusSaver(FileToWritePath);
            entityToXML = new XMLSerializeSaver(FileToWritePath, new Person());
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

        public bool ReadCSVToDb(int maxRecords)
        {
            try
            {
                fileCSVtoSQL.Run(maxRecords);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
