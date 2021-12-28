using CSVtoSQL.Models.Operations.Converters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoSQL.Models
{
    /// <summary>
    /// Класс генератор имен
    /// </summary>
    internal class NameGenerator : INameGeneratior
    {
        public NameGenerator()
        {

        }

        /// <summary>
        /// Создает уникальное имя на основе даты и времени
        /// </summary>
        /// <returns></returns>
        public string GenerateName()
        {
            return String.Join("",DateTime.Now.ToString().Split('/', ':', ' ', 'A', 'P', 'M'));
        }
    }
}
