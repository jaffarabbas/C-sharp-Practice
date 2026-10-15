namespace DependancyInjectionProject.Repository;

public interface IEmailRepo
{
    Task<string> SendEmailAsync(string to, string subject, string body);
}

public class EmailRepo : IEmailRepo
{
    public async Task<string> SendEmailAsync(string to, string subject, string body)
    {
        // Simulate sending an email (for demonstration purposes)
        await Task.Delay(1000); // Simulate some delay
        return $"Email sent to: {to}, Subject: {subject}, Body: {body}";
    }
}