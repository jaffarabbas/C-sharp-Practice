using DBLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ApiTemplate.Attributes;
using ApiTemplate.Repository;

namespace ApiTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuth]
    public class CompanyUnitController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyUnitController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var repo = _unitOfWork.Repository<TblCompanyUnit>();
                var data = await repo.GetAllAsync("tblCompanyUnit");
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
                var repo = _unitOfWork.Repository<TblCompanyUnit>();
                var data = await repo.GetByIdAsync("tblCompanyUnit", "CompanyUnitID", id);
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
        public async Task<IActionResult> Add([FromBody] TblCompanyUnit companyUnit)
        {
            if (companyUnit == null)
                return BadRequest("Company Unit cannot be null");

            try
            {
                var repo = _unitOfWork.Repository<TblCompanyUnit>();
                await repo.AddAsync("tblCompanyUnit", companyUnit);
                _unitOfWork.Commit();
                return CreatedAtAction(nameof(GetById), new { id = companyUnit.CompanyUnitId }, companyUnit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] TblCompanyUnit companyUnit)
        {
            if (companyUnit == null || companyUnit.CompanyUnitId != id)
                return BadRequest();

            try
            {
                var repo = _unitOfWork.Repository<TblCompanyUnit>();
                await repo.UpdateAsync("tblCompanyUnit", companyUnit,"CompanyUnitID");
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
                var repo = _unitOfWork.Repository<TblCompanyUnit>();
                await repo.DeleteAsync("tblCompanyUnit", "CompanyUnitID", id);
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