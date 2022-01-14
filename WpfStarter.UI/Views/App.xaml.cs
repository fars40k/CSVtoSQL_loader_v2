using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Prism.Regions;


namespace WpfStarter.UI.Views
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
