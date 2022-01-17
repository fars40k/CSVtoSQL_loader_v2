using Prism.Commands;
using Prism.Mvvm;
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
        private ObservableCollection<Person> _operations;

        public ObservableCollection<Person> Operations
        {
            get { return _operations; }
            set { SetProperty(ref _operations, value); }
        }

        public DelegateCommand<Operation> OperationSelectedCommand { get; private set; }

        public OperationsViewModel()
        {


        }
    }
}
