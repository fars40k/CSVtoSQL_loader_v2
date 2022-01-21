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
        private IContainerProvider _container;

        private Operation _selectedOperation;
        public List<IDatabaseAction> DatabaseOperationsServices { get; private set; }

        private DataFilters _filters;

        public bool DoesDatabaseConnectionInitialized { get; private set; } = false;

        public EntityWorker(IContainerProvider containerProvider)
        {
            _container = containerProvider;
            VerifyConnection();
        }

        private void FillOperationsList()
        {
            DatabaseOperationsServices = new List<IDatabaseAction>();
            if (DatabaseOperationsServices.Count == 0)
            {
                DatabaseOperationsServices.Add(_container.Resolve<CSVReader>());
                DatabaseOperationsServices.Add(_container.Resolve<EPPLusSaver>());
                DatabaseOperationsServices.Add(_container.Resolve<XMLSaver>());           
            }    
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
            FillOperationsList();

            IRegionManager regionManager = _container.Resolve<IRegionManager>();

            Operations view = _container.Resolve<Operations>();            
            IRegion region = regionManager.Regions["OperationsRegion"];
            region.Add(view);

            _filters = _container.Resolve<DataFilters>();
            region = regionManager.Regions["FiltersRegion"];
            region.Add(_filters);

            //OperationsListFilled.Invoke(DatabaseOperationsServices);
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
