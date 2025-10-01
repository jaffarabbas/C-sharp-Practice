using System.Threading.Tasks;
using Shared.Dtos;

namespace Shared.Services
{
    /// <summary>
    /// Email service interface for sending emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="emailRequest">Email request details</param>
        /// <returns>True if email sent successfully, false otherwise</returns>
        Task<bool> SendEmailAsync(EmailRequest emailRequest);

        /// <summary>
        /// Sends an email synchronously.
        /// </summary>
        /// <param name="emailRequest">Email request details</param>
        /// <returns>True if email sent successfully, false otherwise</returns>
        bool SendEmail(EmailRequest emailRequest);

        /// <summary>
        /// Sends password reset email.
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="userName">User's name</param>
        /// <param name="resetToken">Password reset token</param>
        /// <param name="resetUrl">Reset URL (optional)</param>
        /// <returns>True if email sent successfully, false otherwise</returns>
        Task<bool> SendPasswordResetEmailAsync(string email, string userName, string resetToken, string resetUrl = "");

        /// <summary>
        /// Sends JWT reset token email.
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="userName">User's name</param>
        /// <param name="resetToken">JWT reset token</param>
        /// <returns>True if email sent successfully, false otherwise</returns>
        Task<bool> SendJwtResetTokenEmailAsync(string email, string userName, string resetToken);

        /// <summary>
        /// Validates email address format.
        /// </summary>
        /// <param name="email">Email address to validate</param>
        /// <returns>True if valid email format, false otherwise</returns>
        bool IsValidEmail(string email);
    }
}