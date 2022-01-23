using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq.Dynamic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
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
