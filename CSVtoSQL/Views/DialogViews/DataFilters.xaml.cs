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

namespace WpfStarter.UI.Views.DialogViews
{
    /// <summary>
    /// Interaction logic for DataFilters.xaml
    /// </summary>
    public partial class DataFilters : Window
    {
        private Dictionary<ComboBox, object> comboBoxesDataTypes;
        private Dictionary<int,TextBox> parametersTextBoxesbyRow;
        private Dictionary<string, Action> dataFilterActionsPresetNames;

        public DataFilters()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FillCollections();
        }

        /// <summary>
        /// Заполняет коллекции по элементам View
        /// </summary>
        private void FillCollections()
        {
            comboBoxesDataTypes.Add(Combobox_1, new SqlDateTime());
            dataFilterActionsPresetNames.Add(Localisation.Strings.FilterDataAll, SelectAll);
            dataFilterActionsPresetNames.Add(Localisation.Strings.FilterDataContains, SelectContain);
            for (int i = 0; i < this.VisualChildrenCount; i++)
            {
                if (this.GetVisualChild(i) is TextBox)
                {
                    TextBox tB = this.GetVisualChild(i) as TextBox;
                    if (tB != null)
                    {
                        if (tB.Name.Contains("Param"))
                        {

                            int RowNumber = Int32.Parse(tB.Name.Substring(tB.Name.IndexOf("_") + 1, 1));
                            parametersTextBoxesbyRow.Add(RowNumber, tB);
                        }
                    }
                    if (this.GetVisualChild(i) is ComboBox)
                    {
                        ComboBox cB = this.GetVisualChild(i) as ComboBox;
                        if (cB != null)
                        {
                            if (cB.Name.Contains("Combobox_1"))
                            {
                                comboBoxesDataTypes.Add(cB, new SqlDateTime());
                            } else
                            if (cB.Name.Contains("Combobox"))
                            {
                                comboBoxesDataTypes.Add(cB, new SqlString());
                            }
                        }

                    }
                }
            }
        }

        private void SelectAll()
        {
            //Row_6_Param_1.IsVisible = false;
        }

        private void SelectContain()
        {
            // Dynamic Linq here
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
