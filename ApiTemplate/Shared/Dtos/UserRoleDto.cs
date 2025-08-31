using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class UserRoleDto
    {
        public int RoleId { get; set; }
        public bool UserRoleIsActive { get; set; } = true;
    }
}
