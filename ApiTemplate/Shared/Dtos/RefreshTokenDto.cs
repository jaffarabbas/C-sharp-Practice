using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
    /// <summary>
    /// DTO for refreshing JWT token using reset token.
    /// </summary>
    public class RefreshTokenDto
    {
        [Required]
        public string ResetToken { get; set; } = string.Empty;
    }
}