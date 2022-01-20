using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using WpfStarter.Data.Export;
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

            containerRegistry.RegisterSingleton<EntityWorker>();

            containerRegistry.Register<IDatabaseAction, CSVReader>();
            containerRegistry.Register<IDatabaseAction, EPPLusSaver>();
            containerRegistry.Register<IDatabaseAction, XMLSaver>();

        }

    }
}
