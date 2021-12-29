using EntityAssembly.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAssembly
{ 
    public class EntityWorker
    {
        private FileCSVtoSQL fileCSVtoSQL;
        private EntityToExcel entityToEXL;
        private EntityToXML entityToXML;

        public string filePath;

        public EntityWorker()
        {
            SetDefaultDataProcessingMethods();
        }

        private void SetDefaultDataProcessingMethods()
        {
            fileCSVtoSQL = new CSVHelperLoader(filePath);
            entityToEXL = new EPPLusSaver(filePath);
            entityToXML = new XMLSerializeSaver(filePath, new Person());
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
                    pC.Database.Connection.Open();
                    if (pC.Database.Exists()) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }
}
