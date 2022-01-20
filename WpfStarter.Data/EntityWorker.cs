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
using Prism.Regions;
using WpfStarter.Data.Views;

namespace WpfStarter.Data
{
    public class EntityWorker
    {
        public bool DoesDatabaseConnectionInitialized { get; private set; } = false;

        private IContainerExtension _container;

        private Operation _selectedOperation;
        public List<IDatabaseAction> DatabaseOperationsServices = new List<IDatabaseAction>();

        private DataFilters _filters;

        public Action<List<IDatabaseAction>> OperationsListFilled;

        public EntityWorker(IContainerExtension container)
        {
            _container = container;        
            VerifyConnection();
            FillOperationsList(container);
        }

        private void FillOperationsList(IContainerExtension container)
        {
            if (DatabaseOperationsServices.Count == 0)
            {
                DatabaseOperationsServices.Add(container.Resolve<CSVReader>());
                DatabaseOperationsServices.Add(container.Resolve<EPPLusSaver>());
                DatabaseOperationsServices.Add(container.Resolve<XMLSaver>());           
            }
            if (OperationsListFilled != null)  OperationsListFilled.Invoke(DatabaseOperationsServices);
    
        }

        public void BeginOperation()
        {
            if (_selectedOperation != null)
            {
                if (_selectedOperation is ILinqBuildRequired)
                {

                }
            }

        }

        public void AddDataViewsToRegions()
        {
            IRegionManager regionManager = _container.Resolve<IRegionManager>();

            Operations view = _container.Resolve<Operations>();
            IRegion region = regionManager.Regions["OperationsRegion"];
            region.Add(view);

            _filters = _container.Resolve<DataFilters>();
            region = regionManager.Regions["FiltersRegion"];
            region.Add(_filters);

            FillOperationsList(_container);

        }

        public void VerifyConnection()
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
