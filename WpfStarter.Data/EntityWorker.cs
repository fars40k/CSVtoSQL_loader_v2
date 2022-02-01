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
    public partial class EntityWorker
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

        public EntityWorker(IContainerProvider container)
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
                    ActionsCollection.Add(_container.Resolve<EPPLusSaver>());
                    ActionsCollection.Add(_container.Resolve<XMLSaver>());
                }
                else
                {
                    bool NotHaveItem = true;
                    foreach (IDatabaseAction action in ActionsCollection)
                    {
                        if (action is CSVReader) NotHaveItem = false;
                    }
                    if (NotHaveItem) ActionsCollection.Add(_container.Resolve<CSVReader>());
                }

                if (OperationsListUpdated != null) OperationsListUpdated.Invoke(ActionsCollection);
            }
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
        /// Launches a database operation from an argument
        /// </summary>
        public void PreprocessAndBeginOperation(IDatabaseAction newAction)
        {
            SelectedAction = newAction;
            PreprocessAndBeginOperation();
        }

        /// <summary>
        /// In conformity with implemented interfaces, method doing necessary preparations before launching operation
        /// </summary>
        private void DoPreprocessingForOperation(ResourceManager resourceManager)
        {
            try
            {
                if (SelectedAction == null)
                {
                    throw new ArgumentException();
                }

                if (SelectedAction is IRequiringBuildLinq)
                {
                    IRequiringBuildLinq BuildLinqService = (IRequiringBuildLinq)SelectedAction;

                    // Getting linq shards from Datafilters view
                    List<string> shardsCollection = GetLinqShardsRequest.Invoke();

                    if (shardsCollection != null)
                    {
                        List<string> ContextPropertyNames = new List<string>()
                             {   nameof(Person.Date),
                                 nameof(Person.FirstName),
                                 nameof(Person.SurName),
                                 nameof(Person.LastName),
                                 nameof(Person.City),
                                 nameof(Person.Country)
                             };

                        int inc = 0;

                        BuildLinqService.LinqExpression = "";

                        foreach (string shard in shardsCollection)
                        {
                            if (shard.Trim() != "")
                            {
                                if (BuildLinqService.LinqExpression.Length != 0)
                                {
                                    BuildLinqService.LinqExpression += " && ";
                                }
                                BuildLinqService.LinqExpression +=
                                    ContextPropertyNames[inc] + "== \"" + shard.Trim() + "\"";
                            }

                            inc++;
                        }
                    }
                }

                if (SelectedAction is IRequiringSavepathSelection)
                {
                    IRequiringSavepathSelection savepathService = (IRequiringSavepathSelection)SelectedAction;
                    SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
                    Random random = new Random();

                    dialog.DefaultExt = savepathService.TargetFormat;
                    dialog.FileName = random.Next(0, 9000).ToString();
                    dialog.Filter = "(*" + dialog.DefaultExt + ")|*" + dialog.DefaultExt;
                    dialog.ShowDialog();
                    if ((dialog.FileName != "") && (dialog.FileName.Contains("." + dialog.DefaultExt)))
                    {

                        // Checks if file exist then, to prevent override, saves extracted data in
                        // incremental marked file

                        string nonDuplicatefilePath = dialog.FileName;
                        if (File.Exists(dialog.FileName))
                        {
                            int increment = 0;

                            while (File.Exists(dialog.FileName))
                            {
                                increment++;
                                dialog.FileName = nonDuplicatefilePath
                                                  .Replace(".", ("_" + increment.ToString() + "."));
                            }
                            nonDuplicatefilePath = dialog.FileName;
                        }

                        savepathService.SetSavePath(nonDuplicatefilePath);
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }

                if (SelectedAction is IRequiringSourceFileSelection)
                {
                    IRequiringSourceFileSelection sourceFileService = (IRequiringSourceFileSelection)SelectedAction;
                    sourceFileService.SourceFilePath = SourceFile;
                }

                if ((SelectedAction is IParametrisedAction<Inference>))
                {
                    IParametrisedAction<Inference> action = (IParametrisedAction<Inference>)SelectedAction;
                    Inference obj = _container.Resolve<Inference>();
                    action.Settings = obj;
                }

            }
            catch (ArgumentException ex)
            {
                DoesAnyOperationBeenSetToProcessing = false;
                NotifyMessageFromData.Invoke(resourceManager.GetString("OpNotSelected"));
            }
            catch (Exception ex)
            {
                DoesAnyOperationBeenSetToProcessing = false;
                NotifyMessageFromData.Invoke(ex.Message);
            }

        }

        /// <summary>
        /// Launches a selected operation asynchronously
        /// </summary>
        private void BeginOperationAsync(ResourceManager resourceManager)
        {         
            if (SelectedAction is Operation)
            {
                Operation operationItem = (Operation)SelectedAction;
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        RefreshCancelationTokenAndSource();

                        if (NotifyIsAsyncRunned != null) NotifyIsAsyncRunned.Invoke(true);
                        Task<string> task;
                        task = operationItem.RunTask(_container);
                        if (NotifyIsAsyncRunned != null) NotifyIsAsyncRunned.Invoke(false);

                        // Raises exeptions from the callstack to be handled in caller code
                        if (task.IsCanceled) throw new OperationCanceledException();
                        if (task.IsFaulted) throw task.Exception.InnerException;
                    }
                    catch (OperationCanceledException)
                    {
                        NotifyMessageFromData.Invoke(resourceManager.GetString("OpCanceled"));
                    }
                    catch (Exception ex)
                    {
                        NotifyMessageFromData.Invoke(ex.Message);
                    }
                }).ContinueWith((t) =>
                {
                    // Starting this method here with the method operationItem variable
                    // to prevent postprocessing a changed selectedAction item
                    DoPostprocessingForOperation(resourceManager,operationItem);
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
