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
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;
        private IContainerProvider container;

        public string SourceFile { get; private set; }

        public List<IDatabaseAction> OperationsCollection { get; private set; } = new List<IDatabaseAction>();
        public IDatabaseAction SelectedOperation { get; private set; }
        public bool DoesDatabaseConnectionInitialized { get; private set; } = false;

        public Action UpdateDataViews;
        public Action<bool> NotifyIsAsyncRunned;
        public Action<string> SourceFileSelected;
        public Action<string> NotifyMessageFromData;
        public Action<List<IDatabaseAction>> OperationsListUpdated;
        public Func<List<string>> GetLinqShardsRequest;

        public EntityWorker(IContainerProvider container)
        {
            this.container = container;
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
        /// Checks connection to Database and
        /// </summary>
        public void VerifyConnection()
        {
            try
            {
                using (PersonsContext pC = new PersonsContext())
                {
                    pC.Database.Connection.Open();
                    pC.Database.Connection.Close();
                    this.DoesDatabaseConnectionInitialized = true;
                }
            }
            catch (Exception ex)
            {
                this.DoesDatabaseConnectionInitialized = false;
            }
        }

        /// <summary>
        /// Method for adding Views to correspondent regions
        /// </summary>
        private void AddDataViewsToRegions()
        {
            if ((SourceFile == null)&&(DoesDatabaseConnectionInitialized))
            {
                IRegionManager regionManager = container.Resolve<IRegionManager>();

                Operations view = container.Resolve<Operations>();
                IRegion region = regionManager.Regions["OperationsRegion"];
                if (region.ActiveViews.Count() == 0) region.Add(view);

                DataFilters filters = container.Resolve<DataFilters>();
                region = regionManager.Regions["FiltersRegion"];
                if (region.ActiveViews.Count() == 0) region.Add(filters);
            }
        }

        /// <summary>
        /// Updating the list of operations according to the inner state of the EntityWorker
        /// </summary>
        private void AddOperationsToList()
        {
            if (DoesDatabaseConnectionInitialized)
            {
                if (SourceFile == null)
                {
                    OperationsCollection.Add(container.Resolve<EPPLusSaver>());
                    OperationsCollection.Add(container.Resolve<XMLSaver>());
                }
                else
                {
                    bool NotHaveItem = true;
                    foreach (IDatabaseAction action in OperationsCollection)
                    {
                        if (action is CSVReader) NotHaveItem = false;
                    }
                    if (NotHaveItem) OperationsCollection.Add(container.Resolve<CSVReader>());
                }

                if (OperationsListUpdated != null) OperationsListUpdated.Invoke(OperationsCollection);
            }
        }


        public void PreprocessAndBeginOperation()
        {
            var resourceManager = container.Resolve<ResourceManager>();
           
            PreprocessOperation(resourceManager);
            BeginOperationAsync(resourceManager);

        }

        /// <summary>
        /// In conformity with implemented interfaces, method doing necessary preparations before launching operation
        /// </summary>
        private void PreprocessOperation(ResourceManager resourceManager)
        {
            try
            {
                if (SelectedOperation != null)
                {
                    if (SelectedOperation is IRequiringBuildLinq)
                    {
                        IRequiringBuildLinq BuildLinqService = (IRequiringBuildLinq)SelectedOperation;

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

                            foreach (string shard in shardsCollection)
                            {
                                if (shard != "")
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

                    if (SelectedOperation is IRequiringSavepathSelection)
                    {
                        IRequiringSavepathSelection savepathService = (IRequiringSavepathSelection)SelectedOperation;
                        SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
                        Random random = new Random();

                        dialog.DefaultExt = savepathService.TargetFormat;
                        dialog.FileName = random.Next(0, 9000).ToString();
                        dialog.Filter = "(*" + dialog.DefaultExt + ")|*" + dialog.DefaultExt;
                        dialog.ShowDialog();
                        if (dialog.FileName != "")
                        {

                            // Checks if file exist then, to prevent override, saves extracted data in incremental marked file

                            string nonDuplicatefilePath = dialog.FileName;
                            if (File.Exists(dialog.FileName))
                            {
                                int increment = 0;

                                while (File.Exists(dialog.FileName))
                                {
                                    increment++;
                                    dialog.FileName = nonDuplicatefilePath.Replace(".", ("_" + increment.ToString() + "."));
                                }
                                nonDuplicatefilePath = dialog.FileName;
                            }

                            savepathService.SetSavePath(nonDuplicatefilePath);
                        }
                    }

                    if (SelectedOperation is IRequiringSourceFileSelection)
                    {
                        IRequiringSourceFileSelection sourceFileService = (IRequiringSourceFileSelection)SelectedOperation;
                        sourceFileService.SourceFilePath = SourceFile;
                    }

                    if ((SelectedOperation is IParametrisedAction<Inference>))
                    {
                        IParametrisedAction<Inference> action = (IParametrisedAction<Inference>)SelectedOperation;
                        Inference obj = container.Resolve<Inference>();
                        action.Settings = obj;
                    }
                }
                else
                {
                    NotifyMessageFromData.Invoke(resourceManager.GetString("Help4"));
                }
            }
            catch (Exception ex)
            {
                NotifyMessageFromData.Invoke(ex.Message);
            }

        }

        /// <summary>
        /// Launches a selected operation asynchronously
        /// </summary>
        private void BeginOperationAsync(ResourceManager resourceManager)
        {         
            if (SelectedOperation is Operation)
            {
                Operation operationItem = (Operation)SelectedOperation;
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        RefreshCancelationTokenAndSource();

                        if (NotifyIsAsyncRunned != null) NotifyIsAsyncRunned.Invoke(true);
                        Task<string> task;
                        task = operationItem.RunTask(container);
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
                    PostprocessOperation(resourceManager);
                });              
            } else
            {
                NotifyMessageFromData.Invoke(resourceManager.GetString("OpNotProcessed"));
            }
        }

        /// <summary>
        /// By operation resulsts makes smth
        /// </summary>
        private void PostprocessOperation(ResourceManager resourceManager)
        {
            if (SelectedOperation is IParametrisedAction<Inference>)
            {
                IParametrisedAction<Inference> parametrisedAction = (IParametrisedAction<Inference>)SelectedOperation;
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
            SelectedOperation = operation;
        }

        /// <summary>
        /// Rewrites the registered instance of a cancellation token if cancellation were requested or not yet created
        /// </summary>
        public void RefreshCancelationTokenAndSource()
        {
            IContainerExtension extension= container.Resolve<IContainerExtension>();
            if ((cancellationTokenSource == null) || (cancellationTokenSource.IsCancellationRequested))
            {
                cancellationTokenSource = container.Resolve<CancellationTokenSource>();
                extension.RegisterInstance<CancellationTokenSource>(cancellationTokenSource, "DataCancellationSource");
                cancellationToken = cancellationTokenSource.Token;
                extension.RegisterInstance<CancellationToken>(cancellationToken, "DataCancellationToken");
            }
        }
    }
}
