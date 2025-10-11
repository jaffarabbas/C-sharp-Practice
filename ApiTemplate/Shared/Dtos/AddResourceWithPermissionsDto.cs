namespace Shared.Dtos
{
    /// <summary>
    /// DTO for adding a new resource with its permissions and role mappings
    /// </summary>
    public class AddResourceWithPermissionsDto
    {
        /// <summary>
        /// Resource ID (unique identifier)
        /// </summary>
        public int ResourceId { get; set; }

        /// <summary>
        /// Resource name (e.g., "Employee", "Invoice", "Project")
        /// </summary>
        public string ResourceName { get; set; } = string.Empty;

        /// <summary>
        /// Action type IDs to create for this resource (e.g., [1, 2, 3, 4] for view, add, edit, delete)
        /// </summary>
        public List<int> ActionTypeIds { get; set; } = new List<int>();

        /// <summary>
        /// Role IDs and their permission mappings for this resource
        /// Key: RoleID, Value: List of action type IDs that role can perform
        /// Example: { 1: [1, 2, 3, 4], 2: [1], 3: [1, 4] }
        /// </summary>
        public Dictionary<int, List<int>> RolePermissions { get; set; } = new Dictionary<int, List<int>>();
    }

    /// <summary>
    /// DTO for assigning a role to a user
    /// </summary>
    public class AssignUserRoleDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Role ID to assign
        /// </summary>
        public int RoleId { get; set; }
    }

    /// <summary>
    /// DTO for creating a new role
    /// </summary>
    public class CreateRoleDto
    {
        /// <summary>
        /// Role ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Role title (e.g., "Admin", "HR", "Manager")
        /// </summary>
        public string RoleTitle { get; set; } = string.Empty;

        /// <summary>
        /// Optional: Parent role IDs for role hierarchy
        /// </summary>
        public List<int>? ParentRoleIds { get; set; }
    }
}
