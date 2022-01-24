using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.Data
{
    /// <summary>
    /// Class designed to create instances of essential DataAccess classes in the WpfStarter.Data namespace 
    /// </summary>
    public class DataObjectPool
    {
        public EntityWorker DatabaseWorker;
        public ResourceManager ResourceManager;

        public DataObjectPool(IContainerExtension container)
        {
            ResourceManager = new ResourceManager("WpfStarter.UI.Localisation.Strings", Assembly.GetEntryAssembly());
            container.RegisterInstance<ResourceManager>(ResourceManager);

            DatabaseWorker = container.Resolve<EntityWorker>();

        }
    }
}
