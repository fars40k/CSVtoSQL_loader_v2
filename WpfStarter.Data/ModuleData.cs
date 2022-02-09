using Prism.Ioc;
using Prism.Modularity;
using WpfStarter.Data.Export;

namespace WpfStarter.Data
{
    /// <summary>
    /// Prism infrastructure class
    /// </summary>
    public class ModuleData : IModule

    { 
        void IModule.OnInitialized(IContainerProvider containerProvider)
        {
            // Adding views at the End of Module classes initialisation

            DataAccessModel eW = containerProvider.Resolve<DataAccessModel>();
            eW.UpdateDataViews.Invoke();
        }

        void IModule.RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDatabaseAction, DefaultCsvFileReader>();
            containerRegistry.Register<IDatabaseAction, DefaultExcelExporter>();
            containerRegistry.Register<IDatabaseAction, DefaultXmlExporter>();
        }

    }
}
