using WpfStarter.Data.Export;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Regions;
using WpfStarter.Data.Views;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Resources;
using System.Resources;
using System.Reflection;
using WpfStarter.Data.ViewModels;

namespace WpfStarter.Data
{
    public class EntityWorker
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
        public Action<string> NotifyDataAccessError;
        public Action<List<IDatabaseAction>> OperationsListUpdated;
        public Func<List<string>> GetLINQShardsRequest;

        public IDatabaseAction SelectedOperation { get; private set; }
        private IContainerProvider _container;
        public List<IDatabaseAction> DatabaseOperationsServices { get; private set; } = new List<IDatabaseAction>();

        public List<string> contextPropertyNames { get; private set; }
            = new List<string>()
            {
                nameof(Person.Date), nameof(Person.FirstName), nameof(Person.SurName),
                nameof(Person.LastName), nameof(Person.City), nameof(Person.Country)
            };

        private string _currentError { get; set; }
        public string CurrentError 
        { get => _currentError;
            set
            {
                _currentError = value;
                if (NotifyDataAccessError != null) NotifyDataAccessError.Invoke(value);
            }
        }    
        public string SourceFile { get; private set; }

        public bool DoesDatabaseConnectionInitialized { get; private set; } = false;

        public void VerifyConnection()
        {
            try
            {
                using (PersonsContext pC = new PersonsContext())
                {
                    pC.Database.CreateIfNotExists();
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

        private void AddDataViewsToRegions()
        {
            if (SourceFile == null)
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

        private void AddOperationsToList()
        {
            if (SourceFile == null)
            {
                DatabaseOperationsServices.Add(_container.Resolve<EPPLusSaver>());
                DatabaseOperationsServices.Add(_container.Resolve<XMLSaver>());
            }
            else
            {
                bool CanCreateFlag = true;
                foreach (IDatabaseAction action in DatabaseOperationsServices)
                {
                    if (action is CSVReader) CanCreateFlag = false;
                }
                if (CanCreateFlag) DatabaseOperationsServices.Add(_container.Resolve<CSVReader>());
            }

            if (OperationsListUpdated != null) OperationsListUpdated.Invoke(DatabaseOperationsServices);
        }

        public void BeginOperation()
        {
            try
            {
                var _resourceManager = _container.Resolve<ResourceManager>();
                NotifyDataAccessError(_resourceManager.GetString("Help6") ?? "missing");

                if (SelectedOperation != null)
                {
                    if (SelectedOperation is ILinqBuildRequired)
                    {
                        ILinqBuildRequired linqBuildRequired = (ILinqBuildRequired)SelectedOperation;
                        List<string> shards = GetLINQShardsRequest.Invoke();
                        if (shards != null)
                        {
                            int i = 0;
                            foreach (string shard in shards)
                            {
                              if (shard != "")
                                {
                                    if (linqBuildRequired.LINQExpression.Length == 0)
                                    {
                                        linqBuildRequired.LINQExpression += " && ";
                                    }
                                    linqBuildRequired.LINQExpression += contextPropertyNames[i] + "== \"" + shard + "\"";
                                }
                                i++;
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
                        dlg.Filter = " (*" + dlg.DefaultExt + ")|*" + dlg.DefaultExt;
                        dlg.ShowDialog();
                        if (dlg.FileName != "")
                        {
                            savePathSelection.SetSavePath(dlg.FileName);
                        }
                    }

                    switch (SelectedOperation)
                    {
                        case CSVReader:
                            {
                                CSVReader obj = SelectedOperation as CSVReader;
                                obj.Run(SourceFile);
                                break;
                            }

                        case EPPLusSaver:
                            {
                                EPPLusSaver obj = SelectedOperation as EPPLusSaver;
                                obj.Run();
                                break;
                            }
                        case XMLSaver:
                            {
                                XMLSaver obj = SelectedOperation as XMLSaver;
                                obj.Run();
                                break;
                            }

                    }                   
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        
            NotifyDataAccessError("");
        }


        public void OperationSelected(Operation operation)
        {
            SelectedOperation = operation;
        }


       /* private void RemoveOperationSelection()
        {
            SelectedOperation = null;
            IRegionManager regionManager = _container.Resolve<IRegionManager>();
            Operations view = _container.Resolve<Operations>();
            IRegion region = regionManager.Regions["OperationsRegion"];
            region.RemoveAll();
            region.Add(view);
        }
       */
    }
}
