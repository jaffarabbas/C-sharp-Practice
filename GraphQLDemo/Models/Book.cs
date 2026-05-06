namespace GraphQLDemo.Models;

/// <summary>
/// Represents a book entity - demonstrates GraphQL Object Types with relationships
/// </summary>
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Pages { get; set; }
    public decimal Price { get; set; }
    public DateTime PublishedDate { get; set; }
    public BookGenre Genre { get; set; }

    // Foreign key
    public int AuthorId { get; set; }

    // Navigation properties - demonstrates nested queries in GraphQL
    public Author Author { get; set; } = null!;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

/// <summary>
/// Enum type - demonstrates GraphQL Enum Types
/// </summary>
public enum BookGenre
{
    Fiction,
    NonFiction,
    Mystery,
    SciFi,
    Fantasy,
    Biography,
    History,
    Technology
}
