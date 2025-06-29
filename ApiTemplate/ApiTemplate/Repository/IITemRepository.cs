using ApiTemplate.Models;
using TestApi.Dto;

namespace TestApi.Repository
{
    public interface IITemRepository : IGenericRepository<TblItem>
    {
        Task<List<TblItemDto>> GetAllItemsWithPricingTitle();
        Task<TblItemDto?> GetItemWithPricingTitleById(int id);
    }
}
