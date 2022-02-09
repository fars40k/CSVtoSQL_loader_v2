using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using WpfStarter.Data.Export;
using System.Collections.ObjectModel;


namespace WpfStarter.Data.ViewModels
{
    internal class OperationsViewModel : BindableBase
    {
        public OperationsViewModel(IContainerProvider containerProvider)
        {
            OperationsItems = new ObservableCollection<Operation>();
            var eW = containerProvider.Resolve<DataAccessModel>();

            OperationSelectedCommand = new DelegateCommand<Operation>(eW.OperationSelected);

            if (eW.OperationsListUpdated == null) eW.OperationsListUpdated += AddOperations;
        }

        private ObservableCollection<Operation> operationsItems;

        public ObservableCollection<Operation> OperationsItems
        {
            get { return operationsItems; }
            set { SetProperty(ref operationsItems, value); }
        }

        public DelegateCommand<Operation> OperationSelectedCommand { get; private set; }

        public void AddOperations(IList<IDatabaseAction> actions)
        {
            foreach (var action in actions)
            {
                if (!OperationsItems.Contains(action)) OperationsItems.Add(action as Operation);
            }
        }
    }
}
