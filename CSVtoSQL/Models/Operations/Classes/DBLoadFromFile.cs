using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfStarter.UI.Models.Operations
{
    internal class DBLoadFromFile : Operation
    {
        private string loadParam;

        public DBLoadFromFile(MainModel model, string newDescription, string loadParam) : base(model, newDescription)
        {
        }
        public override void Select(object sender, RoutedEventArgs e)
        {
            mainModel.LoadFromFileRequested.Invoke(loadParam);
        }
    } 

   
}

