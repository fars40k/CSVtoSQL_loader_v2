using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfStarter.Data.Export;

namespace WpfStarter.Data.ViewModels
{
    internal class OperationsViewModel : BindableBase
    {
        private ObservableCollection<Operation> _operationsItems;

        public ObservableCollection<Operation> OperationsItems
        {
            get { return _operationsItems; }
            set { SetProperty(ref _operationsItems, value); }
        }

        public DelegateCommand<Operation> OperationSelectedCommand { get; private set; }

        public OperationsViewModel(IContainerProvider containerProvider)
        {
            OperationsItems = new ObservableCollection<Operation>();
            var eW = containerProvider.Resolve<EntityWorker>();
            OperationSelectedCommand = new DelegateCommand<Operation>(eW.OperationSelected);
            if (eW.OperationsListUpdated == null) eW.OperationsListUpdated += AddOperations;
        }

        public void AddOperations(IList<IDatabaseAction> actions)
        {
            foreach (var action in actions)
            {
                if (!OperationsItems.Contains(action)) OperationsItems.Add(action as Operation);
            }
        }
    }
}
