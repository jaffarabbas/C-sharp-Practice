using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
    /// <summary>
    /// DTO for testing email functionality.
    /// </summary>
    public class TestEmailDto
    {
        [Required]
        [EmailAddress]
        public string ToEmail { get; set; } = string.Empty;
    }
}