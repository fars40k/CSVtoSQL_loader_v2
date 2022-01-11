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
using System.Windows.Shapes;
using WpfStarter.UI.Localisation;
using System.Configuration;
using System.Collections.Specialized;

namespace WpfStarter.UI.Views.DialogViews
{
    /// <summary>
    /// Interaction logic for StringInput.xaml
    /// </summary>
    public partial class StringInput : Window
    {
        public string? CurrentConnString = "";

        public StringInput(string newConnString)
        {
            InitializeComponent();
            this.DataContext = this;
            DB_string.Text = CurrentConnString = newConnString;
        }

        public string ResponseText
        {
            get { return DB_string.Text; }
            set { DB_string.Text = value; }
        }

        private void Accept_button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentConnString = DB_string.Text;      
            DialogResult = true;
        }
    }
}
