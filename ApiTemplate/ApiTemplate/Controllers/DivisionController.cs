using DBLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ApiTemplate.Attributes;
using ApiTemplate.Repository;
using ApiTemplate.Shared.Helper.Constants;

namespace ApiTemplate.Controllers
{
    [ApiVersion(ApiVersioningConstants.CurrentVersion)]
    [Route(ApiVersioningConstants.versionRoute)]
    [ApiController]
    [CustomAuth]
    public class DivisionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DivisionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var repo = _unitOfWork.Repository<TblDivision>();
                var data = await repo.GetAllAsync("tblDivision");
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var repo = _unitOfWork.Repository<TblDivision>();
                var data = await repo.GetByIdAsync("tblDivision", "DivisionID", id);
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TblDivision division)
        {
            if (division == null)
                return BadRequest("Division cannot be null");

            try
            {
                var repo = _unitOfWork.Repository<TblDivision>();
                await repo.AddAsync("tblDivision", division);
                _unitOfWork.Commit();
                return CreatedAtAction(nameof(GetById), new { id = division.DivisionId }, division);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] TblDivision division)
        {
            if (division == null || division.DivisionId != id)
                return BadRequest();

            try
            {
                var repo = _unitOfWork.Repository<TblDivision>();
                await repo.UpdateAsync("tblDivision", division,"DivisionID");
                _unitOfWork.Commit();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var repo = _unitOfWork.Repository<TblDivision>();
                await repo.DeleteAsync("tblDivision", "DivisionID", id);
                _unitOfWork.Commit();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}