using ApiTemplate.Dtos;
using ApiTemplate.Helper;
using ApiTemplate.Repository;
using ApiTemplate.Shared.Helper.Constants;
using Asp.Versioning;
using DBLayer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [SkipJwtValidation]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _unitOfWork.IAuthRepository.LoginAsync(loginDto);
            if (result == null)
            {
                return Unauthorized("Invalid User");
            }

            return Ok(result);
        }

        [SkipJwtValidation]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto userDto)
        {
            try
            {
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
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var repo = _unitOfWork.Repository<TblUser>();
                var data = await repo.GetByIdAsync("tblUser", "UserID", id);
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var repo = _unitOfWork.Repository<TblUser>();
            var users = await repo.GetAllAsync("tblUser");
            return Ok(users);
        }

        /// <summary>
        /// Updates user profile information.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(long id, [FromBody] TblUsersDto user)
        {
            if (user == null || id <= 0)
                return BadRequest("Invalid user data.");

            try
            {
                var repo = _unitOfWork.Repository<TblUser>();
                var existingUser = await repo.GetByIdAsync("tblUser", "UserID", id);
                if (existingUser == null)
                    return NotFound();

                // Update fields
                existingUser.Username = user.Username;
                existingUser.Firstname = user.Firstname;
                existingUser.Lastname = user.Lastname;
                existingUser.Email = user.Email;

                await repo.UpdateAsync("tblUser", existingUser, "UserID");
                _unitOfWork.Commit();
                return Ok("User profile updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Changes the password for a user.
        /// </summary>
        [HttpPost("{id}/ChangePassword")]
        public async Task<IActionResult> ChangePassword(long id, [FromBody] ChangePasswordDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("Invalid password data.");

            try
            {
                var repo = _unitOfWork.Repository<TblUser>();
                var user = await repo.GetByIdAsync("tblUser", "UserID", id);
                if (user == null)
                    return NotFound();

                // Optionally: verify old password here

                var salt = HashPassword.GenerateSalt();
                var encryptedPassword = HashPassword.Hash(dto.NewPassword, salt, "sha256");
                user.Password = Convert.ToBase64String(encryptedPassword);
                user.Salt = Convert.ToBase64String(salt);

                await repo.UpdateAsync("tblUser", user, "UserID");
                _unitOfWork.Commit();
                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deactivates a user (soft delete).
        /// </summary>
        [HttpPost("{id}/Deactivate")]
        public async Task<IActionResult> DeactivateUser(long id)
        {
            try
            {
                var repo = _unitOfWork.Repository<TblUser>();
                var user = await repo.GetByIdAsync("tblUser", "UserID", id);
                if (user == null)
                    return NotFound();

                user.Status = false;
                await repo.UpdateAsync("tblUser", user, "UserID");
                _unitOfWork.Commit();
                return Ok("User deactivated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Reactivates a user.
        /// </summary>
        [HttpPost("{id}/Reactivate")]
        public async Task<IActionResult> ReactivateUser(long id)
        {
            try
            {
                var repo = _unitOfWork.Repository<TblUser>();
                var user = await repo.GetByIdAsync("tblUser", "UserID", id);
                if (user == null)
                    return NotFound();

                user.Status = true;
                await repo.UpdateAsync("tblUser", user, "UserID");
                _unitOfWork.Commit();
                return Ok("User reactivated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a user permanently.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            try
            {
                var repo = _unitOfWork.Repository<TblUser>();
                await repo.DeleteAsync("tblUser", "UserID", id);
                _unitOfWork.Commit();
                return Ok("User deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var username = User.Identity?.Name;
            return Ok(new { Username = username });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out successfully.");
        }

        [HttpGet("TestKeyPress")]
        public async Task<IActionResult> TestKeyPress()
        {
            return Ok("Key Pressed Successfully");
        }

        // Simplify the 'using' statement for 'DirectoryEntry' and 'DirectorySearcher'
        [Authorize] // Requires Windows Authentication
        [HttpGet("ldap-samaccountname")]
        public IActionResult GetSamAccountNameFromLdap()
        {
            // Get current Windows username (DOMAIN\username)
            var windowsUsername = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(windowsUsername))
                return Unauthorized("No Windows user found.");

            // Extract username part
            var username = windowsUsername.Contains("\\") ? windowsUsername.Split('\\')[1] : windowsUsername;

            // LDAP path and domain (customize as needed)
            var ldapPath = "LDAP://your-ad-server"; // e.g., LDAP://DC=yourdomain,DC=com
            var domain = "yourdomain"; // e.g., "MYDOMAIN"

            try
            {
                using var entry = new System.DirectoryServices.DirectoryEntry(ldapPath);
                using var searcher = new DirectorySearcher(entry)
                {
                    Filter = $"(&(objectClass=user)(sAMAccountName={username}))"
                };
                searcher.PropertiesToLoad.Add("sAMAccountName");

                var result = searcher.FindOne();
                if (result != null && result.Properties["sAMAccountName"].Count > 0)
                {
                    var samAccountName = result.Properties["sAMAccountName"][0].ToString();
                    return Ok(new { samAccountName });
                }
                else
                {
                    return NotFound("sAMAccountName not found in LDAP.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"LDAP error: {ex.Message}");
            }
        }
    }
}