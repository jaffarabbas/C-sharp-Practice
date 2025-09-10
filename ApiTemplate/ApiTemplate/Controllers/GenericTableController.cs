using ApiTemplate.Helper.Enum;
using ApiTemplate.Repository;
using Microsoft.AspNetCore.Mvc;
using Repositories.Services;
using Shared.Dtos;
using System.Text.Json;
using ApiTemplate.Shared.Helper.Constants;

namespace ApiTemplate.Controllers
{
    [ApiVersion(ApiVersioningConstants.CurrentVersion)]
    [Route("api/v{version:apiVersion}/table")]
    [ApiController]
    public class GenericTableController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ITableOperationService _tableService;

        public GenericTableController(IUnitOfWork uow)
        {
            _uow = uow;
            _tableService = _uow.TableOperations; // resolved via UnitOfWork property
        }

        // GET api/table/{tableName}?orm=Dapper
        // GET api/table/{tableName}?pageNumber=1&pageSize=25&orderBy=TranID DESC  (auto switches to Paged)
        [HttpGet("{tableName}")]
        public async Task<IActionResult> GetAll(
            string tableName,
            [FromQuery] OrmType orm = OrmType.Dapper,
            [FromQuery] int? pageNumber = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] string? orderBy = null)
        {
            var result = await _tableService.GetAllAsync(tableName, orm, pageNumber, pageSize, orderBy);
            return Ok(result);
        }

        // GET api/table/{tableName}/{id}
        [HttpGet("{tableName}/{id}")]
        public async Task<IActionResult> GetById(string tableName, string id, [FromQuery] OrmType orm = OrmType.Dapper)
        {
            var result = await _tableService.GetByIdAsync(tableName, id, orm);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST api/table/{tableName}
        // Body: { "Username":"john", "Email":"x@test.com", ... }
        [HttpPost("{tableName}")]
        public async Task<IActionResult> Add(
            string tableName,
            [FromBody] JsonElement body,
            [FromQuery] OrmType orm = OrmType.Dapper)
        {
            var result = await _tableService.AddAsync(tableName, body, orm);
            return Created($"/api/table/{tableName}", result);
        }

        // PUT api/table/{tableName}/{id}
        [HttpPut("{tableName}/{id}")]
        public async Task<IActionResult> Update(
            string tableName,
            string id,
            [FromBody] JsonElement body,
            [FromQuery] OrmType orm = OrmType.Dapper)
        {
            var result = await _tableService.UpdateAsync(tableName, id, body, orm);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // DELETE api/table/{tableName}/{id}
        [HttpDelete("{tableName}/{id}")]
        public async Task<IActionResult> Delete(
            string tableName,
            string id,
            [FromQuery] OrmType orm = OrmType.Dapper)
        {
            var success = await _tableService.DeleteAsync(tableName, id, orm);
            if (!success) return NotFound();
            return NoContent();
        }

        // Advanced: still allow raw request body if needed
        // POST api/table/execute
        [HttpPost("execute")]
        public async Task<IActionResult> Execute([FromBody] TableOperationRequest request)
        {
            var result = await _tableService.ExecuteAsync(request);
            return Ok(result);
        }
    }
}