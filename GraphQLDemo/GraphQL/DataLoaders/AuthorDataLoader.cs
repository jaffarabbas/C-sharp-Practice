using GraphQLDemo.Data;
using GraphQLDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.GraphQL.DataLoaders;

/// <summary>
/// DataLoader for Authors - demonstrates DataLoaders to solve the N+1 query problem
///
/// The N+1 Problem:
/// When fetching books and their authors, without DataLoader:
/// 1 query to get all books + N queries to get each book's author = N+1 queries
///
/// With DataLoader:
/// 1 query to get all books + 1 batched query to get all authors = 2 queries
///
/// DataLoaders batch and cache data requests automatically
/// </summary>
public class AuthorDataLoader : BatchDataLoader<int, Author>
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public AuthorDataLoader(
        IDbContextFactory<AppDbContext> dbContextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options!)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<int, Author>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var authors = await context.Authors
            .Where(a => keys.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);

        return authors;
    }
}
