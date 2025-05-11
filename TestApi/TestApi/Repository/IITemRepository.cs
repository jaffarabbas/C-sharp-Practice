using TestApi.Models;

namespace TestApi.Repository
{
    public interface IITemRepository
    {
        Task<List<TblItem>> GetAllItems();
        Task<TblItem> GetItemById(int id);
        Task<TblItem> AddItem(TblItem item);
        Task<TblItem> UpdateItem(TblItem item);
        Task<bool> DeleteItem(int id);
    }
}
