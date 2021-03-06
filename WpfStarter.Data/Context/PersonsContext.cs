using System;
using System.Data.Entity;
using System.Linq;

namespace WpfStarter.Data
{
    public partial class PersonsContext : DbContext
    {
        public PersonsContext()
            : base("PersonsContext")
        {
        }

        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .Property(e => e.FirstName)
                .IsFixedLength();

            modelBuilder.Entity<Person>()
                .Property(e => e.LastName)
                .IsFixedLength();

            modelBuilder.Entity<Person>()
                .Property(e => e.SurName)
                .IsFixedLength();

            modelBuilder.Entity<Person>()
                .Property(e => e.City)
                .IsFixedLength();

            modelBuilder.Entity<Person>()
                .Property(e => e.Country)
                .IsFixedLength();
        }
    }
}
