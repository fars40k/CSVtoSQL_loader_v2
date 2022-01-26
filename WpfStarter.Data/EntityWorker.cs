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

        public EntityWorker(IContainerProvider container)
        {
            _container = container;
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

        public Action UpdateDataViews;
        public Action<string> SourceFileSelected;
        public Action<string> NotifyDataAccess;
        public Action<List<IDatabaseAction>> OperationsListUpdated;
        public Func<List<string>> GetLINQShardsRequest;

        private IContainerProvider _container;
        public List<IDatabaseAction> OperationsCollection { get; private set; } = new List<IDatabaseAction>();
        public IDatabaseAction SelectedOperation { get; private set; }

        private string _currentError { get; set; }
        public string CurrentError 
        { get => _currentError;
            set
            {
                _currentError = value;
                if (NotifyDataAccess != null) NotifyDataAccess.Invoke(value);
            }
        }    

        public string SourceFile { get; private set; }

        public bool DoesDatabaseConnectionInitialized { get; private set; } = false;

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
            if (DoesDatabaseConnectionInitialized)
            {
                if (SourceFile == null)
                {
                    OperationsCollection.Add(_container.Resolve<EPPLusSaver>());
                    OperationsCollection.Add(_container.Resolve<XMLSaver>());
                }
                else
                {
                    bool NotHaveItem = true;
                    foreach (IDatabaseAction action in OperationsCollection)
                    {
                        if (action is CSVReader) NotHaveItem = false;
                    }
                    if (NotHaveItem) OperationsCollection.Add(_container.Resolve<CSVReader>());
                }

                if (OperationsListUpdated != null) OperationsListUpdated.Invoke(OperationsCollection);
            }
        }

        /// <summary>
        /// In conformity with implemented interfaces, method doing necessary preparations and launches operation
        /// </summary>
        public void BeginOperation()
        {
            var _resourceManager = _container.Resolve<ResourceManager>();
            try
            {               
                NotifyDataAccess(_resourceManager.GetString("Help6") ?? "missing");

                if (SelectedOperation != null)
                {
                    if (SelectedOperation is ILinqBuildRequired)
                    {
                        ILinqBuildRequired linqBuildRequired = (ILinqBuildRequired)SelectedOperation;

                        List<string> shards = new List<string>();
                        shards = GetLINQShardsRequest.Invoke();

                        if (shards != null)
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

                            foreach (string shard in shards)
                            {

                                if (shard != "")
                                {
                                    if (linqBuildRequired.LINQExpression.Length != 0)
                                    {
                                        linqBuildRequired.LINQExpression += " && ";
                                    }
                                    linqBuildRequired.LINQExpression += 
                                        ContextPropertyNames[inc] + "== \"" + shard.Trim() + "\"";
                                }
                                inc++;
                            }
                        }                       
                    }

                    if (SelectedOperation is ISavePathSelectionRequired)
                    {
                        ISavePathSelectionRequired savePathSelection = (ISavePathSelectionRequired)SelectedOperation;
                        SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                        Random random = new Random();

                        dlg.DefaultExt = savePathSelection.TargetFormat;
                        dlg.FileName = random.Next(0, 9000).ToString();
                        dlg.Filter = "|*" + dlg.DefaultExt;
                        dlg.ShowDialog();
                        if (dlg.FileName != "")
                        {

                            // Checks if file exist then, to prevent override, saves extracted data in incremental marked file

                            string nonDuplicatefilePath = dlg.FileName;
                            if (File.Exists(dlg.FileName))
                            {
                                int increment = 0;

                                while (File.Exists(nonDuplicatefilePath))
                                {
                                    increment++;
                                    nonDuplicatefilePath = dlg.FileName.Replace(".", ("_" + increment.ToString() + "."));
                                }
                            }

                            savePathSelection.SetSavePath(nonDuplicatefilePath);
                        }
                    }

                    if (SelectedOperation is ISourceFileSelectionRequired)
                    {
                        ISourceFileSelectionRequired sourceFileSelection = (ISourceFileSelectionRequired)SelectedOperation;
                        sourceFileSelection.SourceFilePath = SourceFile;
                    }

                    Operation operationItem = (Operation)SelectedOperation;
                    string result = operationItem.Run();
                    NotifyDataAccess.Invoke(SelectNotifyByOperationResult(result) ?? "missing");

                } else
                {
                    NotifyDataAccess.Invoke(_resourceManager.GetString("Help4"));
                }
                
            }
            catch (Exception ex)
            {
                NotifyDataAccess.Invoke(_resourceManager.GetString("OpError"));
            }
        }

        public string SelectNotifyByOperationResult(string result)
        {
            var _resourceManager = _container.Resolve<ResourceManager>();
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
