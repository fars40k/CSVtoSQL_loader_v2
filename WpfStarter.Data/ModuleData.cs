using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using WpfStarter.Data.Views;

namespace WpfStarter.Data
{
    internal class ModuleData : IModule
    {
        void IModule.OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("FiltersRegion", typeof(DataFilters));
            regionManager.RegisterViewWithRegion("Operations", typeof(Operations));
        }

        void IModule.RegisterTypes(IContainerRegistry containerRegistry)
        {
           
        }
    }
}
