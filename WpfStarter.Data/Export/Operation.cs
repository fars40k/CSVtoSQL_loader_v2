using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.Data.Export
{
    public class Operation : IDatabaseAction
    {
        public string Description { get; private set; }

        public Operation(string newDescription)
        {
            Description = newDescription;
        }
    }
}
