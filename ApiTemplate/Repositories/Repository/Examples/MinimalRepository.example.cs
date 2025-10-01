using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Repositories.Attributes;

namespace Repositories.Repository.Examples
{
    // ============================================
    // EXAMPLE 1: MINIMAL - Just TestContext
    // ============================================
    public interface IMinimalRepository
    {
        Task<int> GetItemCountAsync();
        Task<TblItem?> GetItemByIdAsync(int id);
    }

    [AutoRegisterRepository(typeof(IMinimalRepository))]
    public class MinimalRepository : IMinimalRepository
    {
        private readonly TestContext _context;

        // ✅ ONLY 1 parameter - that's all you need!
        public MinimalRepository(TestContext context)
        {
            _context = context;
        }

        public async Task<int> GetItemCountAsync()
        {
            return await _context.TblItems.CountAsync();
        }

        public async Task<TblItem?> GetItemByIdAsync(int id)
        {
            return await _context.TblItems.FindAsync(id);
        }
    }

    // ============================================
    // EXAMPLE 2: DAPPER ONLY - Just IDbConnection
    // ============================================
    public interface IDapperOnlyRepository
    {
        Task<List<dynamic>> GetReportAsync();
    }

    [AutoRegisterRepository(typeof(IDapperOnlyRepository))]
    public class DapperOnlyRepository : IDapperOnlyRepository
    {
        private readonly System.Data.IDbConnection _connection;

        // ✅ ONLY 1 parameter - just Dapper connection
        public DapperOnlyRepository(System.Data.IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<dynamic>> GetReportAsync()
        {
            var sql = "SELECT * FROM tblItem";
            return (await Dapper.SqlMapper.QueryAsync(_connection, sql)).ToList();
        }
    }

    // ============================================
    // EXAMPLE 3: EF + CACHE - Just 2 parameters
    // ============================================
    public interface ICachedRepository
    {
        Task<TblItem?> GetCachedItemAsync(int id);
    }

    [AutoRegisterRepository(typeof(ICachedRepository))]
    public class CachedRepository : ICachedRepository
    {
        private readonly TestContext _context;
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;

        // ✅ ONLY 2 parameters
        public CachedRepository(
            TestContext context,
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<TblItem?> GetCachedItemAsync(int id)
        {
            var cacheKey = $"Item_{id}";

            if (_cache.TryGetValue(cacheKey, out object? cachedObj) && cachedObj is TblItem cachedItem)
                return cachedItem;

            var item = await _context.TblItems.FindAsync(id);
            if (item != null)
            {
                _cache.Set(cacheKey, item, TimeSpan.FromMinutes(5));
            }

            return item;
        }
    }

    // ============================================
    // HOW TO USE THESE REPOSITORIES
    // ============================================
    /*

    // In your controller or service:

    // Example 1: Minimal Repository
    var minimalRepo = _unitOfWork.GetRepository<IMinimalRepository>();
    var count = await minimalRepo.GetItemCountAsync();

    // Example 2: Dapper Only Repository
    var dapperRepo = _unitOfWork.GetRepository<IDapperOnlyRepository>();
    var report = await dapperRepo.GetReportAsync();

    // Example 3: Cached Repository
    var cachedRepo = _unitOfWork.GetRepository<ICachedRepository>();
    var item = await cachedRepo.GetCachedItemAsync(123);

    // ALL WORK PERFECTLY! ✅
    // No need to use all 5 parameters if you don't need them!

    */
}
