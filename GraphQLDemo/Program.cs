using GraphQLDemo.Data;
using GraphQLDemo.GraphQL.DataLoaders;
using GraphQLDemo.GraphQL.Directives;
using GraphQLDemo.GraphQL.Mutations;
using GraphQLDemo.GraphQL.Queries;
using GraphQLDemo.GraphQL.Types;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// GRAPHQL CONFIGURATION - ALL MAIN CONCEPTS
// ============================================

// 1. Configure Database Context (use Pooled for GraphQL)
builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseInMemoryDatabase("GraphQLDemoDB")); // Using in-memory DB for demo

// 2. Configure GraphQL Server
builder.Services
    .AddGraphQLServer()

    // Add Query, Mutation, and Subscription types
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()

    // Add custom types (Interface, Union)
    .AddType<BookType>()
    .AddType<ReviewableType>()

    // Add custom scalars
    .AddType<CustomUrlType>()
    .AddType<EmailType>()

    // Add custom directives
    .AddDirectiveType<UpperCaseDirectiveType>()

    // Enable filtering, sorting, and projections
    .AddFiltering()
    .AddSorting()
    .AddProjections()

    // Add DataLoaders to solve N+1 problem
    .AddDataLoader<AuthorDataLoader>()
    .AddDataLoader<BookDataLoader>()
    .AddDataLoader<BooksByAuthorDataLoader>()

    // Enable mutations with conventions
    .AddMutationConventions()

    // Enable subscriptions (real-time updates via WebSockets)
    .AddInMemorySubscriptions();

var app = builder.Build();

// Initialize the database with seed data
using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var context = contextFactory.CreateDbContext();
    context.Database.EnsureCreated();
}

// ============================================
// MIDDLEWARE CONFIGURATION
// ============================================

app.UseHttpsRedirection();

// Enable WebSocket support for subscriptions
app.UseWebSockets();

// Map GraphQL endpoint
app.MapGraphQL("/graphql");

// Enable Banana Cake Pop GraphQL IDE (like GraphiQL or Playground)
// Access it at http://localhost:5000/graphql
// This provides a UI to explore the schema and test queries

app.Run();

// ============================================
// GRAPHQL CONCEPTS COVERED IN THIS PROJECT:
// ============================================
/*
 * 1. QUERIES - Read operations (Query.cs)
 *    - Basic queries
 *    - Queries with arguments
 *    - Filtering with [UseFiltering]
 *    - Sorting with [UseSorting]
 *    - Projections with [UseProjection]
 *    - Nested queries (relationships)
 *    - Aggregate queries
 *    - Search functionality
 *
 * 2. MUTATIONS - Write operations (Mutation.cs)
 *    - Create operations (POST)
 *    - Update operations (PUT/PATCH)
 *    - Delete operations (DELETE)
 *    - Batch operations
 *    - Custom payload types
 *
 * 3. SUBSCRIPTIONS - Real-time updates (Subscription.cs)
 *    - Event-based subscriptions
 *    - Filtered subscriptions
 *    - Streaming data
 *    - WebSocket support
 *
 * 4. TYPES
 *    - Object Types (Book, Author, Review)
 *    - Scalar Types (String, Int, Float, Boolean, ID)
 *    - Custom Scalars (URL, Email)
 *    - Enum Types (BookGenre)
 *    - Input Types (AddBookInput, UpdateBookInput, etc.)
 *    - Interface Types (IReviewable)
 *    - Union Types (PublicationResult)
 *
 * 5. DATALOADER - Solving N+1 problem
 *    - Batch loading (AuthorDataLoader, BookDataLoader)
 *    - Grouped loading (BooksByAuthorDataLoader)
 *    - Automatic caching
 *
 * 6. DIRECTIVES - Execution behavior modification
 *    - Custom directives (@uppercase)
 *    - Built-in directives (@skip, @include)
 *
 * 7. SCHEMA FEATURES
 *    - Filtering (where clauses)
 *    - Sorting (order by)
 *    - Pagination (first, skip, cursor-based)
 *    - Field selection (projections)
 *
 * 8. ERROR HANDLING
 *    - GraphQLException for validation errors
 *    - Custom error messages
 *
 * 9. VALIDATION
 *    - Input validation (Validators.cs)
 *    - Business rule validation
 */
