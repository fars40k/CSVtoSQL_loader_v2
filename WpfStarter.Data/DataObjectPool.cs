using System.Reflection;
using System.Resources;
using Prism.Ioc;
using Prism.Regions;
using WpfStarter.Data.Views;

namespace WpfStarter.Data
{
    /// <summary>
    /// Class designed to create instances of essential DataAccess classes in the WpfStarter.Data namespace 
    /// </summary>
    public class DataObjectPool
    {
        private DataAccessModel _databaseWorker;
        private ResourceManager _resourceManager;
        private Progress<string> _progress;

        public DataObjectPool(IContainerExtension container)
        {
            _resourceManager = new ResourceManager("WpfStarter.UI.Localisation.Strings", Assembly.GetEntryAssembly());
            container.RegisterInstance<ResourceManager>(_resourceManager);

            _databaseWorker = container.Resolve<DataAccessModel>();

            _progress = container.Resolve<Progress<string>>();
            container.RegisterInstance<Progress<string>>(_progress, "DataProgress");

            var regionManager = container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("OperationsRegion", typeof(Operations));
            regionManager.RegisterViewWithRegion("FiltersRegion", typeof(DataFilters));
        }
    }
}
