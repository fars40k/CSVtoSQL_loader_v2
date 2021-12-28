using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoSQL.Models.Operations
{
    internal class PersonsContext : System.Data.Entity.DbContext
    {
        public PersonsContext(string newConnString) : base(newConnString)
        {

        }

        public IDbSet<Person> Persons { get; set; }
    }
}
