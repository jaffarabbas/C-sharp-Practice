# GraphQL Main Concepts - Complete Implementation

This project demonstrates **ALL main GraphQL concepts** using ASP.NET Core and HotChocolate v15.

## What's Implemented

### 1. Queries (Read Operations)
- **Location**: [GraphQL/Queries/Query.cs](GraphQL/Queries/Query.cs)
- **Basic queries**: Simple string returns
- **Parameterized queries**: Queries with arguments
- **Object queries**: Returning complex types
- **Collection queries**: Returning lists
- **Nested queries**: Related data (Books with Authors, Authors with Books)
- **Filtering**: `where` clauses for complex filtering
- **Sorting**: `order` clauses for data ordering
- **Pagination**: `first`, `skip` for pagination
- **Projections**: Select only needed fields
- **Search**: Full-text search functionality
- **Aggregate queries**: Statistics and computed values

### 2. Mutations (Write Operations)
- **Location**: [GraphQL/Mutations/Mutation.cs](GraphQL/Mutations/Mutation.cs)
- **Create**: Add new entities
- **Update**: Modify existing entities
- **Delete**: Remove entities
- **Batch operations**: Multiple operations at once
- **Custom payloads**: Structured responses with status
- **Validation**: Input validation with error handling

### 3. Subscriptions (Real-time Updates)
- **Location**: [GraphQL/Subscriptions/Subscription.cs](GraphQL/Subscriptions/Subscription.cs)
- **Event-based**: Subscribe to mutations
- **Filtered**: Subscribe to specific data
- **Streaming**: Time-based data streams
- **WebSocket support**: Full real-time functionality

### 4. Type System

#### Object Types
- **Book**: Demonstrates complex types with relationships
- **Author**: Shows one-to-many relationships
- **Review**: Demonstrates many-to-one relationships

#### Scalar Types
- **Built-in**: String, Int, Float, Boolean, ID, DateTime
- **Custom**: URL, Email with validation
- **Location**: [GraphQL/Types/CustomScalars.cs](GraphQL/Types/CustomScalars.cs)

#### Enum Types
- **BookGenre**: Fiction, NonFiction, Mystery, SciFi, Fantasy, etc.
- **Location**: [Models/Book.cs](Models/Book.cs)

#### Input Types
- **AddBookInput**: For creating books
- **UpdateBookInput**: For updating books
- **AddAuthorInput**: For creating authors
- **AddReviewInput**: For creating reviews
- **Location**: [GraphQL/Mutations/Mutation.cs](GraphQL/Mutations/Mutation.cs)

#### Interface Types
- **IReviewable**: Interface for types that can be reviewed
- **BookType**: Implementation of IReviewable
- **Location**: [GraphQL/Types/Interfaces.cs](GraphQL/Types/Interfaces.cs)

#### Union Types
- **PublicationResult**: Can be EBook, PhysicalBook, or AudioBook
- **Location**: [GraphQL/Types/UnionTypes.cs](GraphQL/Types/UnionTypes.cs)

### 5. DataLoaders (N+1 Problem Solution)
- **Location**: [GraphQL/DataLoaders/](GraphQL/DataLoaders/)
- **AuthorDataLoader**: Batch load authors
- **BookDataLoader**: Batch load books
- **BooksByAuthorDataLoader**: Group load books by author
- **Solves N+1 query problem automatically**

### 6. Directives
- **Location**: [GraphQL/Directives/UpperCaseDirective.cs](GraphQL/Directives/UpperCaseDirective.cs)
- **Custom directive**: @uppercase
- **Built-in directives**: @skip, @include (available by default)

### 7. Advanced Query Features
- **Filtering**: Complex WHERE clauses with AND/OR
- **Sorting**: Multi-field sorting
- **Pagination**: Offset and cursor-based
- **Projections**: Field selection optimization

### 8. Validation
- **Location**: [GraphQL/Types/Validators.cs](GraphQL/Types/Validators.cs)
- **Input validation**: Required fields, format validation
- **Business rules**: Rating ranges, email format
- **Error handling**: Structured GraphQL errors

### 9. Schema Features
- **Auto-generated schema**: From C# types
- **Introspection**: Query the schema itself
- **Documentation**: XML comments become GraphQL descriptions
- **Null handling**: Nullable and non-nullable types

### 10. Database Integration
- **Entity Framework Core**: Full ORM integration
- **In-memory database**: For demo purposes
- **Seed data**: Pre-populated test data
- **Relationships**: One-to-many, many-to-one

## Technology Stack

- **.NET 10.0**: Latest .NET version
- **HotChocolate 15.1.11**: GraphQL server
- **Entity Framework Core 10.0**: ORM
- **In-Memory Database**: For demo simplicity

## Project Structure

```
GraphQLDemo/
├── Models/
│   ├── Author.cs              # Domain model
│   ├── Book.cs                # Domain model + BookGenre enum
│   └── Review.cs              # Domain model
│
├── Data/
│   └── AppDbContext.cs        # EF Core + seed data
│
├── GraphQL/
│   ├── Queries/
│   │   └── Query.cs           # All query operations
│   ├── Mutations/
│   │   └── Mutation.cs        # All mutation operations
│   ├── Subscriptions/
│   │   └── Subscription.cs    # Real-time subscriptions
│   ├── Types/
│   │   ├── CustomScalars.cs   # Custom URL & Email types
│   │   ├── Interfaces.cs      # IReviewable interface
│   │   ├── UnionTypes.cs      # Union type definitions
│   │   └── Validators.cs      # Input validation
│   ├── DataLoaders/
│   │   ├── AuthorDataLoader.cs
│   │   └── BookDataLoader.cs
│   └── Directives/
│       └── UpperCaseDirective.cs
│
├── Program.cs                 # Application setup
├── README.md                  # Getting started guide
├── GRAPHQL_EXAMPLES.md        # Query examples
└── CONCEPTS_SUMMARY.md        # This file
```

## Learning Path

### Beginner Level
1. **Simple Queries**: Start with `hello` and `greet`
2. **Object Queries**: Fetch books and authors
3. **Simple Mutations**: Add a book or review
4. **Field Selection**: Request only needed fields

### Intermediate Level
5. **Filtering**: Use `where` clauses
6. **Sorting**: Order results
7. **Nested Queries**: Fetch related data
8. **Pagination**: Handle large datasets
9. **Input Types**: Understand mutation inputs
10. **Error Handling**: Handle validation errors

### Advanced Level
11. **Subscriptions**: Real-time updates
12. **DataLoaders**: Understand N+1 problem
13. **Interfaces**: Abstract types
14. **Union Types**: Multiple type results
15. **Custom Scalars**: Type validation
16. **Directives**: Modify execution
17. **Introspection**: Query the schema

## Key Concepts Explained

### 1. Schema-First vs Code-First
This project uses **Code-First** approach:
- Write C# classes
- HotChocolate generates GraphQL schema
- Type-safe and refactor-friendly

### 2. Resolver Pattern
Every field can have a resolver:
```csharp
descriptor
    .Field("averageRating")
    .Resolve(context => {
        var book = context.Parent<Book>();
        return book.Reviews.Average(r => r.Rating);
    });
```

### 3. DataLoader Pattern
Solves N+1 problem by batching requests:
- Without DataLoader: 1 + N queries
- With DataLoader: 2 queries (batched)

### 4. Projection Pattern
Only queries database for requested fields:
```graphql
{ books { title } }  # Only selects Title from DB
```

### 5. Subscription Pattern
WebSocket-based real-time updates:
1. Client subscribes
2. Server sends updates when data changes
3. Connection stays open

## Running the Project

```bash
# Build
dotnet build

# Run
dotnet run

# Access GraphQL IDE
# Open: http://localhost:5000/graphql
```

## Testing Examples

See [GRAPHQL_EXAMPLES.md](GRAPHQL_EXAMPLES.md) for comprehensive examples of:
- 18+ query examples
- 8+ mutation examples
- 5+ subscription examples
- Advanced filtering, sorting, pagination
- Using directives
- Introspection queries

## GraphQL vs REST Comparison

| Feature | REST | GraphQL (This Project) |
|---------|------|------------------------|
| Endpoints | Multiple (`/api/books`, `/api/authors`) | Single (`/graphql`) |
| Over-fetching | Common (fixed responses) | Eliminated (request exactly what you need) |
| Under-fetching | Requires multiple requests | Single request with nested queries |
| Versioning | URL versioning (`/api/v1/`) | Schema evolution |
| Real-time | Polling or WebSockets | Built-in subscriptions |
| Documentation | Swagger/OpenAPI (separate) | Self-documenting (introspection) |
| Type Safety | Depends on implementation | Strong typing built-in |
| Caching | HTTP caching | Field-level caching with DataLoaders |

## Performance Optimizations Implemented

1. **DataLoaders**: Automatic request batching and caching
2. **Projections**: Only query needed fields from database
3. **Pooled DbContext**: Efficient database connection management
4. **In-memory subscriptions**: Fast real-time updates
5. **Filtering at DB level**: Push filtering to database

## Best Practices Demonstrated

1. **Separation of Concerns**: Models, Data, GraphQL layers
2. **Input Validation**: Validate before processing
3. **Error Handling**: Structured GraphQL errors
4. **Documentation**: XML comments for GraphQL descriptions
5. **Type Safety**: Strong typing throughout
6. **Async/Await**: Proper async patterns
7. **Resource Management**: Using statements for DbContext
8. **Naming Conventions**: Clear, consistent names

## Common Patterns Demonstrated

1. **Repository Pattern**: Through EF Core DbContext
2. **Data Transfer Objects**: Input types for mutations
3. **Factory Pattern**: IDbContextFactory for DataLoaders
4. **Observer Pattern**: Subscriptions
5. **Strategy Pattern**: Different query strategies (filtering, sorting)

## Further Learning Resources

- [HotChocolate Documentation](https://chillicream.com/docs/hotchocolate)
- [GraphQL Official Site](https://graphql.org)
- [GraphQL Best Practices](https://graphql.org/learn/best-practices/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)

## What Makes This Implementation Complete

- **All GraphQL Operations**: Queries, Mutations, Subscriptions
- **All Type Systems**: Object, Scalar, Enum, Input, Interface, Union
- **Performance Optimization**: DataLoaders, Projections
- **Real-world Features**: Validation, Error Handling, Relationships
- **Advanced Features**: Directives, Custom Scalars, Filtering, Sorting, Pagination
- **Best Practices**: Separation of concerns, async/await, proper error handling
- **Comprehensive Examples**: 30+ examples in documentation
- **Production-Ready**: All necessary features for real applications

This is a **complete reference implementation** for learning GraphQL with .NET!
