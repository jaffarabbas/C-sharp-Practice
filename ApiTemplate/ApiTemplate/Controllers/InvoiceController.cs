using Microsoft.AspNetCore.Mvc;
using TestApi.Models;
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
            var invoicesRepo = _unitOfWork.Repository<TblInvoice>();
            var data = await invoicesRepo.GetEnityPagedAsync(pageNumber,pageSize);
            return Ok(data);
        }
    }
}
