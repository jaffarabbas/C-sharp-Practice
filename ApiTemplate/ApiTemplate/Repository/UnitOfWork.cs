using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using TestApi.Repository;
using TestApi.Models;
using System.Data;
using Microsoft.Extensions.Caching.Memory;

public class UnitOfWork : IUnitOfWork
{
    private readonly TestContext _context;
    private Hashtable _repositories;
    private IDbContextTransaction? _transaction;
    private IDbConnection _connection;
    private IDbTransaction _dapperTransaction;
    private IMemoryCache _cache;

    private IITemRepository? _itemRepository;

    public UnitOfWork(TestContext context, IDbConnection connection,IMemoryCache cache)
    {
        _context = context;
        _connection = connection;
        _connection.Open();
        _dapperTransaction = _connection.BeginTransaction();
        _cache = cache;
    }

    public IITemRepository iTemRepository =>
        _itemRepository ??= new ItemRepository(_context,_connection,_cache, _dapperTransaction);

    public IDbConnection Connection => _connection;
    public IDbTransaction Transaction => _dapperTransaction;

    public IGenericRepository<T> Repository<T>() where T : class
    {
        _repositories ??= new Hashtable();
        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repoInstance = new GenericRepository<T>(_context,_connection,_cache,_dapperTransaction);
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

    public void Commit()
    {
        _dapperTransaction.Commit();
        _dapperTransaction.Dispose();
        _dapperTransaction = _connection.BeginTransaction();
    }

    public void Rollback()
    {
        _dapperTransaction.Rollback();
        _dapperTransaction.Dispose();
        _dapperTransaction = _connection.BeginTransaction();
    }

    public void DisposeDapper()
    {
        _dapperTransaction?.Dispose();
        _connection?.Dispose();
    }
}
