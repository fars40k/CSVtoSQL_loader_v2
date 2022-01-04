using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSVtoSQL.Models.Operations
{
    internal class DBLoadFromFile : Operation
    {
        public DBLoadFromFile(MainModel model, string newDescription) : base(model, newDescription)
        {
        }
        public override void Select(object sender, RoutedEventArgs e)
        {
            mainModel.LoadFromFileRequested.Invoke();

        }
    } 

   
}

