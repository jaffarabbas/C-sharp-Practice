# GraphQL Demo - Complete Learning Project

A comprehensive ASP.NET Core project demonstrating all main GraphQL concepts using HotChocolate.

## What You'll Learn

This project covers every essential GraphQL concept:

1. **Queries** - Reading data
2. **Mutations** - Creating, updating, deleting data
3. **Subscriptions** - Real-time updates
4. **Types** - Object, Scalar, Enum, Input, Interface, Union
5. **DataLoaders** - Solving N+1 query problem
6. **Directives** - Modifying execution behavior
7. **Filtering** - Advanced query filtering
8. **Sorting** - Data ordering
9. **Pagination** - Handling large datasets
10. **Validation** - Input validation
11. **Error Handling** - Structured error responses

## Project Structure

```
GraphQLDemo/
├── Models/                          # Domain models
│   ├── Author.cs                    # Author entity
│   ├── Book.cs                      # Book entity & BookGenre enum
│   └── Review.cs                    # Review entity
│
├── Data/
│   └── AppDbContext.cs              # EF Core context with seed data
│
├── GraphQL/
│   ├── Queries/
│   │   └── Query.cs                 # All query operations
│   │
│   ├── Mutations/
│   │   └── Mutation.cs              # All mutation operations
│   │
│   ├── Subscriptions/
│   │   └── Subscription.cs          # Real-time subscriptions
│   │
│   ├── Types/
│   │   ├── CustomScalars.cs         # URL and Email scalar types
│   │   ├── Interfaces.cs            # IReviewable interface
│   │   ├── UnionTypes.cs            # Union type definitions
│   │   └── Validators.cs            # Input validation logic
│   │
│   ├── DataLoaders/
│   │   ├── AuthorDataLoader.cs      # Batch loading authors
│   │   └── BookDataLoader.cs        # Batch loading books
│   │
│   └── Directives/
│       └── UpperCaseDirective.cs    # Custom @uppercase directive
│
├── Program.cs                       # Application configuration
├── GRAPHQL_EXAMPLES.md             # Comprehensive query examples
└── README.md                        # This file
```

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- Any IDE (Visual Studio, VS Code, Rider)

### Installation

1. Clone the repository
2. Navigate to the project directory
3. Restore dependencies:
   ```bash
   dotnet restore
   ```

### Running the Application

```bash
dotnet run
```

The application will start on `http://localhost:5000`

### Accessing the GraphQL IDE

Open your browser and navigate to:
```
http://localhost:5000/graphql
```

This opens **Banana Cake Pop**, an interactive GraphQL IDE where you can:
- Explore the complete schema
- Test queries, mutations, and subscriptions
- View auto-generated documentation
- Use syntax highlighting and autocomplete

## Quick Start Examples

### Simple Query
```graphql
query {
  books {
    title
    author {
      name
    }
  }
}
```

### Query with Filtering
```graphql
query {
  books(where: { genre: { eq: FANTASY } }) {
    title
    price
  }
}
```

### Create Mutation
```graphql
mutation {
  addBook(input: {
    title: "New Book"
    description: "A great book"
    pages: 300
    price: 25.99
    genre: FICTION
    authorId: 1
  }) {
    id
    title
  }
}
```

### Subscription
```graphql
subscription {
  onBookAdded {
    title
    author {
      name
    }
  }
}
```

For more examples, see [GRAPHQL_EXAMPLES.md](GRAPHQL_EXAMPLES.md)

## Main Concepts Explained

### 1. Queries ([Query.cs](GraphQL/Queries/Query.cs))

Queries are read operations. Unlike REST where you get fixed responses, GraphQL lets you request exactly the fields you need.

**Implemented Features:**
- Basic queries
- Parameterized queries
- Nested queries (relationships)
- Filtering with `[UseFiltering]`
- Sorting with `[UseSorting]`
- Projections with `[UseProjection]`
- Aggregate queries
- Search functionality

### 2. Mutations ([Mutation.cs](GraphQL/Mutations/Mutation.cs))

Mutations are write operations (create, update, delete).

**Implemented Features:**
- Create operations
- Update operations
- Delete operations
- Batch operations
- Custom payload types
- Input validation

### 3. Subscriptions ([Subscription.cs](GraphQL/Subscriptions/Subscription.cs))

Subscriptions enable real-time updates via WebSocket.

**Implemented Features:**
- Event-based subscriptions
- Filtered subscriptions
- Streaming data
- Multiple subscription types

### 4. Types

**Object Types:** `Book`, `Author`, `Review` - Complex types with multiple fields

**Scalar Types:** Primitive types (`String`, `Int`, `Float`, `Boolean`, `ID`)

**Custom Scalars:** `URL`, `Email` - Validated custom types

**Enum Types:** `BookGenre` - Fixed set of values

**Input Types:** `AddBookInput`, `UpdateBookInput` - For mutation parameters

**Interface Types:** `IReviewable` - Abstract type that other types implement

**Union Types:** `PublicationResult` - Can be one of several types

### 5. DataLoaders ([DataLoaders folder](GraphQL/DataLoaders/))

DataLoaders solve the **N+1 query problem**:

**Without DataLoader:**
```
Query 1: Get all books (10 books)
Query 2: Get author for book 1
Query 3: Get author for book 2
...
Query 11: Get author for book 10
Total: 11 queries
```

**With DataLoader:**
```
Query 1: Get all books (10 books)
Query 2: Get all authors in one batch
Total: 2 queries
```

**Implemented:**
- `AuthorDataLoader` - Batch load authors
- `BookDataLoader` - Batch load books
- `BooksByAuthorDataLoader` - Group load books by author

### 6. Filtering, Sorting, Pagination

**Filtering:**
```graphql
books(where: {
  and: [
    { price: { lt: 20 } }
    { genre: { eq: FANTASY } }
  ]
})
```

**Sorting:**
```graphql
books(order: { price: DESC })
```

**Pagination:**
```graphql
books(first: 10, skip: 20)
```

### 7. Directives ([UpperCaseDirective.cs](GraphQL/Directives/UpperCaseDirective.cs))

Directives modify execution behavior:

**Built-in:** `@skip`, `@include`
**Custom:** `@uppercase`

```graphql
query GetBook($includeAuthor: Boolean!) {
  book(id: 1) {
    title
    author @include(if: $includeAuthor) {
      name
    }
  }
}
```

### 8. Validation ([Validators.cs](GraphQL/Types/Validators.cs))

Input validation ensures data quality:
- Required fields
- Format validation (email)
- Range validation (rating 1-5)
- Business rules

### 9. Error Handling

GraphQL provides structured error responses:
```json
{
  "errors": [
    {
      "message": "Rating must be between 1 and 5",
      "path": ["addReview"]
    }
  ]
}
```

## Database

The project uses an **in-memory database** for simplicity. Data is seeded automatically on startup with:
- 3 Authors (J.K. Rowling, George Orwell, Isaac Asimov)
- 5 Books (Harry Potter series, 1984, Animal Farm, Foundation)
- 5 Reviews

To use a real database (SQL Server, PostgreSQL, etc.), modify [Program.cs](Program.cs):

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
```

## Schema Exploration

Once running, you can explore the entire schema using introspection:

```graphql
query {
  __schema {
    types {
      name
      kind
    }
  }
}
```

Or view type details:
```graphql
query {
  __type(name: "Book") {
    fields {
      name
      type {
        name
      }
    }
  }
}
```

## Learning Path

1. **Start with Simple Queries** - Understand how to fetch data
2. **Try Nested Queries** - See how relationships work
3. **Experiment with Filtering** - Learn advanced query capabilities
4. **Test Mutations** - Create, update, delete data
5. **Explore Subscriptions** - Try real-time updates
6. **Study DataLoaders** - Understand performance optimization
7. **Review the Code** - Learn implementation details

## GraphQL vs REST

| Feature | REST | GraphQL |
|---------|------|---------|
| Endpoints | Multiple endpoints | Single endpoint |
| Data Fetching | Fixed responses | Request exactly what you need |
| Over-fetching | Common | Eliminated |
| Under-fetching | Requires multiple requests | Single request |
| Versioning | URL versioning | Schema evolution |
| Real-time | Polling/SSE | Built-in subscriptions |

## Resources

- [GraphQL Official Documentation](https://graphql.org/)
- [HotChocolate Documentation](https://chillicream.com/docs/hotchocolate)
- [GraphQL Best Practices](https://graphql.org/learn/best-practices/)
- [Examples in this project](GRAPHQL_EXAMPLES.md)

## License

This is a learning project - feel free to use and modify as needed.
