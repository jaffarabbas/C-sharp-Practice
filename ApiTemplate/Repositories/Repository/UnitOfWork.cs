using ApiTemplate.Dtos;
using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Repositories.Repository;
using System.Collections;
using System.Data;
using static Dapper.SqlMapper;

public class UnitOfWork : IUnitOfWork
{
    private readonly TestContext _context;
    private Hashtable _repositories;
    private IDbContextTransaction? _transaction;
    private IDbConnection _connection;
    private IDbTransaction _dapperTransaction;
    private IMemoryCache _cache;
    private readonly IOptions<JWTSetting> _setting;

    private IITemRepository? _itemRepository;
    private IAuthRepository? _authRepository;

    public UnitOfWork(TestContext context, IDbConnection connection,IMemoryCache cache,IOptions<JWTSetting> setting)
    {
        _context = context;
        _connection = connection;
        _connection.Open();
        _dapperTransaction = _connection.BeginTransaction();
        _cache = cache;
        _setting = setting; 
    }

    #region Bussniess Layer Repositories
    public IITemRepository iTemRepository =>
        _itemRepository ??= new ItemRepository(_context,_connection,_cache, _dapperTransaction);

    public IAuthRepository IAuthRepository =>
        _authRepository ??= new AuthRepository(_context, _connection, _cache, _dapperTransaction,_setting);
    #endregion
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

    public IGenericRepositoryWrapper<T> RepositoryWrapper<T>() where T : class
    {
        _repositories ??= new Hashtable();
        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repoInstance = new GenericRepository<T>(_context, _connection, _cache, _dapperTransaction);
            _repositories.Add(type, repoInstance);
        }

        return (IGenericRepositoryWrapper<T>)_repositories[type]!;
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
