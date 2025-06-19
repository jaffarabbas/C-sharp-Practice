using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using TestApi.Repository;
using TestApi.Models;

public class UnitOfWork : IUnitOfWork
{
    private readonly TestContext _context;
    private Hashtable _repositories;
    private IDbContextTransaction? _transaction;

    private IITemRepository? _itemRepository;

    public UnitOfWork(TestContext context)
    {
        _context = context;
    }

    public IITemRepository iTemRepository =>
        _itemRepository ??= new ItemRepository(_context);

    public IGenericRepository<T> Repository<T>() where T : class
    {
        _repositories ??= new Hashtable();
        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repoInstance = new GenericRepository<T>(_context);
            _repositories.Add(type, repoInstance);
        }

        return (IGenericRepository<T>)_repositories[type]!;
    }

    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

    public async Task BeginTransactionAsync()
    {
        if (_transaction == null)
            _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
