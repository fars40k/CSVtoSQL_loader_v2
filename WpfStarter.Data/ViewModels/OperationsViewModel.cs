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
            OperationSelectedCommand = new DelegateCommand<Operation>(OperationSelected);

            var eW = containerProvider.Resolve<EntityWorker>();
            AddOperations(eW.DatabaseOperationsServices);
        }

        public void OperationSelected(Operation operation)
        {

        }

        public void AddOperations(IList<IDatabaseAction> actions)
        {
            foreach (IDatabaseAction action in actions)
            {
                OperationsItems.Add(action as Operation);
            }
        }
    }
}
