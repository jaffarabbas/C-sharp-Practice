using ApiTemplate.Dtos;
using ApiTemplate.Repository;
using ApiTemplate.Shared.Helper.Constants;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shared.Dtos;
using System.DirectoryServices;

namespace ApiTemplate.Controllers
{
    [ApiVersion(ApiVersioningConstants.CurrentVersion)]
    [Route(ApiVersioningConstants.versionRoute)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Authenticates a user and returns a token.
        /// </summary>
        [SkipJwtValidation]
        [EnableRateLimiting("PerIPPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _unitOfWork.IAuthRepository.LoginAsync(loginDto);
                if (result == null)
                {
                    return Unauthorized("Invalid credentials or inactive account");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Login failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        [SkipJwtValidation]
        [EnableRateLimiting("PerIPPolicy")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var response = await _unitOfWork.IAuthRepository.RegisterAsync(userDto);
                _unitOfWork.Commit();
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.Rollback();
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Registration failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Changes the password for the current authenticated user.
        /// </summary>
        [Authorize]
        [EnableRateLimiting("FixedPolicy")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!long.TryParse(userIdClaim, out long userId))
                    return Unauthorized("Invalid user session.");

                var result = await _unitOfWork.IAuthRepository.ChangePasswordAsync(userId, dto);
                if (!result)
                    return NotFound("User not found.");

                _unitOfWork.Commit();
                return Ok("Password changed successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _unitOfWork.Rollback();
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Error changing password: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok("Logged out successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Logout failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Test endpoint for authentication.
        /// </summary>
        [Authorize]
        [HttpGet("test")]
        public IActionResult TestAuth()
        {
            return Ok(new 
            { 
                Message = "Authentication successful",
                User = User.Identity?.Name,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}