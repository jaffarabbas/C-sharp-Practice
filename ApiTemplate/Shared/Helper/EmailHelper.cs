using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Shared.Dtos;

namespace Shared.Helper
{
    /// <summary>
    /// Generic email helper class with mandatory functionalities.
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="emailSettings">Email configuration settings</param>
        /// <param name="emailRequest">Email request details</param>
        /// <returns>True if email sent successfully, false otherwise</returns>
        public static async Task<bool> SendEmailAsync(EmailSettings emailSettings, EmailRequest emailRequest)
        {
            try
            {
                ValidateEmailSettings(emailSettings);
                ValidateEmailRequest(emailRequest);

                using var smtpClient = CreateSmtpClient(emailSettings);
                using var mailMessage = CreateMailMessage(emailSettings, emailRequest);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sends an email synchronously.
        /// </summary>
        /// <param name="emailSettings">Email configuration settings</param>
        /// <param name="emailRequest">Email request details</param>
        /// <returns>True if email sent successfully, false otherwise</returns>
        public static bool SendEmail(EmailSettings emailSettings, EmailRequest emailRequest)
        {
            try
            {
                ValidateEmailSettings(emailSettings);
                ValidateEmailRequest(emailRequest);

                using var smtpClient = CreateSmtpClient(emailSettings);
                using var mailMessage = CreateMailMessage(emailSettings, emailRequest);

                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Validates email address format.
        /// </summary>
        /// <param name="email">Email address to validate</param>
        /// <returns>True if valid email format, false otherwise</returns>
        public static bool IsValidEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates an HTML email template for password reset.
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <param name="resetToken">Password reset token</param>
        /// <param name="resetUrl">Reset URL (optional)</param>
        /// <returns>HTML email content</returns>
        public static string CreatePasswordResetEmailTemplate(string userName, string resetToken, string resetUrl = "")
        {
            var template = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 20px; border-radius: 5px; }}
        .header {{ background-color: #007bff; color: white; padding: 15px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ padding: 20px; }}
        .token-box {{ background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 15px; margin: 15px 0; border-radius: 5px; }}
        .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #dee2e6; font-size: 12px; color: #6c757d; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Password Reset Request</h2>
        </div>
        <div class='content'>
            <p>Hello {userName},</p>
            <p>You have requested to reset your password. Please use the following reset token:</p>
            <div class='token-box'>
                <strong>Reset Token:</strong> {resetToken}
            </div>";

            if (!string.IsNullOrEmpty(resetUrl))
            {
                template += $@"
            <p>Or click the link below to reset your password:</p>
            <p><a href='{resetUrl}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>";
            }

            template += @"
            <p>This token will expire in 1 hour for security reasons.</p>
            <p>If you did not request this password reset, please ignore this email.</p>
        </div>
        <div class='footer'>
            <p>This is an automated message. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";

            return template;
        }

        /// <summary>
        /// Creates an HTML email template for JWT reset token.
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <param name="resetToken">JWT reset token</param>
        /// <returns>HTML email content</returns>
        public static string CreateJwtResetEmailTemplate(string userName, string resetToken)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 20px; border-radius: 5px; }}
        .header {{ background-color: #28a745; color: white; padding: 15px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ padding: 20px; }}
        .token-box {{ background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 15px; margin: 15px 0; border-radius: 5px; }}
        .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #dee2e6; font-size: 12px; color: #6c757d; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>JWT Token Reset</h2>
        </div>
        <div class='content'>
            <p>Hello {userName},</p>
            <p>You have requested a JWT token reset. Please use the following reset token:</p>
            <div class='token-box'>
                <strong>Reset Token:</strong> {resetToken}
            </div>
            <p>This token will expire in 15 minutes for security reasons.</p>
            <p>Use this token with the refresh-token endpoint to get a new JWT token.</p>
        </div>
        <div class='footer'>
            <p>This is an automated message. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Creates a simple text email template.
        /// </summary>
        /// <param name="title">Email title</param>
        /// <param name="message">Email message</param>
        /// <param name="userName">User's name</param>
        /// <returns>Text email content</returns>
        public static string CreateSimpleEmailTemplate(string title, string message, string userName = "")
        {
            var greeting = string.IsNullOrEmpty(userName) ? "Hello," : $"Hello {userName},";
            return $@"{greeting}

{title}

{message}

Best regards,
System Administrator

---
This is an automated message. Please do not reply to this email.";
        }

        #region Private Methods

        private static void ValidateEmailSettings(EmailSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(settings.SmtpHost))
                throw new ArgumentException("SMTP Host is required", nameof(settings.SmtpHost));

            if (settings.SmtpPort <= 0)
                throw new ArgumentException("SMTP Port must be greater than 0", nameof(settings.SmtpPort));

            if (string.IsNullOrWhiteSpace(settings.FromEmail))
                throw new ArgumentException("From Email is required", nameof(settings.FromEmail));

            if (!IsValidEmail(settings.FromEmail))
                throw new ArgumentException("From Email is not valid", nameof(settings.FromEmail));
        }

        private static void ValidateEmailRequest(EmailRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.ToEmails == null || request.ToEmails.Count == 0)
                throw new ArgumentException("At least one recipient email is required", nameof(request.ToEmails));

            foreach (var email in request.ToEmails)
            {
                if (!IsValidEmail(email))
                    throw new ArgumentException($"Invalid email address: {email}");
            }

            if (string.IsNullOrWhiteSpace(request.Subject))
                throw new ArgumentException("Email subject is required", nameof(request.Subject));

            if (string.IsNullOrWhiteSpace(request.Body))
                throw new ArgumentException("Email body is required", nameof(request.Body));
        }

        private static SmtpClient CreateSmtpClient(EmailSettings settings)
        {
            var smtpClient = new SmtpClient(settings.SmtpHost, settings.SmtpPort)
            {
                EnableSsl = settings.EnableSsl,
                Timeout = settings.TimeoutSeconds * 1000
            };

            if (!string.IsNullOrWhiteSpace(settings.Username) && !string.IsNullOrWhiteSpace(settings.Password))
            {
                smtpClient.Credentials = new NetworkCredential(settings.Username, settings.Password);
            }

            return smtpClient;
        }

        private static MailMessage CreateMailMessage(EmailSettings settings, EmailRequest request)
        {
            var fromName = string.IsNullOrWhiteSpace(settings.FromName) ? settings.FromEmail : settings.FromName;
            var mailMessage = new MailMessage
            {
                From = new MailAddress(settings.FromEmail, fromName),
                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = request.IsBodyHtml
            };

            // Add recipients
            foreach (var email in request.ToEmails)
            {
                mailMessage.To.Add(email);
            }

            // Add CC recipients
            foreach (var email in request.CcEmails)
            {
                mailMessage.CC.Add(email);
            }

            // Add BCC recipients
            foreach (var email in request.BccEmails)
            {
                mailMessage.Bcc.Add(email);
            }

            // Add attachments
            foreach (var attachment in request.Attachments)
            {
                var stream = new MemoryStream(attachment.Content);
                var mailAttachment = new Attachment(stream, attachment.FileName, attachment.ContentType);
                mailMessage.Attachments.Add(mailAttachment);
            }

            return mailMessage;
        }

        #endregion
    }
}