using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WpfStarter.UI.Models.Operations;
using static WpfStarter.UI.Models.MainModel;
using System.Configuration;
using System.Threading;
using WpfStarter.Data;

namespace WpfStarter.UI.Models
{
    public class DBWorker
    {
        private EntityWorker eWorker;
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
                    model.SetAppGlobalState(GlobalState.Disabled);
                } else
                {
                    model.SetAppGlobalState(GlobalState.DbConnected);
                }
            }
        }

        public DBWorker(MainModel newModel, string newConnString)
        {
            model = newModel;
            connString = newConnString;
            eWorker = new WpfStarter.Data.EntityWorker();
            if (eWorker.VerifyConnString(connString))
            {
                IsDBOperationRunned = false;
                model.WriteDBStringToFile(connString);
            }
            else
            {
                ErrorNotify.NewError(new AppError(Localisation.Strings.DBError, ""));
                model.SetAppGlobalState(GlobalState.FileDecided);
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


        public async void BeginLoadToDatabase(string loadParam)
        {
            try
            {
                string DbLoadLimit = ConfigurationManager.AppSettings["BatchLimit"];
                string result = await Task.Run(() => eWorker.ReadCSVToDb(Int32.Parse(DbLoadLimit),model.cancellationToken));
                switch (result)
                {
                    case "False":
                        {
                            throw new Exception();
                        }
                    case "True":
                        {
                            ErrorNotify.NewError(new AppError(Localisation.Strings.OpRecordsAdded, ""));
                            break;
                        }
                    default:
                        {
                            ErrorNotify.NewError(new AppError(Localisation.Strings.OpErrorAddRecords, result));
                            break;
                        }
                }

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
