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

        private GlobalState _previousApplicationGlobalState;
        public GlobalState ApplicationGlobalState { get; private set; }

        private IContainerProvider _containerProvider;

        private void DatabaseInitialized(bool IsInitialized)
        {
            if (IsInitialized)
            {
                SetAppGlobalState(GlobalState.DbConnected);
            } else
            {
                SetAppGlobalState(GlobalState.DbConnectionFailed);                
            }
        }

        public void ApplyDefaultEventRouting()
        {
            var dataWorker = _containerProvider.Resolve<EntityWorker>();
            FileSelected += (value) =>
            {
                if (new FileInfo(value).Length <= 64)
                {
                    ErrorNotify.NewError(Localisation.Strings.ErrorFileEmpty);
                }
                else
                {
                    SetAppGlobalState(GlobalState.FileSelected);
                    dataWorker.SourceFileSelected(value);
                }
            };

            dataWorker.NotifyMessageFromData += ErrorNotify.NewError;
            BeginOperation += dataWorker.BeginOperation;
            dataWorker.NotifyIsAsyncRunned += (isRunned) =>
            {
                if (isRunned)
                {
                    _previousApplicationGlobalState = ApplicationGlobalState;
                    SetAppGlobalState(GlobalState.Disabled);
                }
                else
                {
                    SetAppGlobalState(_previousApplicationGlobalState);
                }
            };

        }
            
        
        public string GetAppGlobalState()
        {
            return ApplicationGlobalState.ToString();
        }


        public void SetAppGlobalState(GlobalState newState)
        {
            ApplicationGlobalState = newState;
            if (AppStateChanged != null) AppStateChanged.Invoke(ApplicationGlobalState.ToString());
        }
    }
}
