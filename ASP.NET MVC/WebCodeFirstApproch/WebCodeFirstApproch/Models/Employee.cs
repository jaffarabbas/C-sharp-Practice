using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebCodeFirstApproch.Models
{
    public class Employee
    {
        [Key]
        public int EmpId { get; set; }
        [DisplayName("Name")]
        public string EmpName { get; set; }
        [DisplayName("Gender")]
        public string EmpGender { get; set; }
        [DisplayName("Age")]
        public int EmpAge { get; set; }
    }
}