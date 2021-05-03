using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebCodeFirstApproch.Models
{
    public class Student
    {
        [Key]
        public int StdId { get; set; }
        [DisplayName("Name")]
        public string StdName { get; set; }
        [DisplayName("Gender")]
        public string StdGender { get; set; }
        [DisplayName("Age")]
        public int StdAge { get; set; }
    }
}