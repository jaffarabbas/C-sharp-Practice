namespace GraphQLDemo.Models;

/// <summary>
/// Represents a review entity - demonstrates GraphQL Object Types
/// </summary>
public class Review
{
    public int Id { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5 stars
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Foreign key
    public int BookId { get; set; }

    // Navigation property
    public Book Book { get; set; } = null!;
}
