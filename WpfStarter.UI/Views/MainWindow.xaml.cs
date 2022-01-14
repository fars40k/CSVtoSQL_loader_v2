﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prism.Unity;
using Prism.Regions;
using Prism.Ioc;

namespace WpfStarter.UI.Views
{
    public partial class MainWindow : Window
    {
        IContainerExtension _headerContainer;
        IContainerExtension _footerContainer;
        IRegionManager _regionManager;

        public MainWindow(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();
            _headerContainer = container;
            IRegion region = _regionManager.Regions["HeaderRegion"];
        }
    }

    /*
            

        public MainWindow(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var view = _container.Resolve<ViewA>();
            IRegion region = _regionManager.Regions["ContentRegion"];
            region.Add(view);
        }
     
     
     
     */
}
