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
        private EntityAssembly.EntityWorker Eworker;
        private MainModel model;
        private string connString;

        public DBWorker(MainModel newModel, string newConnString)
        {    
            model = newModel;
            connString = newConnString;
            Eworker = new EntityAssembly.EntityWorker();
            Eworker.sourceFilePath = model
            if (Eworker.VerifyConnString(connString))
            {
                model.SetAppGlobalState(EnumGlobalState.DbConnected);
                model.WriteDBStringToFile(connString);
            }
            else
            {
                ErrorNotify.NewError(new AppError(Localisation.Strings.DBError, ""));
                model.SetAppGlobalState(EnumGlobalState.FileDecided);
            }
        }

    }
        
}
