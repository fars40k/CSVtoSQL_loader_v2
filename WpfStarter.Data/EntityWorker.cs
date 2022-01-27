using System.Windows;
using System.Resources;
using Microsoft.Win32;
using WpfStarter.Data.Export;
using Prism.Ioc;
using Prism.Regions;
using WpfStarter.Data.Views;


namespace WpfStarter.Data
{
    /// <summary>
    /// Data Access model class
    /// </summary>
    public partial class EntityWorker
    {
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

        /// <summary>
        /// In conformity with implemented interfaces, method doing necessary preparations and launches operation
        /// </summary>
        public async void BeginOperation()
        {
            var _resourceManager = container.Resolve<ResourceManager>();
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
                        dialog.Filter = "|*" + dialog.DefaultExt;
                        dialog.ShowDialog();
                        if (dialog.FileName != "")
                        {

                            // Checks if file exist then, to prevent override, saves extracted data in incremental marked file

                            string nonDuplicatefilePath = dialog.FileName;
                            if (File.Exists(dialog.FileName))
                            {
                                int increment = 0;

                                while (File.Exists(nonDuplicatefilePath))
                                {
                                    increment++;
                                    nonDuplicatefilePath = dialog.FileName.Replace(".", ("_" + increment.ToString() + "."));
                                }
                            }

                            savepathService.SetSavePath(nonDuplicatefilePath);
                        }
                    }

                    if (SelectedOperation is IRequiringSourceFileSelection)
                    {
                        IRequiringSourceFileSelection sourceFileService = (IRequiringSourceFileSelection)SelectedOperation;
                        sourceFileService.SourceFilePath = SourceFile;
                    }

                    Operation operationItem = (Operation)SelectedOperation;
                    if (NotifyIsAsyncRunned != null) NotifyIsAsyncRunned.Invoke(true);
                    string result = await operationItem.RunAsync(container,new CancellationToken());
                    if (NotifyIsAsyncRunned != null) NotifyIsAsyncRunned.Invoke(false);
                    NotifyMessageFromData.Invoke(SelectNotifyByOperationResult(result) ?? "missing");

                } else
                {
                    NotifyMessageFromData.Invoke(_resourceManager.GetString("Help4"));
                }
                
            }
            catch (Exception ex)
            {
                NotifyMessageFromData.Invoke(ex.ToString());
            }

        }

        public string SelectNotifyByOperationResult(string result)
        {
            var _resourceManager = container.Resolve<ResourceManager>();
            switch (result)
            {
               
                case "true": return _resourceManager.GetString("Ready");

                case "false": return  _resourceManager.GetString("OpError");

                default: return _resourceManager.GetString("OpErrorAddRecords") + result;

            }
        }

        public void OperationSelected(Operation operation)
        {
            SelectedOperation = operation;
        }
       
    }
}
