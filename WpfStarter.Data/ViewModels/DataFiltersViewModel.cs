using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Resources;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfStarter.Data.ViewModels
{
    internal class DataFiltersViewModel : BindableBase
    {

        public DataFiltersViewModel(IContainerProvider container)
        {
            var eW = container.Resolve<EntityWorker>();
            if (eW.GetLINQShardsRequest == null) eW.GetLINQShardsRequest += GetLINQShards;

            LINQShardsToBuildExpression.AddRange<string>(new List<string>() { "", "", "", "", "", ""});

            var ResourceManager = container.Resolve<ResourceManager>();
            ComboboxEntries = new ObservableCollection<string>();
            ComboboxEntries.Add(ResourceManager.GetString("FilterDataAll") ?? "missing");
            ComboboxEntries.Add(ResourceManager.GetString("FilterDataContains") ?? "missing");

            RowNames = new ObservableCollection<string>();            
            for (int i = 1; i <= LINQShardsToBuildExpression.Count; i++)
            {
                RowNames.Add(ResourceManager.GetString("FilterRow" + i.ToString()) ?? "missing");             
            }

            ComboboxSelectedIndexes = new ObservableCollection<int>();
            for (int i = 0; i < LINQShardsToBuildExpression.Count; i++)
            {
                ComboboxSelectedIndexes.Add(0);
            }

            dataFilterActionsPresets.Add(SelectAll);
            dataFilterActionsPresets.Add(SelectParam);
        }

        private List<Action<int>> dataFilterActionsPresets = new List<Action<int>>();

        private ObservableCollection<string> _LINQShardsToBuildExpression = new ObservableCollection<string>();

        public ObservableCollection<string> LINQShardsToBuildExpression
        {
            get => _LINQShardsToBuildExpression;
            set => SetProperty(ref _LINQShardsToBuildExpression, value);

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

        private List<string> GetLINQShards()
        {
            for(int i = 0; i < LINQShardsToBuildExpression.Count; i++)
            {
                dataFilterActionsPresets[ComboboxSelectedIndexes[i]].Invoke(i);
            }
          
            return LINQShardsToBuildExpression.ToList<string>();
        }

        private void SelectAll(int rowNumber)
        {
            LINQShardsToBuildExpression[rowNumber] = "";
        }

        private void SelectParam(int rowNumber)
        {

        }
    }
}
