using ApiTemplate.Models;
using Microsoft.AspNetCore.Mvc;
using ApiTemplate.Attributes;
using ApiTemplate.Repository;

namespace ApiTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuth]
    public class DepartmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var repo = _unitOfWork.Repository<TblDepartment>();
                var data = await repo.GetAllAsync("tblDepartment");
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
                var repo = _unitOfWork.Repository<TblDepartment>();
                var data = await repo.GetByIdAsync("tblDepartment", "DepartmentID", id);
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
        public async Task<IActionResult> Add([FromBody] TblDepartment department)
        {
            if (department == null)
                return BadRequest("Department cannot be null");

            try
            {
                var repo = _unitOfWork.Repository<TblDepartment>();
                await repo.AddAsync("tblDepartment", department);
                _unitOfWork.Commit();
                return CreatedAtAction(nameof(GetById), new { id = department.DepartmentId }, department);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] TblDepartment department)
        {
            if (department == null || department.DepartmentId != id)
                return BadRequest();

            try
            {
                var repo = _unitOfWork.Repository<TblDepartment>();
                await repo.UpdateAsync("tblDepartment", department,"DepartmentID");
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
                var repo = _unitOfWork.Repository<TblDepartment>();
                await repo.DeleteAsync("tblDepartment", "DepartmentID", id);
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