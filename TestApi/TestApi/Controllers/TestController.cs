using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestApi.Models;
using TestApi.Repository;

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public IITemRepository _itemRepository;
        public TestController(IITemRepository repository)
        {
            _itemRepository = repository;
        }

        [HttpGet]
        [Route("GetAllItems")]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemRepository.GetAllItems();
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
            var addedItem = await _itemRepository.AddItem(item);
            return CreatedAtAction(nameof(GetAllItems), new { id = addedItem.TranId }, addedItem);
        }
    }
}
