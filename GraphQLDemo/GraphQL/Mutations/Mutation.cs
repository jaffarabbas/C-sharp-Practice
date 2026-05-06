using GraphQLDemo.Data;
using GraphQLDemo.Models;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.GraphQL.Mutations;

/// <summary>
/// Main Mutation type - demonstrates all GraphQL Mutation concepts
/// Mutations are used for write operations (create, update, delete)
/// </summary>
public class Mutation
{
    // CONCEPT 1: Create mutation with input type
    /// <summary>
    /// Add a new book - demonstrates CREATE operations
    /// Example:
    /// mutation {
    ///   addBook(input: {
    ///     title: "New Book"
    ///     description: "A great book"
    ///     pages: 300
    ///     price: 25.99
    ///     genre: FICTION
    ///     authorId: 1
    ///   }) {
    ///     id
    ///     title
    ///   }
    /// }
    /// </summary>
    public async Task<Book> AddBookAsync(
        AddBookInput input,
        AppDbContext context,
        [Service] ITopicEventSender eventSender)
    {
        var book = new Book
        {
            Title = input.Title,
            Description = input.Description,
            Pages = input.Pages,
            Price = input.Price,
            PublishedDate = input.PublishedDate ?? DateTime.UtcNow,
            Genre = input.Genre,
            AuthorId = input.AuthorId
        };

        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Trigger subscription event
        await eventSender.SendAsync(nameof(Subscription.OnBookAdded), book);

        return book;
    }

    // CONCEPT 2: Update mutation
    /// <summary>
    /// Update an existing book - demonstrates UPDATE operations
    /// Example:
    /// mutation {
    ///   updateBook(id: 1, input: {
    ///     title: "Updated Title"
    ///     price: 29.99
    ///   }) {
    ///     id
    ///     title
    ///     price
    ///   }
    /// }
    /// </summary>
    public async Task<Book> UpdateBookAsync(
        int id,
        UpdateBookInput input,
        AppDbContext context)
    {
        var book = await context.Books.FindAsync(id);
        if (book == null)
            throw new GraphQLException($"Book with ID {id} not found");

        if (!string.IsNullOrEmpty(input.Title))
            book.Title = input.Title;

        if (!string.IsNullOrEmpty(input.Description))
            book.Description = input.Description;

        if (input.Pages.HasValue)
            book.Pages = input.Pages.Value;

        if (input.Price.HasValue)
            book.Price = input.Price.Value;

        if (input.Genre.HasValue)
            book.Genre = input.Genre.Value;

        await context.SaveChangesAsync();
        return book;
    }

    // CONCEPT 3: Delete mutation
    /// <summary>
    /// Delete a book - demonstrates DELETE operations
    /// Example:
    /// mutation {
    ///   deleteBook(id: 1)
    /// }
    /// </summary>
    public async Task<bool> DeleteBookAsync(
        int id,
        AppDbContext context)
    {
        var book = await context.Books.FindAsync(id);
        if (book == null)
            return false;

        context.Books.Remove(book);
        await context.SaveChangesAsync();
        return true;
    }

    // CONCEPT 4: Create mutation returning payload
    /// <summary>
    /// Add a new author - demonstrates mutation with custom payload
    /// Example:
    /// mutation {
    ///   addAuthor(input: {
    ///     name: "J.R.R. Tolkien"
    ///     email: "tolkien@example.com"
    ///     birthDate: "1892-01-03"
    ///     country: "United Kingdom"
    ///   }) {
    ///     author { id name }
    ///     success
    ///     message
    ///   }
    /// }
    /// </summary>
    public async Task<AddAuthorPayload> AddAuthorAsync(
        AddAuthorInput input,
        AppDbContext context)
    {
        var author = new Author
        {
            Name = input.Name,
            Email = input.Email,
            BirthDate = input.BirthDate,
            Country = input.Country
        };

        context.Authors.Add(author);
        await context.SaveChangesAsync();

        return new AddAuthorPayload
        {
            Author = author,
            Success = true,
            Message = "Author created successfully"
        };
    }

    // CONCEPT 5: Add review mutation with validation
    /// <summary>
    /// Add a review to a book - demonstrates mutations with validation
    /// Example:
    /// mutation {
    ///   addReview(input: {
    ///     bookId: 1
    ///     reviewerName: "Jane"
    ///     rating: 5
    ///     comment: "Excellent!"
    ///   }) {
    ///     id
    ///     rating
    ///     comment
    ///   }
    /// }
    /// </summary>
    public async Task<Review> AddReviewAsync(
        AddReviewInput input,
        AppDbContext context,
        [Service] ITopicEventSender eventSender)
    {
        // Validation
        if (input.Rating < 1 || input.Rating > 5)
            throw new GraphQLException("Rating must be between 1 and 5");

        var book = await context.Books.FindAsync(input.BookId);
        if (book == null)
            throw new GraphQLException($"Book with ID {input.BookId} not found");

        var review = new Review
        {
            BookId = input.BookId,
            ReviewerName = input.ReviewerName,
            Rating = input.Rating,
            Comment = input.Comment,
            CreatedAt = DateTime.UtcNow
        };

        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        // Trigger subscription event
        await eventSender.SendAsync(nameof(Subscription.OnReviewAdded), review);

        return review;
    }

    // CONCEPT 6: Batch mutation
    /// <summary>
    /// Delete multiple reviews - demonstrates batch operations
    /// Example:
    /// mutation {
    ///   deleteReviews(ids: [1, 2, 3])
    /// }
    /// </summary>
    public async Task<int> DeleteReviewsAsync(
        List<int> ids,
        AppDbContext context)
    {
        var reviews = context.Reviews.Where(r => ids.Contains(r.Id));
        var count = await reviews.CountAsync();

        context.Reviews.RemoveRange(reviews);
        await context.SaveChangesAsync();

        return count;
    }
}

// Input Types - demonstrates GraphQL Input Types

public record AddBookInput(
    string Title,
    string Description,
    int Pages,
    decimal Price,
    BookGenre Genre,
    int AuthorId,
    DateTime? PublishedDate = null
);

public record UpdateBookInput(
    string? Title = null,
    string? Description = null,
    int? Pages = null,
    decimal? Price = null,
    BookGenre? Genre = null
);

public record AddAuthorInput(
    string Name,
    string Email,
    DateTime BirthDate,
    string Country
);

public record AddReviewInput(
    int BookId,
    string ReviewerName,
    int Rating,
    string Comment
);

// Payload Types - demonstrates custom response types

public class AddAuthorPayload
{
    public Author Author { get; set; } = null!;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
