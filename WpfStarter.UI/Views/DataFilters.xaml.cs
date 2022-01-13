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

namespace WpfStarter.UI.Views
{
    /// <summary>
    /// Interaction logic for DataFilters.xaml
    /// </summary>
    public partial class DataFilters : UserControl
    {
        private Dictionary<ComboBox, object> comboBoxesDataTypes;
        private Dictionary<string, Action> dataFilterActionsPresets;

        public DataFilters()
        {
            InitializeComponent();
            //this.DataContext = this;
            //this.Loaded += MainWindow_Loaded;
        }

        /* private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FillCollectionsAndComboboxes();
            MakeAllParamsInvisible();
        }

        private void FillCollectionsAndComboboxes()
        {
            comboBoxesDataTypes.Add(Combobox_1, new SqlDateTime());
            dataFilterActionsPresets.Add(Localisation.Strings.FilterDataAll, SelectAll);
            dataFilterActionsPresets.Add(Localisation.Strings.FilterDataContains, SelectContain);

            for (int i = 0; i < this.VisualChildrenCount; i++)
            {
                if (this.GetVisualChild(i) is ComboBox)
                {
                    ComboBox cB = this.GetVisualChild(i) as ComboBox;
                    if (cB != null)
                    {
                        if (cB.Name.Contains("Combobox_1"))
                        {
                            comboBoxesDataTypes.Add(cB, new SqlDateTime());
                            cB.ItemsSource = dataFilterActionsPresets.Keys;
                        }
                        else
                        if (cB.Name.Contains("Combobox"))
                        {
                            comboBoxesDataTypes.Add(cB, new SqlString());
                            cB.ItemsSource = dataFilterActionsPresets.Keys;
                        }
                    }

                }

            }
        }

        private void SelectAll()
        {
            
        }

        private void SelectContain()
        {
            
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MakeParamVisible(int rowNumber)
        {
            for (int i = 0; i < this.VisualChildrenCount; i++)
            {
                if (this.GetVisualChild(i) is TextBox)
                {
                    TextBox tB = this.GetVisualChild(i) as TextBox;
                    if (tB.Name.Contains(rowNumber.ToString()))
                    {
                        tB.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void MakeAllParamsInvisible()
        {
            Row_1_Param.Visibility = Visibility.Hidden;
            Row_2_Param.Visibility = Visibility.Hidden;
            Row_3_Param.Visibility = Visibility.Hidden;
            Row_4_Param.Visibility = Visibility.Hidden;
            Row_5_Param.Visibility = Visibility.Hidden;
            Row_6_Param.Visibility = Visibility.Hidden;
        }*/
    }
}
