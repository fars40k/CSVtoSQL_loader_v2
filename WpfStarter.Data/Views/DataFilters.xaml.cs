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

namespace WpfStarter.Data.Views
{
    /// <summary>
    /// Interaction logic for DataFilters.xaml
    /// </summary>
    public partial class DataFilters : UserControl
    {
        private string[] LINQShardsToBuildString;
        private string[] RowsNames;
        private Type[] rowDataTypes = new Type[6]
        {
            typeof(DateTime), typeof(string), typeof(string),
            typeof(string), typeof(string), typeof(string)
        };
        private Dictionary<string, Action<int>> dataFilterActionsPresets;

        public DataFilters(string[] localisedStrings)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += MainWindow_Loaded;
            if (localisedStrings.Length == 2)
            {
                dataFilterActionsPresets.Add(localisedStrings[0], (a) => NoSettings(a));
                dataFilterActionsPresets.Add(localisedStrings[1], (a) => SelectParam(a));
            }            
        }

        public string BuildLINQExpressionFromSettings()
        {
            foreach (string shard in LINQShardsToBuildString)
            {

            }
            return "";
        }


        private void SetRowsName(string[] rowsNames)
        {

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MakeAllParamsInvisible();

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
        }

        private void NoSettings(int rowNumber)
        {

        }

        private void SelectParam(int rowNumber)
        {
            if (rowDataTypes[rowNumber] == typeof(DateTime))
            {

            } else
            {
                for (int i = 0; i < this.VisualChildrenCount; i++)
                {
                    if (this.GetVisualChild(i) is TextBox)
                    {
                        TextBox tB = this.GetVisualChild(i) as TextBox;
                        if (tB.Name.Contains(rowNumber.ToString()))
                        {
                            //
                        }
                    }
                }
            }
        }
    }
}
