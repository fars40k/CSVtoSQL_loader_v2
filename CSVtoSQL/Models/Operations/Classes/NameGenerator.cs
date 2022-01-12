using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.UI.Models.Operations
{

    internal class NameGenerator : INameGeneratior
    {
        public NameGenerator()
        {

        }

        /// <summary>
        /// Returns unique name based on current date and time
        /// </summary>
        public string GenerateName()
        {
            return String.Join("",DateTime.Now.ToString().Split('/', ':', ' ', 'A', 'P', 'M'));
        }
    }
}
