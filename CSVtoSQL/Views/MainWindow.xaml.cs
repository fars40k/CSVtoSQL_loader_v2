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
using CSVtoSQL.Models;
using CSVtoSQL.ViewModels;

namespace CSVtoSQL.Views
{
    /// <summary>
    /// Содержит ресурсы относящиеся к Основному окну приложения
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public MainViewModel MainWindowViewModel { get; private set; }
        public MainModel MainWindowModel { get; private set; }

        /// <summary>
        /// Устанавливает зависимости окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel = new MainViewModel(this);
            MainWindowModel = new MainModel(MainWindowViewModel);
            SetDependencies();
            SetBehaviour();
            MainWindowViewModel.UpdateUserUI.Invoke();
        }

        /// <summary>
        /// Cвязывает Model и ViewModel
        /// </summary>
        private void SetDependencies()
        {

            this.DataContext = MainWindowViewModel;
            MainWindowViewModel.SetModelReference(MainWindowModel);

        }

        /// <summary>
        /// Устанавливает поведение окна
        /// </summary>
        private void SetBehaviour()
        {

            this.ResizeMode = ResizeMode.CanResize;

        }


    }
}
