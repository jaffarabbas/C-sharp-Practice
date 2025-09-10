using ApiTemplate.Dtos;
using ApiTemplate.Helper.Enum;
using DBLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ApiTemplate.Repository;
using ApiTemplate.Shared.Helper.Constants;

namespace ApiTemplate.Controllers
{
    [ApiVersion(ApiVersioningConstants.CurrentVersion)]
    [Route(ApiVersioningConstants.versionRoute)]
    [ApiController]
    public class InvoiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public InvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("GetAllInvoices")]
        public async Task<IActionResult> GetAllItems(int pageNumber = 1, int pageSize = 50)
        {
            var invoicesRepo = _unitOfWork.RepositoryWrapper<TblInvoice>();
            CrudOptions options = new CrudOptions
            {
                OrderBy = "TranID DESC"
            };
            var data = await invoicesRepo.GetPagedAsync(pageNumber, pageSize, OrmType.Dapper, options);
            return Ok(data);
        }
    }
}
