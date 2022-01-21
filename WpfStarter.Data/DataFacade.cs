using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.Data
{
    /// <summary>
    /// Class designed to create instances of essential DataAccess classes in WpfStarter.Data namespace 
    /// </summary>
    public class DataFacade
    {
        public EntityWorker DatabaseWorker;
        public DataViewsLocalisation DataLocalisation;

        public DataFacade(IContainerProvider container)
        {
            DatabaseWorker = container.Resolve<EntityWorker>();
            DataLocalisation = container.Resolve<DataViewsLocalisation>();
        }
    }
}
