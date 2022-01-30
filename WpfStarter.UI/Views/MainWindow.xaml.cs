using System.Windows;
using Prism.Regions;
using Prism.Ioc;
using WpfStarter.UI.Models;
using System.ComponentModel;
using System;

namespace WpfStarter.UI.Views
{
    public partial class MainWindow : Window
    {
        private Model model;
        private IRegionManager regionManager;
  
        public MainWindow(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();

            this.regionManager = regionManager;

            regionManager.RegisterViewWithRegion("HeaderRegion", typeof(Header));
            regionManager.RegisterViewWithRegion("FooterRegion", typeof(Footer));

            model = container.Resolve<Model>();
            Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // Put cancellation request here
        }
    }
    
}
