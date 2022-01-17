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

namespace WpfStarter.Data.ViewModel
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
            OperationsItems.Add(new Operation(""));
            OperationsItems.Add(new Operation("fdsdffsd"));



        }

        public void OperationSelected(Operation operation)
        {

        }
    }
}
