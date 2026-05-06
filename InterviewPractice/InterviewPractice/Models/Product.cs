namespace InterviewPractice.Models;

// DB-first: run this to regenerate from your actual test2 schema:
// dotnet ef dbcontext scaffold "Server=localhost,1433;Database=test2;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models --force --no-onconfiguring
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
