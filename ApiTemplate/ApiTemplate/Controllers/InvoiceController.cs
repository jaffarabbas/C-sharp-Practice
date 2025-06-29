using ApiTemplate.Helper.Enum;
using ApiTemplate.Models;
using Microsoft.AspNetCore.Mvc;
using TestApi.Repository;

namespace TestApi.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public InvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("GetAllInvoices")]
        public async Task<IActionResult> GetAllItems(int pageNumber = 1,int pageSize = 50)
        {
            var invoicesRepo = _unitOfWork.RepositoryWrapper<TblInvoice>();
            var data = await invoicesRepo.GetPagedAsync(pageNumber, pageSize,OrmType.Dapper);
            return Ok(data);
        }
    }
}
