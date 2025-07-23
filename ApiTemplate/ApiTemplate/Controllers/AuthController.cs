using ApiTemplate.Dtos;
using ApiTemplate.Helper;
using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;

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

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var data = await _unitOfWork.IAuthRepository.Login(loginDto);
            if (data == null)
            {
                return Unauthorized("Invalid User");
            }
            return Ok(data);
        }

        [HttpPost("")]
        public async Task<IActionResult> Register([FromBody] TblUsersDto user)
        {
            if (user == null)
                return BadRequest("User cannot be null");

            try
            {
                var repo = _unitOfWork.Repository<TblUser>();
                // Hashing password
                var salt = HashPassword.GenerateSalt();
                var encryptedPassword = HashPassword.Hash(user.Password, salt, "sha256");
                var model = new TblUser
                {
                    Userid = await repo.GetMaxID("tblUsers", "Userid"),
                    Username = user.Username,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email,
                    Password = Convert.ToBase64String(encryptedPassword),
                    Salt = Convert.ToBase64String(salt),
                    AccountType = user.AccountType,
                    CreatedAt = DateTime.UtcNow,
                    Status = true,
                };
                await repo.AddAsync("tblUsers", model);
                _unitOfWork.Commit();
                return Ok(new
                {
                    Userid = model.Userid,
                    UserCreatedDate = DateTime.UtcNow,
                    Message = "User Created Successfully"
                });
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
                existingUser.AccountType = user.AccountType;

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
    }
}