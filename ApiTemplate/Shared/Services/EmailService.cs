using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Shared.Dtos;
using Shared.Helper;

namespace Shared.Services
{
    /// <summary>
    /// Email service implementation for sending emails.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="emailRequest">Email request details</param>
        /// <returns>True if email sent successfully, false otherwise</returns>
        public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
        {
            try
            {
                _logger.LogInformation("Attempting to send email to: {Recipients}", string.Join(", ", emailRequest.ToEmails));
                _logger.LogInformation("SMTP Configuration - Host: {Host}, Port: {Port}, EnableSsl: {EnableSsl}",
                    _emailSettings.SmtpHost, _emailSettings.SmtpPort, _emailSettings.EnableSsl);

                var result = await EmailHelper.SendEmailAsync(_emailSettings, emailRequest);

                if (result)
                {
                    _logger.LogInformation("Email sent successfully to: {Recipients}", string.Join(", ", emailRequest.ToEmails));
                }
                else
                {
                    _logger.LogWarning("Email sending failed to: {Recipients}", string.Join(", ", emailRequest.ToEmails));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while sending email to: {Recipients}", string.Join(", ", emailRequest.ToEmails));
                return false;
            }
        }

        /// <summary>
        /// Sends an email synchronously.
        /// </summary>
        /// <param name="emailRequest">Email request details</param>
        /// <returns>True if email sent successfully, false otherwise</returns>
        public bool SendEmail(EmailRequest emailRequest)
        {
            try
            {
                return EmailHelper.SendEmail(_emailSettings, emailRequest);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sends password reset email.
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="userName">User's name</param>
        /// <param name="resetToken">Password reset token</param>
        /// <param name="resetUrl">Reset URL (optional)</param>
        /// <returns>True if email sent successfully, false otherwise</returns>
        public async Task<bool> SendPasswordResetEmailAsync(string email, string userName, string resetToken, string resetUrl = "")
        {
            try
            {
                var emailBody = EmailHelper.CreatePasswordResetEmailTemplate(userName, resetToken, resetUrl);

                var emailRequest = new EmailRequest
                {
                    ToEmails = { email },
                    Subject = "Password Reset Request",
                    Body = emailBody,
                    IsBodyHtml = true
                };

                return await SendEmailAsync(emailRequest);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sends JWT reset token email.
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="userName">User's name</param>
        /// <param name="resetToken">JWT reset token</param>
        /// <returns>True if email sent successfully, false otherwise</returns>
        public async Task<bool> SendJwtResetTokenEmailAsync(string email, string userName, string resetToken)
        {
            try
            {
                var emailBody = EmailHelper.CreateJwtResetEmailTemplate(userName, resetToken);

                var emailRequest = new EmailRequest
                {
                    ToEmails = { email },
                    Subject = "JWT Token Reset",
                    Body = emailBody,
                    IsBodyHtml = true
                };

                return await SendEmailAsync(emailRequest);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates email address format.
        /// </summary>
        /// <param name="email">Email address to validate</param>
        /// <returns>True if valid email format, false otherwise</returns>
        public bool IsValidEmail(string email)
        {
            return EmailHelper.IsValidEmail(email);
        }
    }
}