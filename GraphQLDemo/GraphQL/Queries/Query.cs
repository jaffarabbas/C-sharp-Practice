using GraphQLDemo.Data;
using GraphQLDemo.Models;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.GraphQL.Queries;

/// <summary>
/// Main Query type - demonstrates all GraphQL Query concepts
/// This is the entry point for all read operations in GraphQL
/// </summary>
public class Query
{
    // CONCEPT 1: Basic Query - Simple field resolution
    /// <summary>
    /// Simple query that returns a string
    /// Example: { hello }
    /// </summary>
    public string Hello() => "Hello, GraphQL!";

    // CONCEPT 2: Query with arguments
    /// <summary>
    /// Query with arguments - demonstrates parameterized queries
    /// Example: { greet(name: "John") }
    /// </summary>
    public string Greet(string name) => $"Hello, {name}!";

    // CONCEPT 3: Querying collections with filtering, sorting, and pagination
    /// <summary>
    /// Get all books with HotChocolate filtering, sorting, and pagination
    /// Example:
    /// {
    ///   books(where: { genre: { eq: FANTASY } }, order: { title: ASC }) {
    ///     id
    ///     title
    ///     price
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Book> GetBooks(AppDbContext context)
    {
        return context.Books;
    }

    // CONCEPT 4: Querying single entity by ID
    /// <summary>
    /// Get a book by ID - demonstrates single entity queries
    /// Example: { book(id: 1) { title author { name } } }
    /// </summary>
    [UseProjection]
    public async Task<Book?> GetBookAsync(int id, AppDbContext context)
    {
        return await context.Books
            .Include(b => b.Author)
            .Include(b => b.Reviews)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    // CONCEPT 5: Querying with relationships
    /// <summary>
    /// Get all authors with their books - demonstrates nested queries
    /// Example:
    /// {
    ///   authors {
    ///     name
    ///     books {
    ///       title
    ///       reviews { rating }
    ///     }
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Author> GetAuthors(AppDbContext context)
    {
        return context.Authors;
    }

    // CONCEPT 6: Query with custom filtering logic
    /// <summary>
    /// Get books by genre - demonstrates custom query methods
    /// Example: { booksByGenre(genre: SCIFI) { title } }
    /// </summary>
    public async Task<List<Book>> GetBooksByGenreAsync(
        BookGenre genre,
        AppDbContext context)
    {
        return await context.Books
            .Where(b => b.Genre == genre)
            .Include(b => b.Author)
            .ToListAsync();
    }

    // CONCEPT 7: Computed fields
    /// <summary>
    /// Get book statistics - demonstrates computed/aggregate queries
    /// Example: { bookStats { totalBooks averagePrice } }
    /// </summary>
    public async Task<BookStats> GetBookStatsAsync(AppDbContext context)
    {
        var books = await context.Books.ToListAsync();
        return new BookStats
        {
            TotalBooks = books.Count,
            AveragePrice = books.Any() ? books.Average(b => b.Price) : 0,
            TotalPages = books.Sum(b => b.Pages),
            MostExpensiveBook = books.OrderByDescending(b => b.Price).FirstOrDefault()?.Title ?? "N/A"
        };
    }

    // CONCEPT 8: Get reviews with filtering
    /// <summary>
    /// Get all reviews - demonstrates filtering on related entities
    /// Example: { reviews(where: { rating: { gte: 4 } }) { comment rating } }
    /// </summary>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Review> GetReviews(AppDbContext context)
    {
        return context.Reviews;
    }

    // CONCEPT 9: Search functionality
    /// <summary>
    /// Search books by title or description
    /// Example: { searchBooks(searchTerm: "Potter") { title description } }
    /// </summary>
    public async Task<List<Book>> SearchBooksAsync(
        string searchTerm,
        AppDbContext context)
    {
        return await context.Books
            .Where(b => b.Title.Contains(searchTerm) || b.Description.Contains(searchTerm))
            .Include(b => b.Author)
            .ToListAsync();
    }
}

/// <summary>
/// Custom output type for statistics - demonstrates complex return types
/// </summary>
public class BookStats
{
    public int TotalBooks { get; set; }
    public decimal AveragePrice { get; set; }
    public int TotalPages { get; set; }
    public string MostExpensiveBook { get; set; } = string.Empty;
}
