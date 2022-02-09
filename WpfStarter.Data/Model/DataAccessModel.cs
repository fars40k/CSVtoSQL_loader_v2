using System.Windows;
using System.Resources;
using Microsoft.Win32;
using WpfStarter.Data.Export;
using Prism.Ioc;
using Prism.Regions;
using WpfStarter.Data.Views;
using System;

namespace WpfStarter.Data
{
    /// <summary>
    /// Data Access model class
    /// </summary>
    public partial class DataAccessModel
    {
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private IContainerProvider _container;

        public string SourceFile { get; private set; }

        public List<IDatabaseAction> ActionsCollection { get; private set; } = new List<IDatabaseAction>();
        public IDatabaseAction SelectedAction { get; private set; }
        public bool DoesTheDatabaseConnectionInitialized { get; private set; } = false;
        public bool DoesAnyOperationBeenSetToProcessing { get; private set; } = false;

        public Action UpdateDataViews;
        public Action<bool> NotifyIsAsyncRunned;
        public Action<string> SourceFileSelected;
        public Action<string> NotifyMessageFromData;
        public Action<List<IDatabaseAction>> OperationsListUpdated;
        public Func<List<string>> GetLinqShardsRequest;

        public DataAccessModel(IContainerProvider container)
        {
            this._container = container;
            VerifyConnection();
            RefreshCancelationTokenAndSource();

            UpdateDataViews += () =>
            {
                AddDataViewsToRegions();
                AddOperationsToList();
            };

            SourceFileSelected += (s) =>
            {
                SourceFile = s;
                UpdateDataViews.Invoke();
            };
        }

        /// <summary>
        /// Checks connection to Database and sets DoesDatabaseConnectionInitialized flag
        /// </summary>
        public void VerifyConnection()
        {
            try
            {
                using (PersonsContext pC = new PersonsContext())
                {
                    pC.Database.Connection.Open();
                    pC.Database.Connection.Close();
                    this.DoesTheDatabaseConnectionInitialized = true;
                }
            }
            catch (Exception ex)
            {
                this.DoesTheDatabaseConnectionInitialized = false;
            }
        }

        /// <summary>
        /// Method for adding Views to correspondent regions
        /// </summary>
        private void AddDataViewsToRegions()
        {
            if ((SourceFile == null)&&(DoesTheDatabaseConnectionInitialized))
            {
                IRegionManager regionManager = _container.Resolve<IRegionManager>();

                Operations view = _container.Resolve<Operations>();
                IRegion region = regionManager.Regions["OperationsRegion"];
                if (region.ActiveViews.Count() == 0) region.Add(view);

                DataFilters filters = _container.Resolve<DataFilters>();
                region = regionManager.Regions["FiltersRegion"];
                if (region.ActiveViews.Count() == 0) region.Add(filters);
            }
        }

        /// <summary>
        /// Updating the list of operations according to the inner state of the EntityWorker
        /// </summary>
        private void AddOperationsToList()
        {
            if (DoesTheDatabaseConnectionInitialized)
            {
                if (SourceFile == null)
                {
                    ActionsCollection.Add(_container.Resolve<DefaultExcelExporter>());
                    ActionsCollection.Add(_container.Resolve<DefaultXmlExporter>());
                }
                else
                {
                    bool notHaveItem = true;
                    foreach (IDatabaseAction action in ActionsCollection)
                    {
                        if (action is DefaultCsvFileReader) notHaveItem = false;
                    }
                    if (notHaveItem) ActionsCollection.Add(_container.Resolve<DefaultCsvFileReader>());
                }

                if (OperationsListUpdated != null) OperationsListUpdated.Invoke(ActionsCollection);
            }
        }

        /// <summary>
        /// Launches a database operation from an argument
        /// </summary>
        public void PreprocessAndBeginOperation(IDatabaseAction newAction)
        {
            SelectedAction = newAction;
            PreprocessAndBeginOperation();
        }

        /// <summary>
        /// Launches a database operation
        /// </summary>
        public void PreprocessAndBeginOperation()
        {
            var resourceManager = _container.Resolve<ResourceManager>();
            DoesAnyOperationBeenSetToProcessing = true;

            DoPreprocessingForOperation(resourceManager);
            if (DoesAnyOperationBeenSetToProcessing) BeginOperationAsync(resourceManager);

        }

        /// <summary>
        /// In conformity with implemented interfaces, method doing necessary preparations before launching operation
        /// </summary>
        private void DoPreprocessingForOperation(ResourceManager resourceManager)
        {

                

                DoesAnyOperationBeenSetToProcessing = false;
        }

        /// <summary>
        /// Launches a selected operation asynchronously
        /// </summary>
        private void BeginOperationAsync(ResourceManager resourceManager)
        {         
            if (SelectedAction is Operation)
            {
                Operation operationItem = (Operation)SelectedAction;

                Task task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        RefreshCancelationTokenAndSource();

                        if (NotifyIsAsyncRunned != null) NotifyIsAsyncRunned.Invoke(true);
                        Task<string> task = operationItem.RunTask(_container);
                        if (NotifyIsAsyncRunned != null) NotifyIsAsyncRunned.Invoke(false);

                        // Raises exeptions from the callstack to be handled in caller code
                        if (task.IsCanceled) throw new OperationCanceledException();
                        if (task.IsFaulted) throw task.Exception.InnerException;

                        DoPostprocessingForOperation(resourceManager, operationItem);
                    }
                    catch (OperationCanceledException)
                    {
                        NotifyMessageFromData.Invoke(resourceManager.GetString("OpCanceled"));
                    }
                    catch (Exception ex)
                    {
                        NotifyMessageFromData.Invoke(ex.Message);
                    }
                });              
            } else
            {
                NotifyMessageFromData.Invoke(resourceManager.GetString("OpNotProcessed"));
            }
        }

        /// <summary>
        /// Doing some actions after operation, derived from implemented interfaces
        /// </summary>
        private void DoPostprocessingForOperation(ResourceManager resourceManager,IDatabaseAction completedAction)
        {
            if (completedAction is IParametrisedAction<Inference>)
            {
                IParametrisedAction<Inference> parametrisedAction = (IParametrisedAction<Inference>)completedAction;
                Inference inference = (Inference)parametrisedAction.Settings;

                string result = inference.ToString();

                if (inference.TotalFailed != 0)
                {

                    result = resourceManager.GetString("OpReadyWithErrors") + " " + result;

                } else
                {

                    result = resourceManager.GetString("OpRecordsAdded") + " " + result;

                }

                NotifyMessageFromData.Invoke(result);
            }
        }

        public void OperationSelected(Operation operation)
        {
            SelectedAction = operation;
        }

        /// <summary>
        /// Rewrites the registered instance of a cancellation token if cancellation were requested or not yet created
        /// </summary>
        public void RefreshCancelationTokenAndSource()
        {
            IContainerExtension extension= _container.Resolve<IContainerExtension>();
            if ((_cancellationTokenSource == null) || (_cancellationTokenSource.IsCancellationRequested))
            {

                _cancellationTokenSource = _container.Resolve<CancellationTokenSource>();
                extension.RegisterInstance<CancellationTokenSource>(_cancellationTokenSource, "DataCancellationSource");
                _cancellationToken = _cancellationTokenSource.Token;
                extension.RegisterInstance<CancellationToken>(_cancellationToken, "DataCancellationToken");

            }
        }
    }
}
