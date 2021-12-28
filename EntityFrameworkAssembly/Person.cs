namespace EntityAssembly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Persons")]
    public partial class Person
    {
        [Key]
        [Column(Order = 0)]
        public DateTime Date { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string LastName { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string SurName { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(50)]
        public string City { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(50)]
        public string Country { get; set; }
    }
}
