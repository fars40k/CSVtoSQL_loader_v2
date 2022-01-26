using Prism.Ioc;
using System;
using System.IO;
using WpfStarter.Data;

namespace WpfStarter.UI.Models
{
    public partial class Model
    {
        public Model(IContainerProvider container)
        {
            _containerProvider = container;
            var dataPool = container.Resolve<DataObjectPool>();

            ApplyDefaultEventRouting();

            var dbWrk = container.Resolve<EntityWorker>();
            DatabaseInitialized(dbWrk.DoesDatabaseConnectionInitialized);

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
                    dbWrk.SourceFileSelected(value);
                }
            };

            dbWrk.NotifyDataAccess += ErrorNotify.NewError;
            BeginOperation += dbWrk.BeginOperation;

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
