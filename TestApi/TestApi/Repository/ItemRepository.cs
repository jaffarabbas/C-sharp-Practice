using Microsoft.EntityFrameworkCore;
using TestApi.Dto;
using TestApi.Models;

namespace TestApi.Repository
{
    public class ItemRepository : GenericRepository<TblItem>, IITemRepository
    {
        private readonly TestContext _context;
        public ItemRepository(TestContext context) : base(context)
        {
            _context = context;
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

    }
}
