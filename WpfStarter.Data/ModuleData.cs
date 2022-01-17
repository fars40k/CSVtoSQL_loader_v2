using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using WpfStarter.Data.Views;

namespace WpfStarter.Data
{
    public class ModuleData : IModule
    {
        IContainerRegistry _containerRegistry;

        void IModule.OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion("FiltersRegion", typeof(DataFilters));
            regionManager.RegisterViewWithRegion("OperationsRegion", typeof(Operations));
        }

        void IModule.RegisterTypes(IContainerRegistry containerRegistry)
        {
           _containerRegistry = containerRegistry;

            containerRegistry.RegisterSingleton<EntityWorker>();


        }
    }
}
