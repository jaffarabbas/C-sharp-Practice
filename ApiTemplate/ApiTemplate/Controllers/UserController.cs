using ApiTemplate.Dtos;
using ApiTemplate.Helper.Enum;
using ApiTemplate.Repository;
using ApiTemplate.Shared.Helper.Constants;
using Asp.Versioning;
using DBLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shared.Dtos;

namespace ApiTemplate.Controllers
{
    [ApiVersion(ApiVersioningConstants.CurrentVersion)]
    [Route(ApiVersioningConstants.versionRoute)]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets a user by ID. (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("SlidingPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid user ID.");

                var repo = _unitOfWork.Repository<TblUser>();
                var user = await repo.GetByIdAsync("tblUsers", "Userid", id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                // Remove sensitive information before returning
                var userDto = new TblUsersDto
                {
                    Username = user.Username,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email,
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all users with pagination. (Admin only)
        /// </summary>
        [Authorize(Roles = "admin")]
        [EnableRateLimiting("SlidingPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                if (page <= 0) page = 1;
                if (pageSize <= 0 || pageSize > 100) pageSize = 50;

                var repo = _unitOfWork.Repository<TblUser>();
                var users = await repo.GetPagedAsync("tblUsers", page, pageSize, "Username ASC");

                // Remove sensitive information
                var userDtos = users.Select(u => new TblUsersDto
                {
                    Username = u.Username,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    Email = u.Email,
                }).ToList();

                return Ok(new 
                {
                    Users = userDtos,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalUsers = userDtos.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving users: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates user profile information.
        /// Users can update their own profile, admins can update any profile.
        /// </summary>
        [EnableRateLimiting("FixedPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(long id, [FromBody] TblUsersDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (userDto == null || id <= 0)
                    return BadRequest("Invalid user data.");

                // Check if user is trying to update their own profile or if they're an admin
                var currentUserId = User.FindFirst("userId")?.Value;
                var isAdmin = User.IsInRole("Admin");
                
                if (!isAdmin && currentUserId != id.ToString())
                    return Forbid("You can only update your own profile.");

                var repo = _unitOfWork.Repository<TblUser>();
                var existingUser = await repo.GetByIdAsync("tblUsers", "Userid", id);
                if (existingUser == null)
                    return NotFound($"User with ID {id} not found.");

                // Update allowed fields
                existingUser.Firstname = userDto.Firstname;
                existingUser.Lastname = userDto.Lastname;
                existingUser.Email = userDto.Email;

                // Only admins can update username and status
                if (isAdmin)
                {
                    existingUser.Username = userDto.Username;
                }

                await repo.UpdateAsync("tblUsers", existingUser, "Userid");
                _unitOfWork.Commit();
                return Ok("User profile updated successfully.");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Error updating profile: {ex.Message}");
            }
        }

        /// <summary>
        /// Deactivates a user (soft delete). Admin only.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("FixedPolicy")]
        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid user ID.");

                var repo = _unitOfWork.Repository<TblUser>();
                var user = await repo.GetByIdAsync("tblUsers", "Userid", id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                if (!user.Status)
                    return BadRequest("User is already deactivated.");

                user.Status = false;
                await repo.UpdateAsync("tblUsers", user, "Userid");
                _unitOfWork.Commit();
                
                return Ok($"User {user.Username} deactivated successfully.");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Error deactivating user: {ex.Message}");
            }
        }

        /// <summary>
        /// Reactivates a user. Admin only.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("FixedPolicy")]
        [HttpPost("{id}/reactivate")]
        public async Task<IActionResult> ReactivateUser(long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid user ID.");

                var repo = _unitOfWork.Repository<TblUser>();
                var user = await repo.GetByIdAsync("tblUsers", "Userid", id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                if (user.Status)
                    return BadRequest("User is already active.");

                user.Status = true;
                await repo.UpdateAsync("tblUsers", user, "Userid");
                _unitOfWork.Commit();
                
                return Ok($"User {user.Username} reactivated successfully.");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Error reactivating user: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a user permanently. Admin only.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("FixedPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid user ID.");

                // Prevent self-deletion
                var currentUserId = User.FindFirst("userId")?.Value;
                if (currentUserId == id.ToString())
                    return BadRequest("You cannot delete your own account.");

                var repo = _unitOfWork.Repository<TblUser>();
                var userRoleRepo = _unitOfWork.Repository<TblUserRole>();
                
                var user = await repo.GetByIdAsync("tblUsers", "Userid", id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                // Delete user roles first
                var userRoles = await userRoleRepo.GetAllAsync("tblUserRole");
                var userRolesToDelete = userRoles.Where(ur => ur.UserId == id);
                
                foreach (var role in userRolesToDelete)
                {
                    await userRoleRepo.DeleteAsync("tblUserRole", "UserRoleId", role.UserRoleId);
                }

                // Delete user
                await repo.DeleteAsync("tblUsers", "Userid", id);
                _unitOfWork.Commit();
                
                return Ok($"User {user.Username} deleted permanently.");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Error deleting user: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets current user's own profile information.
        /// </summary>
        [EnableRateLimiting("PerUserPolicy")]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!long.TryParse(userIdClaim, out long userId))
                    return Unauthorized("Invalid user session.");

                var repo = _unitOfWork.Repository<TblUser>();
                var user = await repo.GetByIdAsync("tblUsers", "Userid", userId);
                if (user == null)
                    return NotFound("User profile not found.");

                var userDto = new TblUsersDto
                {
                    Username = user.Username,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving profile: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates current user's own profile.
        /// </summary>
        [EnableRateLimiting("FixedPolicy")]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] TblUsersDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (userDto == null)
                    return BadRequest("Invalid user data.");

                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!long.TryParse(userIdClaim, out long userId))
                    return Unauthorized("Invalid user session.");

                var repo = _unitOfWork.Repository<TblUser>();
                var existingUser = await repo.GetByIdAsync("tblUsers", "Userid", userId);
                if (existingUser == null)
                    return NotFound("User profile not found.");

                // Users can only update their personal information, not username or status
                existingUser.Firstname = userDto.Firstname;
                existingUser.Lastname = userDto.Lastname;
                existingUser.Email = userDto.Email;

                await repo.UpdateAsync("tblUsers", existingUser, "Userid");
                _unitOfWork.Commit();
                
                return Ok("Your profile has been updated successfully.");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Error updating profile: {ex.Message}");
            }
        }
    }
}