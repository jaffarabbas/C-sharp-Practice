using ApiTemplate.Dtos;
using DBLayer.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Data;

namespace Repositories.Infrastructure
{
    /// <summary>
    /// Complete context for repository creation - consolidates ALL dependencies.
    /// This is created by UnitOfWork and passed to RepositoryFactory.
    /// </summary>
    public class RepositoryContext : IRepositoryContext
    {
        public TestContext DbContext { get; }
        public IDbConnection DapperConnection { get; }
        public IDbTransaction DapperTransaction { get; }
        public IMemoryCache Cache { get; }
        public IOptions<JWTSetting> JwtSettings { get; }
        public IServiceProvider ServiceProvider { get; }
        public object UnitOfWork { get; }

        public RepositoryContext(
            TestContext dbContext,
            IDbConnection dapperConnection,
            IDbTransaction dapperTransaction,
            IMemoryCache cache,
            IOptions<JWTSetting> jwtSettings,
            IServiceProvider serviceProvider,
            object unitOfWork)
        {
            DbContext = dbContext;
            DapperConnection = dapperConnection;
            DapperTransaction = dapperTransaction;
            Cache = cache;
            JwtSettings = jwtSettings;
            ServiceProvider = serviceProvider;
            UnitOfWork = unitOfWork;
        }
    }
}
