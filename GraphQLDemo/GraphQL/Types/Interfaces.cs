using GraphQLDemo.Models;
using HotChocolate.Types;

namespace GraphQLDemo.GraphQL.Types;

/// <summary>
/// Interface type - demonstrates GraphQL Interfaces
/// Interfaces define a set of fields that multiple types can implement
/// Useful for querying across different types that share common fields
/// </summary>
[InterfaceType("Reviewable")]
public interface IReviewable
{
    int Id { get; }
    string Title { get; }
    ICollection<Review> Reviews { get; }
}

/// <summary>
/// Reviewable interface type for HotChocolate
/// </summary>
public class ReviewableType : InterfaceType<IReviewable>
{
    protected override void Configure(IInterfaceTypeDescriptor<IReviewable> descriptor)
    {
        descriptor.Name("Reviewable");
        descriptor.Field(r => r.Id).Type<NonNullType<IdType>>();
        descriptor.Field(r => r.Title).Type<NonNullType<StringType>>();
        descriptor.Field(r => r.Reviews);
    }
}

/// <summary>
/// Make Book implement the Reviewable interface
/// </summary>
public class BookType : ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
    {
        descriptor.Implements<ReviewableType>();

        descriptor
            .Field(b => b.Id)
            .Type<NonNullType<IdType>>();

        descriptor
            .Field(b => b.Title)
            .Type<NonNullType<StringType>>();

        // Add computed field
        descriptor
            .Field("averageRating")
            .Type<FloatType>()
            .Resolve(context =>
            {
                var book = context.Parent<Book>();
                if (book.Reviews == null || !book.Reviews.Any())
                    return null;
                return book.Reviews.Average(r => r.Rating);
            });
    }
}

/// <summary>
/// Abstract type for different types of publications
/// Demonstrates union types - types that can be one of several object types
/// </summary>
public abstract class Publication
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
}

/// <summary>
/// Digital book type
/// </summary>
public class EBook : Publication
{
    public decimal Price { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
    public long FileSizeInBytes { get; set; }
    public string Format { get; set; } = string.Empty; // PDF, EPUB, MOBI
}

/// <summary>
/// Physical book type
/// </summary>
public class PhysicalBook : Publication
{
    public decimal Price { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public decimal ShippingWeight { get; set; }
}

/// <summary>
/// Audio book type
/// </summary>
public class AudioBook : Publication
{
    public decimal Price { get; set; }
    public string NarratorName { get; set; } = string.Empty;
    public int DurationInMinutes { get; set; }
    public string PreviewUrl { get; set; } = string.Empty;
}
