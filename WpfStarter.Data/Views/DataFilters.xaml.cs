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

namespace WpfStarter.Data.Views
{
    /// <summary>
    /// Interaction logic for DataFilters.xaml
    /// </summary>
    public partial class DataFilters : UserControl
    {
        public string[] LINQShardsToBuildExpression = new string[6] 
        {"", "", "", "", "", ""};

        private string[] RowNames = new string[6]
        {"Date","FirstName","SurName","LastName","City","Country"};

        private ObservableCollection<string> _comboboxEntries;
        public ObservableCollection<string> ComboboxEntries
        {
            set => _comboboxEntries = value;
            get
            {
                return _comboboxEntries;
            }
        }


        private Type[] rowDataTypes = new Type[6]
        {
            typeof(DateTime), typeof(string), typeof(string),
            typeof(string), typeof(string), typeof(string)
        };

        private List<Action<int>> dataFilterActionsPresets = new List<Action<int>>();

        public DataFilters()
        {
            InitializeComponent();
            this.DataContext = this;      
        }

        public DataFilters(string[] localisedStrings)
        {
            SetLocalizedStrings(localisedStrings);
            this.DataContext = this;
            InitializeComponent();
        }

        private void SetLocalizedStrings(string[] localisedStrings)
        {

            ComboboxEntries = new ObservableCollection<string> { localisedStrings[0], localisedStrings[1] };
            Row_1_Name.Text = localisedStrings[2];
            Row_2_Name.Text = localisedStrings[3];
            Row_3_Name.Text = localisedStrings[4];
            Row_4_Name.Text = localisedStrings[5];
            Row_5_Name.Text = localisedStrings[6];
            Row_6_Name.Text = localisedStrings[7];
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MakeAllParamsInvisible();
            dataFilterActionsPresets.Add(SelectAll);
            dataFilterActionsPresets.Add(SelectParam);
        }

        private void SetParamVisibility(int rowNumber,Visibility newVisibility)
        {
            for (int i = 0; i < this.VisualChildrenCount; i++)
            {
                if (this.GetVisualChild(i) is TextBox)
                {
                    TextBox tB = this.GetVisualChild(i) as TextBox;
                    if (tB.Name.Contains(rowNumber.ToString()))
                    {
                        tB.Visibility = newVisibility;
                    }
                }
            }
        }

        private void MakeAllParamsInvisible()
        {
            Row_Param_1.Visibility = Visibility.Hidden;
            Row_Param_2.Visibility = Visibility.Hidden;
            Row_Param_3.Visibility = Visibility.Hidden;
            Row_Param_4.Visibility = Visibility.Hidden;
            Row_Param_5.Visibility = Visibility.Hidden;
            Row_Param_6.Visibility = Visibility.Hidden;
        }

        private void SelectAll(int rowNumber)
        {
            LINQShardsToBuildExpression[rowNumber] = "";
            SetParamVisibility(rowNumber, Visibility.Hidden);
        }

        private void SelectParam(int rowNumber)
        {
            SetParamVisibility(rowNumber, Visibility.Visible);            
        }

        private void AnySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                ComboBox cB = sender as ComboBox;
                int RowNumber = Int32.Parse(cB.Name.Substring(cB.Name.Length-1, cB.Name.Length));
                int SelectedRow = cB.SelectedIndex;
                dataFilterActionsPresets[SelectedRow].Invoke(RowNumber);
            }
        }

        private void ParameterChangedEvent(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tB = sender as TextBox;
                int RowNumber = Int32.Parse(tB.Name.Substring(tB.Name.Length - 1, 1));

                string ExtractedParam = "";

                if (rowDataTypes[RowNumber] == typeof(DateTime))
                {
                    Regex pattern = new Regex(@"[0-9]{4}-[0-1][0-9]-[0-3][0-9]", RegexOptions.Compiled);
                    if (pattern.IsMatch(tB.Text)) ExtractedParam += tB.Text;
                }
                else
                {
                    ExtractedParam += tB.Text;
                }

                if (ExtractedParam == "")
                {
                    LINQShardsToBuildExpression[RowNumber] = "";
                }
                else
                {
                    LINQShardsToBuildExpression[RowNumber] = RowNames[RowNumber] + " = " + ExtractedParam;
                }
            }


        }
    }
}
