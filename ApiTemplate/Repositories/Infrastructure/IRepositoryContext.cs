using ApiTemplate.Dtos;
using DBLayer.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Data;

namespace Repositories.Infrastructure
{
    /// <summary>
    /// Extended infrastructure context that includes all dependencies needed for repository creation.
    /// This removes the need to pass individual dependencies through UnitOfWork.
    /// </summary>
    public interface IRepositoryContext
    {
        // Infrastructure
        TestContext DbContext { get; }
        IDbConnection DapperConnection { get; }
        IDbTransaction DapperTransaction { get; }
        IMemoryCache Cache { get; }

        // Configuration
        IOptions<JWTSetting> JwtSettings { get; }

        // Service Resolution
        IServiceProvider ServiceProvider { get; }

        // UnitOfWork reference (for repositories that need it)
        object UnitOfWork { get; }
    }
}
