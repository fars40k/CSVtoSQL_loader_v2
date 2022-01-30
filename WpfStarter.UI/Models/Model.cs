using Prism.Ioc;
using System;
using System.IO;
using WpfStarter.Data;

namespace WpfStarter.UI.Models
{
    public partial class Model
    {
        public Action BeginOperation;
        public Action<string> AppStateChanged;
        public Action<string> FileSelected;

        private GlobalState _previousApplicationGlobalState;
        private GlobalState _applicationGlobalState;
        public string ApplicationGlobalState
        {
            get
            {
                return _applicationGlobalState.ToString();
            }

            set
            {
                if (Enum.TryParse(value,out _applicationGlobalState))
                {
                    _applicationGlobalState = (GlobalState)Enum.Parse(typeof(GlobalState), value, true);

                } else
                {
                    _applicationGlobalState = GlobalState.CriticalError;
                }

                if (AppStateChanged != null) AppStateChanged.Invoke(ApplicationGlobalState.ToString());
            }
        }

        private IContainerProvider _containerProvider;

        public Model(IContainerProvider container)
        {
            _containerProvider = container;
            var dataPool = container.Resolve<DataObjectPool>();

            Progress<string> dataProgress = container.Resolve<Progress<string>>("DataProgress");
            dataProgress.ProgressChanged += (sender, e) => ErrorNotify.NewError(e);

            ApplyDefaultEventRouting();

            var eWorker = container.Resolve<EntityWorker>();
            DatabaseInitialized(eWorker.DoesDatabaseConnectionInitialized);
        }

        private void DatabaseInitialized(bool IsInitialized)
        {
            if (IsInitialized)
            {
                ApplicationGlobalState = "DbConnected";

            } else
            {
                ApplicationGlobalState = "DbConnectionFailed";                
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
                    ApplicationGlobalState = "FileSelected";
                    dataWorker.SourceFileSelected(value);
                }
            };

            dataWorker.NotifyMessageFromData += ErrorNotify.NewError;
            BeginOperation += dataWorker.PreprocessAndBeginOperation;
            dataWorker.NotifyIsAsyncRunned += (isRunned) =>
            {
                if (isRunned)
                {
                    _previousApplicationGlobalState = _applicationGlobalState;
                    ApplicationGlobalState = "Disabled";
                }
                else
                {
                    ApplicationGlobalState = _previousApplicationGlobalState.ToString();
                }
            };
   
        }

    }
}
