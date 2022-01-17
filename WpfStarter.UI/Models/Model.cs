using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.UI.Models
{
    public class Model
    {
        IContainerExtension _container;
        IRegionManager _regionManager;

        private string _sourceFile;

        public Model(IContainerExtension container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public string SourceFile
        {
            get { return _sourceFile; }
            set { _sourceFile = value; }
        }





    }
}
