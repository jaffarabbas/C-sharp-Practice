using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AptechWorkshop2.Models.Database
{
    [Table("EMPLOYEE")]
    [Index(nameof(EmpEmail), Name = "UQ__EMPLOYEE__19388119B1EC796D", IsUnique = true)]
    public partial class Employee
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("EMP_NAME")]
        [StringLength(20)]
        [Unicode(false)]
        public string EmpName { get; set; } = null!;
        [Column("EMP_EMAIL")]
        [StringLength(40)]
        [Unicode(false)]
        public string EmpEmail { get; set; } = null!;
        [Column("EMP_ADDRESS_ID")]
        public int EmpAddressId { get; set; }
        [Column("EMP_CODE")]
        [StringLength(50)]
        [Unicode(false)]
        public string EmpCode { get; set; } = null!;

        [ForeignKey(nameof(EmpAddressId))]
        [InverseProperty(nameof(Address.Employees))]
        public virtual Address EmpAddress { get; set; } = null!;
    }
}
