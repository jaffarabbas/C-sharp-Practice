using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCodeFirstApproch.Models
{
    public class MultiModelData
    {
        public List<Student> Students { get; set; }
        public List<Employee> Employees { get; set; }
    }
}