using HotChocolate.Types;

namespace GraphQLDemo.GraphQL.Types;

/// <summary>
/// Union type - demonstrates GraphQL Unions
/// A union type represents an object that could be one of several types
/// Unlike interfaces, union types don't specify common fields
///
/// Example query:
/// {
///   searchPublications(searchTerm: "Harry") {
///     ... on EBook {
///       title
///       downloadUrl
///       format
///     }
///     ... on PhysicalBook {
///       title
///       isbn
///       stockQuantity
///     }
///     ... on AudioBook {
///       title
///       narratorName
///       durationInMinutes
///     }
///   }
/// }
/// </summary>
[UnionType("PublicationResult")]
public abstract class PublicationResultBase
{
    public static PublicationResultBase Create(object value)
    {
        return value switch
        {
            EBook ebook => new EBookResult { EBook = ebook },
            PhysicalBook physical => new PhysicalBookResult { PhysicalBook = physical },
            AudioBook audio => new AudioBookResult { AudioBook = audio },
            _ => throw new NotSupportedException()
        };
    }
}

public class EBookResult : PublicationResultBase
{
    public EBook EBook { get; set; } = null!;
}

public class PhysicalBookResult : PublicationResultBase
{
    public PhysicalBook PhysicalBook { get; set; } = null!;
}

public class AudioBookResult : PublicationResultBase
{
    public AudioBook AudioBook { get; set; } = null!;
}

/// <summary>
/// Search result union - demonstrates another union use case
/// Search can return either Books or Authors
/// </summary>
[UnionType("SearchResult")]
public abstract class SearchResultBase
{
}
