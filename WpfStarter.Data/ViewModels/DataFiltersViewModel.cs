using Prism.Ioc;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Resources;

namespace WpfStarter.Data.ViewModels
{
    internal class DataFiltersViewModel : BindableBase
    {

        public DataFiltersViewModel(IContainerProvider container)
        {

            var resManager = container.Resolve<ResourceManager>();
            var eWorker = container.Resolve<EntityWorker>();

            #region Localisation_and_collections_filling

            ComboboxEntries = new ObservableCollection<string>();
            ComboboxEntries.Add(resManager.GetString("FilterDataAll") ?? "missing");
            ComboboxEntries.Add(resManager.GetString("FilterDataContains") ?? "missing");

            RowNames = new ObservableCollection<string>();            
            for (int i = 1; i <= LinqShardsToBuildExpression.Count; i++)
            {
                RowNames.Add(resManager.GetString("FilterRow" + i.ToString()) ?? "missing");             
            }

            ComboboxSelectedIndexes = new ObservableCollection<int>();
            for (int i = 0; i < LinqShardsToBuildExpression.Count; i++)
            {
                ComboboxSelectedIndexes.Add(0);
            }

            _dataFilterActions.Add(SelectAll);
            _dataFilterActions.Add(SelectParam);

            #endregion

            if (eWorker.GetLinqShardsRequest == null) eWorker.GetLinqShardsRequest += GetLINQShards;

        }

        #region Collections

        public ObservableCollection<string> LinqShardsToBuildExpression
        {
            get => _linqShardsToBuildExpression;
            set => SetProperty(ref _linqShardsToBuildExpression, value);

        }

        private ObservableCollection<string> _rowNames;

        public ObservableCollection<string> RowNames
        {
            get => _rowNames;
            set => SetProperty(ref _rowNames, value);
        }

        private ObservableCollection<string> _comboboxEntries;

        public ObservableCollection<string> ComboboxEntries
        {
            get => _comboboxEntries;
            set => SetProperty(ref _comboboxEntries, value);
        }

        private ObservableCollection<int> _comboBoxSelectedIndexes = new ObservableCollection<int>();

        public ObservableCollection<int> ComboboxSelectedIndexes
        {
            get => _comboBoxSelectedIndexes;
            set => SetProperty(ref _comboBoxSelectedIndexes, value);      
        }

        private ObservableCollection<string> _linqShardsToBuildExpression = new ObservableCollection<string>()
                                                                                { "", "", "", "", "", "" };

        private List<Action<int>> _dataFilterActions = new List<Action<int>>();

#endregion

        private List<string> GetLINQShards()
        {
            for(int i = 0; i < LinqShardsToBuildExpression.Count; i++)
            {
                _dataFilterActions[ComboboxSelectedIndexes[i]].Invoke(i);
            }
          
            return LinqShardsToBuildExpression.ToList<string>();
        }

        private void SelectAll(int rowNumber)
        {
            LinqShardsToBuildExpression[rowNumber] = "";
        }

        private void SelectParam(int rowNumber)
        {

        }
    }
}
