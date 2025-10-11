using ApiTemplate.Repository;
using ApiTemplate.Shared.Helper.Constants;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Attributes;
using Repositories.Repository;
using Shared.Dtos;

namespace ApiTemplate.Controllers
{
    [ApiVersion(ApiVersioningConstants.CurrentVersion)]
    [Route(ApiVersioningConstants.versionRoute)]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserManagerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adds a new resource with all its permissions and role mappings in one API call.
        /// This is a scalable approach to manage the entire permission chain.
        /// </summary>
        /// <remarks>
        /// Example request:
        ///
        ///     POST /api/v1/usermanager/resources
        ///     {
        ///         "resourceName": "Salary",
        ///         "actionTypeIds": [1, 2, 3, 4],
        ///         "rolePermissions": {
        ///             "1": [1, 2, 3, 4],
        ///             "2": [1],
        ///             "3": [1, 4]
        ///         }
        ///     }
        ///
        /// Where ActionTypeIds: 1=view, 2=add, 3=delete, 4=edit
        ///
        /// This creates:
        /// - Resource "Salary"
        /// - 4 Permissions (Salary+view, Salary+add, Salary+delete, Salary+edit)
        /// - Maps to roles: Admin (1) gets all, General (2) gets view only, HR (3) gets view and edit
        /// </remarks>
        [HttpPost("resources")]
        [RequirePermission("UserManagement", "add")]
        public async Task<IActionResult> AddResourceWithPermissions([FromBody] AddResourceWithPermissionsDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var repo = _unitOfWork.GetRepository<IUserManagementRepository>();
                var result = await repo.AddResourceWithPermissionsAsync(dto);

                return Ok(new
                {
                    Message = "Resource with permissions created successfully",
                    ResourceId = dto.ResourceId,
                    ResourceName = dto.ResourceName,
                    PermissionsCreated = dto.ActionTypeIds.Count,
                    RoleMappingsCreated = dto.RolePermissions.Sum(rp => rp.Value.Count),
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message, Timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Failed to create resource: {ex.Message}", Timestamp = DateTime.UtcNow });
            }
        }

        /// <summary>
        /// Assigns a role to a user
        /// </summary>
        [HttpPost("users/assign-role")]
        [RequirePermission("UserManagement", "edit")]
        public async Task<IActionResult> AssignUserRole([FromBody] AssignUserRoleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var repo = _unitOfWork.GetRepository<IUserManagementRepository>();
                var result = await repo.AssignUserRoleAsync(dto);

                return Ok(new
                {
                    Message = "Role assigned to user successfully",
                    UserId = dto.UserId,
                    RoleId = dto.RoleId,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message, Timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Failed to assign role: {ex.Message}", Timestamp = DateTime.UtcNow });
            }
        }

        /// <summary>
        /// Creates a new role with optional hierarchy
        /// </summary>
        [HttpPost("roles")]
        [RequirePermission("UserManagement", "add")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var repo = _unitOfWork.GetRepository<IUserManagementRepository>();
                var result = await repo.CreateRoleAsync(dto);

                return Ok(new
                {
                    Message = "Role created successfully",
                    RoleId = dto.RoleId,
                    RoleTitle = dto.RoleTitle,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message, Timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Failed to create role: {ex.Message}", Timestamp = DateTime.UtcNow });
            }
        }

        /// <summary>
        /// Gets all resources with their permissions
        /// </summary>
        [HttpGet("resources")]
        [RequirePermission("UserManagement", "view")]
        public async Task<IActionResult> GetAllResourcesWithPermissions()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IUserManagementRepository>();
                var result = await repo.GetAllResourcesWithPermissionsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Failed to retrieve resources: {ex.Message}", Timestamp = DateTime.UtcNow });
            }
        }

        /// <summary>
        /// Gets all roles with their permissions
        /// </summary>
        [HttpGet("roles")]
        [RequirePermission("UserManagement", "view")]
        public async Task<IActionResult> GetAllRolesWithPermissions()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IUserManagementRepository>();
                var result = await repo.GetAllRolesWithPermissionsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Failed to retrieve roles: {ex.Message}", Timestamp = DateTime.UtcNow });
            }
        }

        /// <summary>
        /// Gets a user's roles and all their permissions
        /// </summary>
        [HttpGet("users/{userId}/permissions")]
        [RequirePermission("UserManagement", "view")]
        public async Task<IActionResult> GetUserPermissions(long userId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IUserManagementRepository>();
                var result = await repo.GetUserPermissionsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Failed to retrieve user permissions: {ex.Message}", Timestamp = DateTime.UtcNow });
            }
        }
    }
}
