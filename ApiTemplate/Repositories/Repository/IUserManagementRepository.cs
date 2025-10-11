using Shared.Dtos;

namespace Repositories.Repository
{
    public interface IUserManagementRepository
    {
        /// <summary>
        /// Adds a new resource with permissions and role mappings in one transaction
        /// </summary>
        Task<bool> AddResourceWithPermissionsAsync(AddResourceWithPermissionsDto dto);

        /// <summary>
        /// Assigns a role to a user
        /// </summary>
        Task<bool> AssignUserRoleAsync(AssignUserRoleDto dto);

        /// <summary>
        /// Creates a new role with optional hierarchy
        /// </summary>
        Task<bool> CreateRoleAsync(CreateRoleDto dto);

        /// <summary>
        /// Gets all resources with their permissions
        /// </summary>
        Task<object> GetAllResourcesWithPermissionsAsync();

        /// <summary>
        /// Gets all roles with their permissions
        /// </summary>
        Task<object> GetAllRolesWithPermissionsAsync();

        /// <summary>
        /// Gets user's roles and permissions
        /// </summary>
        Task<object> GetUserPermissionsAsync(long userId);
    }
}
