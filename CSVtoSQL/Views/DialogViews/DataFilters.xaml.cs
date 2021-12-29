using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
using System.Windows.Shapes;

namespace CSVtoSQL.Views.DialogViews
{
    /// <summary>
    /// Interaction logic for DataFilters.xaml
    /// </summary>
    public partial class DataFilters : Window
    {
        private Dictionary<ComboBox, object> comboBoxesToFilterPreset;
        private Dictionary<string,Action> DatefilterPresetNames;

        public DataFilters()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FillComboBoxFilterPresset();
        }

        private void FillComboBoxFilterPresset()
        {
            comboBoxesToFilterPreset.Add(Combobox_1, new SqlDateTime());
            comboBoxesToFilterPreset.Add(Combobox_2, new SqlString());
            comboBoxesToFilterPreset.Add(Combobox_3, new SqlString());
            comboBoxesToFilterPreset.Add(Combobox_4, new SqlString());
            comboBoxesToFilterPreset.Add(Combobox_5, new SqlString());
            comboBoxesToFilterPreset.Add(Combobox_6, new SqlString());
        }
    }
}
