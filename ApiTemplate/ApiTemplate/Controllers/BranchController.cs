using ApiTemplate.Models;
using Microsoft.AspNetCore.Mvc;
using TestApi.Attributes;
using TestApi.Repository;

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuth]
    public class BranchController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BranchController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var repo = _unitOfWork.Repository<TblBranch>();
                var data = await repo.GetAllAsync("tblBranch");
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
                var repo = _unitOfWork.Repository<TblBranch>();
                var data = await repo.GetByIdAsync("tblBranch", "BranchID", id);
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
        public async Task<IActionResult> Add([FromBody] TblBranch branch)
        {
            if (branch == null)
                return BadRequest("Branch cannot be null");

            try
            {
                var repo = _unitOfWork.Repository<TblBranch>();
                await repo.AddAsync("tblBranch", branch);
                _unitOfWork.Commit();
                return CreatedAtAction(nameof(GetById), new { id = branch.BranchId }, branch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] TblBranch branch)
        {
            if (branch == null || branch.BranchId != id)
                return BadRequest();

            try
            {
                var repo = _unitOfWork.Repository<TblBranch>();
                await repo.UpdateAsync("tblBranch", branch,"BranchID");
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
                var repo = _unitOfWork.Repository<TblBranch>();
                await repo.DeleteAsync("tblBranch", "BranchID", id);
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