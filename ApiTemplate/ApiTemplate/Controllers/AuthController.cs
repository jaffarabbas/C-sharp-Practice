using ApiTemplate.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ApiTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SkipJwtValidation]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Login()
        {
            return Ok("test"); // Replace with actual token generation
        }
    }
}