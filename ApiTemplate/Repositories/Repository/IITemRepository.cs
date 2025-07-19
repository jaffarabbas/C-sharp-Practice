using DBLayer.Models;
using ApiTemplate.Dtos;

namespace ApiTemplate.Repository
{
    public interface IITemRepository : IGenericRepository<TblItem>
    {
        Task<List<TblItemDto>> GetAllItemsWithPricingTitle();
        Task<TblItemDto?> GetItemWithPricingTitleById(int id);
    }
}
