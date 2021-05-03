using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WebCodeFirstApproch.Models
{
    public class StudentContext: DbContext
    {
        public DbSet<Student> Students { get; set; }
    }
}