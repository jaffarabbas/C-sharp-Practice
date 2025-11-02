using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oop.SOLID
{
    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class  EmailService 
    {
        public void SendEmail(string to, string subject, string body)
        {
            // Code to send email
            Console.WriteLine($"Email sent to {to} with subject {subject}");
        }
    }

    public class ReportGenerator
    {
        public string GenerateReport(User user)
        {
            // Code to generate report
            return $"Report for {user.Name}";
        }
    }
    public class SingleResponsiblity
    {
        public void Execute()
        {
            User user = new User { Name = "John Doe", Email = "sadas@asdsa.xcom", Password = "password123" };
            ReportGenerator reportGenerator = new ReportGenerator();
            string report = reportGenerator.GenerateReport(user);
            EmailService emailService = new EmailService();
            emailService.SendEmail(user.Email, "Your Report", report);
        }
    }
}
