using ApiTemplate.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using TestApi.Dto;

namespace TestApi.Repository
{
    public class ItemRepository : GenericRepository<TblItem>, IITemRepository
    {
        private readonly TestContext _context;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction? _transaction;
        private readonly IMemoryCache _cache;

        public ItemRepository(TestContext context,IDbConnection connection,IMemoryCache cache, IDbTransaction? transaction) : base(context,connection,cache,transaction)
        {
            _context = context;
            _connection = connection;
            _transaction = transaction;
            _cache = cache;
        }

        public async Task<List<TblItemDto>> GetAllItemsWithPricingTitle()
        {
            var result = await (from item in _context.TblItems
                                join price in _context.TblPricingLists
                                on item.TranId equals price.ItemId
                                select new TblItemDto
                                {
                                    TranId = item.TranId,
                                    ItemRefNo = item.ItemRefNo,
                                    ItemTitle = item.ItemTitle,
                                    SaleRate = item.SaleRate,
                                    TransactionDate = item.TransactionDate,
                                    PricingTitle = price.PricingTitle
                                }).ToListAsync();

            return result;
        }

        public async Task<TblItemDto?> GetItemWithPricingTitleById(int id)
        {
            var result = _connection.QueryFirstOrDefaultAsync<TblItemDto>(
                @"SELECT i.TranId, i.ItemRefNo, i.ItemTitle, i.SaleRate, i.TransactionDate, p.PricingTitle
                  FROM tblItem i
                  INNER JOIN tblPricingList p ON i.TranId = p.ItemId
                  WHERE i.TranId = @Id",
                new { Id = id },
                transaction: _transaction);

            return await result;
        }
    }
}
