using Prism.Mvvm;
using System.Windows.Controls;
using Prism.Ioc;

namespace WpfStarter.Data.Views
{
    /// <summary>
    /// Interaction logic for DataFilters.xaml
    /// </summary>
    public partial class DataFilters : UserControl
    {
        public DataFilters(IContainerProvider containerProvider)
        {
            InitializeComponent();
        }
    }
}
