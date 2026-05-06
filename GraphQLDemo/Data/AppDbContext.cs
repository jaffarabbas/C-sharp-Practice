using Microsoft.EntityFrameworkCore;
using GraphQLDemo.Models;

namespace GraphQLDemo.Data;

/// <summary>
/// Database context for the GraphQL demo application
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId);

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Authors
        modelBuilder.Entity<Author>().HasData(
            new Author
            {
                Id = 1,
                Name = "J.K. Rowling",
                Email = "jk@example.com",
                BirthDate = new DateTime(1965, 7, 31),
                Country = "United Kingdom"
            },
            new Author
            {
                Id = 2,
                Name = "George Orwell",
                Email = "george@example.com",
                BirthDate = new DateTime(1903, 6, 25),
                Country = "United Kingdom"
            },
            new Author
            {
                Id = 3,
                Name = "Isaac Asimov",
                Email = "isaac@example.com",
                BirthDate = new DateTime(1920, 1, 2),
                Country = "United States"
            }
        );

        // Seed Books
        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Title = "Harry Potter and the Philosopher's Stone",
                Description = "A young wizard's journey begins",
                Pages = 223,
                Price = 19.99m,
                PublishedDate = new DateTime(1997, 6, 26),
                Genre = BookGenre.Fantasy,
                AuthorId = 1
            },
            new Book
            {
                Id = 2,
                Title = "Harry Potter and the Chamber of Secrets",
                Description = "The second year at Hogwarts",
                Pages = 251,
                Price = 21.99m,
                PublishedDate = new DateTime(1998, 7, 2),
                Genre = BookGenre.Fantasy,
                AuthorId = 1
            },
            new Book
            {
                Id = 3,
                Title = "1984",
                Description = "A dystopian social science fiction novel",
                Pages = 328,
                Price = 15.99m,
                PublishedDate = new DateTime(1949, 6, 8),
                Genre = BookGenre.Fiction,
                AuthorId = 2
            },
            new Book
            {
                Id = 4,
                Title = "Animal Farm",
                Description = "A satirical allegorical novella",
                Pages = 112,
                Price = 12.99m,
                PublishedDate = new DateTime(1945, 8, 17),
                Genre = BookGenre.Fiction,
                AuthorId = 2
            },
            new Book
            {
                Id = 5,
                Title = "Foundation",
                Description = "The first book in the Foundation series",
                Pages = 255,
                Price = 18.99m,
                PublishedDate = new DateTime(1951, 6, 1),
                Genre = BookGenre.SciFi,
                AuthorId = 3
            }
        );

        // Seed Reviews
        modelBuilder.Entity<Review>().HasData(
            new Review
            {
                Id = 1,
                ReviewerName = "John Doe",
                Rating = 5,
                Comment = "Absolutely magical! A perfect start to an amazing series.",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                BookId = 1
            },
            new Review
            {
                Id = 2,
                ReviewerName = "Jane Smith",
                Rating = 5,
                Comment = "My childhood favorite. Still love it!",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                BookId = 1
            },
            new Review
            {
                Id = 3,
                ReviewerName = "Bob Johnson",
                Rating = 4,
                Comment = "Great sequel, even better than the first!",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                BookId = 2
            },
            new Review
            {
                Id = 4,
                ReviewerName = "Alice Brown",
                Rating = 5,
                Comment = "A chilling masterpiece that remains relevant today.",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                BookId = 3
            },
            new Review
            {
                Id = 5,
                ReviewerName = "Charlie Wilson",
                Rating = 4,
                Comment = "Short but powerful. A must-read.",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                BookId = 4
            }
        );
    }
}
