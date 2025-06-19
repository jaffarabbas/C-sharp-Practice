using TestApi.Dto;
using TestApi.Models;

namespace TestApi.Repository
{
    public interface IITemRepository : IGenericRepository<TblItem>
    {
        Task<List<TblItemDto>> GetAllItemsWithPricingTitle();
        Task<TblItemDto?> GetItemWithPricingTitleById(int id);
    }
}
