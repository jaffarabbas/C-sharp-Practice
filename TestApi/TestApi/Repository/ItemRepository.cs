using TestApi.Models;

namespace TestApi.Repository
{
    public class ItemRepository : IITemRepository
    {
        private readonly TestContext _context;
        public ItemRepository(TestContext context)
        {
            _context = context;
        }
        public Task<TblItem> AddItem(TblItem item)
        {
            var itemID = _context.TblItems.Max(x => x.TranId) + 1;
            item.TranId = itemID;
            _context.TblItems.Add(item);
            _context.SaveChanges();
            return Task.FromResult(item);
        }

        public Task<bool> DeleteItem(int id)
        {
            var item = _context.TblItems.Find(id);
            if (item != null)
            {
                _context.TblItems.Remove(item);
                _context.SaveChanges();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<List<TblItem>> GetAllItems()
        {
            var items = _context.TblItems.ToList();
            return Task.FromResult(items);
        }

        public Task<TblItem> GetItemById(int id)
        {
            var item = _context.TblItems.Find(id);
            return Task.FromResult(item);
        }

        public async Task<TblItem> UpdateItem(TblItem item)
        {
            var existingItem = _context.TblItems.Find(item.TranId);
            if (existingItem != null)
            {
                existingItem.ItemRefNo = item.ItemRefNo;
                existingItem.ItemTitle = item.ItemTitle;
                existingItem.SaleRate = item.SaleRate;
                existingItem.TransactionDate = item.TransactionDate;
                _context.SaveChanges();
            }
            return await Task.FromResult(existingItem);
        }
    }
}
