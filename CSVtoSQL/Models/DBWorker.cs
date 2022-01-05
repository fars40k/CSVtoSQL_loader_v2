using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CSVtoSQL.Models.Operations;
using static CSVtoSQL.Models.MainModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;

namespace CSVtoSQL.Models
{
    public class DBWorker
    {
        private EntityAssembly.EntityWorker eWorker;
        private MainModel model;
        private string connString;
        private bool isDBOperationRunned;

        public bool IsDBOperationRunned
        {
            get { return isDBOperationRunned; }
            private set
            {
                isDBOperationRunned = value;
                if (isDBOperationRunned)
                {
                    model.SetAppGlobalState(EnumGlobalState.Disabled);
                } else
                {
                    model.SetAppGlobalState(EnumGlobalState.DbConnected);
                }
            }
        }

        public DBWorker(MainModel newModel, string newConnString)
        {
            model = newModel;
            connString = newConnString;
            eWorker = new EntityAssembly.EntityWorker();
            if (eWorker.VerifyConnString(connString))
            {
                IsDBOperationRunned = false;
                model.WriteDBStringToFile(connString);
            }
            else
            {
                ErrorNotify.NewError(new AppError(Localisation.Strings.DBError, ""));
                model.SetAppGlobalState(EnumGlobalState.FileDecided);
            }
        }

        public void SetFileSourcePath(string path)
        {
            eWorker.SetSourceFilePath(path);
        }

        public void SetWriteToFilePath(string path)
        {
            eWorker.SetFileToWritePath(path);
        }

        /// <summary>
        /// Читает лимит загрузок для одного сеаса из App.config
        /// </summary>
        public void BeginLoadToDatabase()
        {
            try
            {
                string? DbLoadLimit = ConfigurationManager.AppSettings["DbLoadLimit"];
                bool result = eWorker.ReadCSVToDb(Int32.Parse(DbLoadLimit));
                if (result == false) throw new Exception();
            }
            catch (Exception ex)
            {
                if ((ex is FormatException) || (ex is ArgumentNullException))
                {
                    ErrorNotify.NewError(new AppError(Localisation.Strings.OpWrongSettings, ""));
                } 
                else
                {
                    ErrorNotify.NewError(new AppError(Localisation.Strings.OpLoadFromFileError, ""));
                }
            }

        }
    }
}
