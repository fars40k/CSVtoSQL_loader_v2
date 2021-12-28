using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoSQL.Models.Operations
{
    internal class DBOperation : Operation
    {
        public DBOperation(MainModel model, string newDescription) : base(model, newDescription)
        {

        }
    }
}
