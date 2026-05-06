using GraphQLDemo.Data;
using GraphQLDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.GraphQL.DataLoaders;

/// <summary>
/// DataLoader for Books - demonstrates batch loading with DataLoader
/// </summary>
public class BookDataLoader : BatchDataLoader<int, Book>
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public BookDataLoader(
        IDbContextFactory<AppDbContext> dbContextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options!)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<int, Book>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var books = await context.Books
            .Where(b => keys.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id, cancellationToken);

        return books;
    }
}

/// <summary>
/// DataLoader for Books by Author - demonstrates group data loading
/// This is used when you want to load all books for multiple authors efficiently
/// </summary>
public class BooksByAuthorDataLoader : GroupedDataLoader<int, Book>
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public BooksByAuthorDataLoader(
        IDbContextFactory<AppDbContext> dbContextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options!)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<ILookup<int, Book>> LoadGroupedBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var books = await context.Books
            .Where(b => keys.Contains(b.AuthorId))
            .ToListAsync(cancellationToken);

        return books.ToLookup(b => b.AuthorId);
    }
}
