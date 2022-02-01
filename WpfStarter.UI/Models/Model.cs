using Prism.Ioc;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WpfStarter.Data;

namespace WpfStarter.UI.Models
{
    public partial class Model
    {
        public Action BeginOperation;
        public Action<string> AppStateChanged;
        public Action<string> FileSelected;

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
            dataProgress.ProgressChanged += async (sender, e) =>
            {
               ErrorNotify.NewError(e);
               await System.Threading.Tasks.Task.Delay(30);
            };

            ApplyDefaultEventRouting();

            var eWorker = container.Resolve<EntityWorker>();
            DatabaseInitialized(eWorker.DoesTheDatabaseConnectionInitialized);               
        }

        /// <summary>
        /// Turns window in a blocked state due to a database connection failure
        /// </summary>
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
                /// If the selected file is empty, throws an error to UI
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
            //dataWorker.NotifyIsAsyncRunned += (isRunned) =>   
        }
    }
}
