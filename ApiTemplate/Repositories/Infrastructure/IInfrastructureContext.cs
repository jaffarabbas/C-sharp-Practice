using DBLayer.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace Repositories.Infrastructure
{
    /// <summary>
    /// Encapsulates all infrastructure dependencies (connections, cache, etc.)
    /// This removes the need to pass individual infrastructure components around.
    /// </summary>
    public interface IInfrastructureContext
    {
        TestContext DbContext { get; }
        IDbConnection DapperConnection { get; }
        IDbTransaction DapperTransaction { get; }
        IMemoryCache Cache { get; }
    }
}
