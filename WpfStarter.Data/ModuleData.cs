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
        }

        void IModule.RegisterTypes(IContainerRegistry containerRegistry)
        {
           _containerRegistry = containerRegistry;
           _containerRegistry.RegisterForNavigation<DataFilters>();
           _containerRegistry.RegisterForNavigation<Operations>();
            containerRegistry.RegisterSingleton<EntityWorker>();
        }

        public void RegisterRelatedRegions()
        {

        }
    }
}
