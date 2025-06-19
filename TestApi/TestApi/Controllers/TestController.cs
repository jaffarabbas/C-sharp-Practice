using Microsoft.AspNetCore.Mvc;
using TestApi.Attributes;
using TestApi.Models;
using TestApi.Repository; // Your generic repo namespace

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuth]
    public class TestController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public TestController(IGenericRepository<TblItem> repository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("GetAllItems")]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _unitOfWork.iTemRepository.GetAllAsync();
            return Ok(items);
        }

        [HttpPost]
        [Route("AddItem")]
        public async Task<IActionResult> AddItem([FromBody] TblItem item)
        {
            if (item == null)
            {
                return BadRequest("Item cannot be null");
            }

            var addedItem = await _unitOfWork.iTemRepository.AddAsync(item);
            await _unitOfWork.iTemRepository.SaveAsync(); // Ensure SaveChanges is called

            return CreatedAtAction(nameof(GetAllItems), new { id = addedItem.TranId }, addedItem);
        }

        [HttpPost("AddItemWithTransaction")]
        public async Task<IActionResult> AddItemWithTransaction([FromBody] TblItem item)
        {
            if (item == null) return BadRequest("Item is null");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var addedItem = await _unitOfWork.Repository<TblItem>().AddAsync(item);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitAsync();

                return CreatedAtAction(nameof(AddItemWithTransaction), new { id = addedItem.TranId }, addedItem);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetItemWithPricingTitle")]
        public async Task<IActionResult> GetItemWithPricingTitle()
        {
            try
            {
                var items = await _unitOfWork.iTemRepository.GetAllItemsWithPricingTitle();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        //dapper
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var repo = _unitOfWork.Repository<TblItem>();
            var data = await repo.GetAllAsync("tblItem");
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TblItem item)
        {
            var repo = _unitOfWork.Repository<TblItem>();
            await repo.AddAsync("tblItem", item);
            _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet()]
        [Route("GetItemWithPricingTitleByID/{id}")]
        public async Task<IActionResult> GetItemWithPricingTitleByID(int id)
        {
            try
            {
                var item = await _unitOfWork.iTemRepository.GetItemWithPricingTitleById(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
