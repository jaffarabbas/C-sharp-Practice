using DBLayer.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace Repositories.Infrastructure
{
    /// <summary>
    /// Implementation of infrastructure context that holds all infrastructure dependencies.
    /// This is created and managed by UnitOfWork.
    /// </summary>
    public class InfrastructureContext : IInfrastructureContext
    {
        public TestContext DbContext { get; }
        public IDbConnection DapperConnection { get; }
        public IDbTransaction DapperTransaction { get; }
        public IMemoryCache Cache { get; }

        public InfrastructureContext(
            TestContext dbContext,
            IDbConnection dapperConnection,
            IDbTransaction dapperTransaction,
            IMemoryCache cache)
        {
            DbContext = dbContext;
            DapperConnection = dapperConnection;
            DapperTransaction = dapperTransaction;
            Cache = cache;
        }
    }
}
