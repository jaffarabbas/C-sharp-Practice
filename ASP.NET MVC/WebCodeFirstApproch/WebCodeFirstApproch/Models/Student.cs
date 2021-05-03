using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebCodeFirstApproch.Models
{
    public class Student
    {
        [Key]
        public int StdId { get; set; }
        public string StdName { get; set; }
        public string StdGender { get; set; }
        public int StdAge { get; set; }
    }
}