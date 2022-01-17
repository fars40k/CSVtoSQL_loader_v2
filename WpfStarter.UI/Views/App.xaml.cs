using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using WpfStarter.Data;
using WpfStarter.UI.Models;

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
            containerRegistry.RegisterSingleton<Model>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<WpfStarter.Data.ModuleData>();
        }
    }
}
