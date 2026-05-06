namespace GraphQLDemo.Models;

/// <summary>
/// Represents an author entity - demonstrates GraphQL Object Types
/// </summary>
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Country { get; set; } = string.Empty;

    // Navigation property - demonstrates relationships in GraphQL
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
