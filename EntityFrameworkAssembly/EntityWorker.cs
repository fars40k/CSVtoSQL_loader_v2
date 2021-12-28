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
        public string filePath;


        public EntityWorker()
        {

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

        public void SetDataBaseOperationSettings()
        {

        }

    }
}
