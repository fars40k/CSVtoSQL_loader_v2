using System.Windows;
using Prism.Regions;
using Prism.Ioc;
using WpfStarter.UI.Models;
using System.ComponentModel;
using System;
using System.Threading;

namespace WpfStarter.UI.Views
{
    public partial class MainWindow : Window
    {
        private Model _model;
        private IRegionManager _regionManager;
        private IContainerExtension _container;
  
        public MainWindow(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();

            this._regionManager = regionManager;

            regionManager.RegisterViewWithRegion("HeaderRegion", typeof(Header));
            regionManager.RegisterViewWithRegion("FooterRegion", typeof(Footer));

            _model = container.Resolve<Model>();
            this._container = container;
            Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                CancellationTokenSource source = 
                    _container.Resolve<CancellationTokenSource>("DataCancellationSource");
                source.Cancel();
            }
            catch (Exception ex)
            {

            }
            Thread.Sleep(500);
        }
    }
    
}
