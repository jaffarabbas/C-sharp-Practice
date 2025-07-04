using ApiTemplate.Models;
using ApiTemplate.Dto;

namespace ApiTemplate.Repository
{
    public interface IITemRepository : IGenericRepository<TblItem>
    {
        Task<List<TblItemDto>> GetAllItemsWithPricingTitle();
        Task<TblItemDto?> GetItemWithPricingTitleById(int id);
    }
}
