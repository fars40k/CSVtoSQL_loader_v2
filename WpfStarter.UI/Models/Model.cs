using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfStarter.Data;

namespace WpfStarter.UI.Models
{
    public partial class Model
    {
        public EnumGlobalState _appGlobalState { get; private set; }

        private IContainerExtension _container;
        private IRegionManager _regionManager;
        private EntityWorker _databaseWorker;

        private string _sourceFile;
        public string SourceFile
        {
            get { return _sourceFile; }
            set { _sourceFile = value; }
        }

        public Action<string> AppStateChanged;

        public Model(IContainerExtension container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
            _databaseWorker = container.Resolve<EntityWorker>();
            DatabaseInitialized(_databaseWorker.DoesDatabaseConnectionInitialized);
        }

        private void DatabaseInitialized(bool IsInitialized)
        {
            if (IsInitialized)
            {
                SetAppGlobalState(EnumGlobalState.DbConnected);
                _regionManager.RequestNavigate("OperationsRegion","Operations");
            } else
            {
                SetAppGlobalState(EnumGlobalState.DbConnectionFailed);                
            }
        }

        private string GetAppGlobalState()
        {
            return _appGlobalState.ToString();
        }

        public void NotifyAppGlobalState()
        {
            if (AppStateChanged != null)
            {
                AppStateChanged.Invoke(_appGlobalState.ToString());
            }
        }

        public void SetAppGlobalState(EnumGlobalState newState)
        {
            _appGlobalState = newState;
            this.NotifyAppGlobalState();
        }
    }
}
