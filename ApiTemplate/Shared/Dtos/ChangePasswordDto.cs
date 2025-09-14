using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    /// <summary>
    /// DTO for changing password.
    /// </summary>
    public class ChangePasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
    }
}
