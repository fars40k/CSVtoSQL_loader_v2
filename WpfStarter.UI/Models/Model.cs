using Prism.Ioc;
using System;
using System.IO;
using WpfStarter.Data;

namespace WpfStarter.UI.Models
{
    public partial class Model
    {
        private IContainerProvider _containerProvider;

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

                if (ApplicationStateChanged != null) ApplicationStateChanged.Invoke(ApplicationGlobalState.ToString());
            }
        }

        public Action BeginOperation;
        public Action<string> ApplicationStateChanged;
        public Action<string> FileSelected;

        public Model(IContainerProvider container)
        {
            _containerProvider = container;  
            var dataPool = container.Resolve<DataObjectPool>();

            // Showing asynchronious operations progress on UI
            Progress<string> dataProgress = container.Resolve<Progress<string>>("DataProgress");
            dataProgress.ProgressChanged += async (sender, e) =>
            {
               ErrorNotify.NewError(e);
               await System.Threading.Tasks.Task.Delay(30);
            };

            ApplyDefaultEventRouting();

            // Resolving the Data Access model to check database connection
            var dataAccess = container.Resolve<DataAccessModel>();
            DatabaseInitialized(dataAccess.DoesTheDatabaseConnectionInitialized);
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
            var dataWorker = _containerProvider.Resolve<DataAccessModel>();

            // Changing application state and UI if file not empty
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

            // Binds method to begin operation command from UI
            BeginOperation += dataWorker.PreprocessAndBeginOperation;
        }
    }
}
