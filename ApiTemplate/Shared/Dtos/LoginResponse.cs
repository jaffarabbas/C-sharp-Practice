using DBLayer.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public DateTime? LoginDate { get; set; }
        public List<string>? Roles { get; set; }
    }
}
