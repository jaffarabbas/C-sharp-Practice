using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AptechWorkshop2.Models.Database
{
    [Table("ADDRESS")]
    public partial class Address
    {
        public Address()
        {
            Employees = new HashSet<Employee>();
        }

        [Key]
        [Column("AD_ID")]
        public int AdId { get; set; }
        [Column("ADD_DETIALS")]
        [StringLength(50)]
        [Unicode(false)]
        public string? AddDetials { get; set; }
        [Column("ADD_STATE")]
        [StringLength(40)]
        [Unicode(false)]
        public string? AddState { get; set; }
        [Column("ADD_COUNTRY")]
        [StringLength(40)]
        [Unicode(false)]
        public string? AddCountry { get; set; }

        [InverseProperty(nameof(Employee.EmpAddress))]
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
