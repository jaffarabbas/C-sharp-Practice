using System.Collections.Generic;

namespace Shared.Dtos
{
    /// <summary>
    /// Email request model for sending emails.
    /// </summary>
    public class EmailRequest
    {
        public List<string> ToEmails { get; set; } = new List<string>();
        public List<string> CcEmails { get; set; } = new List<string>();
        public List<string> BccEmails { get; set; } = new List<string>();
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsBodyHtml { get; set; } = true;
        public List<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
    }

    /// <summary>
    /// Email attachment model.
    /// </summary>
    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = new byte[0];
        public string ContentType { get; set; } = "application/octet-stream";
    }
}