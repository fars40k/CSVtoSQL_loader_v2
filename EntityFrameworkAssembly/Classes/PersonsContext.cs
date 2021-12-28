using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace EntityAssembly
{
    internal partial class PersonsContext : DbContext
    {
        public PersonsContext()
            : base("name=DefaultConnection")
        {
        }

        public PersonsContext(string connStr)
            : base(connStr)
        {
        }

        public IDbSet<Person> Persons { get; set; }
    }
}
