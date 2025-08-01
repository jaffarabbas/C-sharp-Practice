﻿using Repositories.Repository;
using System.Data;

namespace ApiTemplate.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IITemRepository iTemRepository { get; }
        IAuthRepository IAuthRepository { get; }
        IGenericRepository<T> Repository<T>() where T : class;
        IGenericRepositoryWrapper<T> RepositoryWrapper<T>() where T : class;

        Task<int> SaveAsync();

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

        //dapper
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void Commit();
        void Rollback();
    }

}
