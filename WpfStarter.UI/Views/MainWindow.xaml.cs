using System.Windows;
using Prism.Regions;
using Prism.Ioc;
using WpfStarter.UI.Models;

namespace WpfStarter.UI.Views
{
    public partial class MainWindow : Window
    {
        Model model;
        IContainerExtension _container;
        IRegionManager _regionManager;

        public MainWindow(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();

            _container = container;
            _regionManager = regionManager;

            regionManager.RegisterViewWithRegion("HeaderRegion", typeof(Header));
            regionManager.RegisterViewWithRegion("FooterRegion", typeof(Footer));

            model = container.Resolve<Model>();
        }

    }
    
}
