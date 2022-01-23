using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfStarter.Data;

namespace WpfStarter.UI.Models
{
    public partial class Model
    {
        private DataObjectPool _dataObjectPool;

        public Model(IContainerProvider container)
        {
            _containerProvider = container;

            _dataObjectPool = container.Resolve<DataObjectPool>();

            ApplyDefaultEventRouting();

            var dbWrk = container.Resolve<EntityWorker>();
            DatabaseInitialized(dbWrk.DoesDatabaseConnectionInitialized);
            dbWrk.NotifyDataAccessError += ErrorNotify.NewError;
            BeginOperation += dbWrk.BeginOperation;
        }

        public Action BeginOperation;
        public Action<string> AppStateChanged;
        public Action<string> FileSelected;

        public EnumGlobalState _appGlobalState { get; private set; }

        private IContainerProvider _containerProvider;

        private void DatabaseInitialized(bool IsInitialized)
        {
            if (IsInitialized)
            {
                SetAppGlobalState(EnumGlobalState.DbConnected);
            } else
            {
                SetAppGlobalState(EnumGlobalState.DbConnectionFailed);                
            }
        }

        public void ApplyDefaultEventRouting()
        {
            var dbWrk = _containerProvider.Resolve<EntityWorker>();
            FileSelected += (value) =>
            {
                if (new FileInfo(value).Length <= 64)
                {
                    ErrorNotify.NewError(Localisation.Strings.ErrorFileEmpty);
                }
                else
                {
                    SetAppGlobalState(EnumGlobalState.FileSelected);
                    dbWrk.AddDataViewsToRegions();
                    dbWrk.SourceFileSelected(value);
                }
            };

            BeginOperation += () =>
            {
                dbWrk.BeginOperation();
            };

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
