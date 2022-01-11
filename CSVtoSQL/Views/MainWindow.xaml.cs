using System;
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
using WpfStarter.UI.Models;
using WpfStarter.UI.ViewModels;

namespace WpfStarter.UI.Views
{
    /// <summary>
    /// Contains resources used by the main window
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainViewModel MainWindowViewModel { get; private set; }
        public MainModel MainWindowModel { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            MainWindowViewModel = new MainViewModel(this);
            MainWindowModel = new MainModel(MainWindowViewModel);

            this.DataContext = MainWindowViewModel;
            this.ResizeMode = ResizeMode.CanResize;

            MainWindowViewModel.SetModelReference(MainWindowModel);
            MainWindowViewModel.UpdateUserUI.Invoke();
        }
    }
}
