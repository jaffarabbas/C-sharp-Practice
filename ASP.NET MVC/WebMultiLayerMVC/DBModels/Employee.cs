using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBModels
{
    public class Employee
    {
        public int ID { get; set; }
        [Required]
        public string EMP_NAME { get; set; }
        [Required]
        [EmailAddress]
        public string EMP_EMAIL { get; set; }
        public int EMP_ADDRESS_ID { get; set; }
        [Required]
        public string EMP_CODE { get; set; }

        public Address Address { get; set; }
    }
}
