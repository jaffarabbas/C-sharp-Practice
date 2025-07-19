using DBLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ApiTemplate.Attributes;
using ApiTemplate.Repository;

namespace ApiTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuth]
    public class OrganisationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrganisationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var repo = _unitOfWork.Repository<TblOrganisation>();
                var data = await repo.GetAllAsync("tblOrganisation");
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
                var repo = _unitOfWork.Repository<TblOrganisation>();
                var data = await repo.GetByIdAsync("tblOrganisation", "OrganisationID", id);
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
        public async Task<IActionResult> Add([FromBody] TblOrganisation organisation)
        {
            if (organisation == null)
                return BadRequest("Organisation cannot be null");

            try
            {
                var repo = _unitOfWork.Repository<TblOrganisation>();
                await repo.AddAsync("tblOrganisation", organisation);
                _unitOfWork.Commit();
                return CreatedAtAction(nameof(GetById), new { id = organisation.OrganisationId }, organisation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] TblOrganisation organisation)
        {
            if (organisation == null || organisation.OrganisationId != id)
                return BadRequest();

            try
            {
                var repo = _unitOfWork.Repository<TblOrganisation>();
                await repo.UpdateAsync("tblOrganisation", organisation,"OrganisationID");
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
                var repo = _unitOfWork.Repository<TblOrganisation>();
                await repo.DeleteAsync("tblOrganisation", "OrganisationID", id);
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