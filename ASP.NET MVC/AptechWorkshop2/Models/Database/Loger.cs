using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AptechWorkshop2.Models.Database
{
    [Keyless]
    [Table("loger")]
    public partial class Loger
    {
        [Column("id")]
        public int Id { get; set; }
        [Unicode(false)]
        public string? Loggerfile { get; set; }
    }
}
