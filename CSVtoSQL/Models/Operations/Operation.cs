using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSVtoSQL.Models
{
    /// <summary>
    /// Базовый клас операций над данными
    /// </summary>
    public class Operation
    {
        protected MainModel mainModel;
        public string Description { get; private set; }

        public Operation(MainModel model,string newDescription)
        {
            Description = newDescription;
            mainModel = model;
        }

        /// <summary>
        /// Метод вызываемый при клике на кнопку
        /// </summary>
        public virtual void Select(object sender, RoutedEventArgs e)
        {

        }
    }
}
