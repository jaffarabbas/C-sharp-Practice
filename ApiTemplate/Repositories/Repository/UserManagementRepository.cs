using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Attributes;
using Shared.Dtos;

namespace Repositories.Repository
{
    [AutoRegisterRepository(typeof(IUserManagementRepository))]
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TestContext _context;

        public UserManagementRepository(IUnitOfWork unitOfWork, TestContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<bool> AddResourceWithPermissionsAsync(AddResourceWithPermissionsDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Check if resource already exists using UnitOfWork
                var existingResource = await _unitOfWork.Repository<TblResource>().GetByIdAsync(dto.ResourceId);
                if (existingResource != null)
                {
                    throw new ArgumentException($"Resource with ID {dto.ResourceId} already exists");
                }

                // 2. Add the resource using UnitOfWork
                var resource = new TblResource
                {
                    ResourceId = dto.ResourceId,
                    ResourceName = dto.ResourceName,
                    ResourceIsActive = true,
                    ResourceCreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<TblResource>().AddAsync(resource);
                await _unitOfWork.Repository<TblResource>().SaveAsync();

                // 3. Validate action type IDs exist using UnitOfWork
                var allActionTypes = await _unitOfWork.Repository<TblActionType>().GetAllAsync();
                var actionTypes = allActionTypes
                    .Where(at => dto.ActionTypeIds.Contains(at.ActionTypeId))
                    .ToList();

                if (actionTypes.Count != dto.ActionTypeIds.Count)
                {
                    throw new ArgumentException("One or more action type IDs do not exist in the database");
                }

                // 4. Create permissions (Resource + ActionType combinations) using UnitOfWork
                long permissionIdStart = await _unitOfWork.Repository<TblPermission>().GetMaxID("tblPermission", "PermissionID") + 1;

                foreach (var actionTypeId in dto.ActionTypeIds)
                {
                    var permission = new TblPermission
                    {
                        PermissionId = Convert.ToInt32(permissionIdStart++),
                        ResourceId = dto.ResourceId,
                        ActionTypeId = actionTypeId,
                        PermissionIsActive = true,
                        PermissionCreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Repository<TblPermission>().AddAsync(permission);
                }
                await _unitOfWork.Repository<TblPermission>().SaveAsync();

                // 5. Map permissions to roles (RolePermission) using UnitOfWork
                long rolePermissionIdStart = await _unitOfWork.Repository<TblRolePermission>().GetMaxID("tblRolePermission", "RolePermissionID") + 1;

                // Get permissions we just created
                var allPermissions = await _unitOfWork.Repository<TblPermission>().GetAllAsync();
                var createdPermissions = allPermissions
                    .Where(p => p.ResourceId == dto.ResourceId)
                    .ToList();

                foreach (var rolePermission in dto.RolePermissions)
                {
                    int roleId = rolePermission.Key;
                    var allowedActionTypeIds = rolePermission.Value;

                    // Verify role exists using UnitOfWork
                    var roleExists = await _unitOfWork.Repository<TblRole>().GetByIdAsync(roleId);
                    if (roleExists == null)
                    {
                        throw new ArgumentException($"Role with ID {roleId} does not exist");
                    }

                    // Get permissions for this role's allowed action type IDs
                    var permissionsForRole = createdPermissions
                        .Where(p => allowedActionTypeIds.Contains(p.ActionTypeId))
                        .ToList();

                    foreach (var permission in permissionsForRole)
                    {
                        var rolePermissionEntry = new TblRolePermission
                        {
                            RolePermissionId = Convert.ToInt32(rolePermissionIdStart++),
                            RoleId = roleId,
                            PermissionId = permission.PermissionId,
                            RolePermissionIsActive = true,
                            RolePermissionCreatedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.Repository<TblRolePermission>().AddAsync(rolePermissionEntry);
                    }
                }
                await _unitOfWork.Repository<TblRolePermission>().SaveAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> AssignUserRoleAsync(AssignUserRoleDto dto)
        {
            // Check if user exists using UnitOfWork
            var userExists = await _unitOfWork.Repository<TblUser>().GetByIdAsync(dto.UserId);
            if (userExists == null)
            {
                throw new ArgumentException($"User with ID {dto.UserId} does not exist");
            }

            // Check if role exists using UnitOfWork
            var roleExists = await _unitOfWork.Repository<TblRole>().GetByIdAsync(dto.RoleId);
            if (roleExists == null)
            {
                throw new ArgumentException($"Role with ID {dto.RoleId} does not exist");
            }

            // Check if user already has this role
            var allUserRoles = await _unitOfWork.Repository<TblUserRole>().GetAllAsync();
            var existingUserRole = allUserRoles
                .FirstOrDefault(ur => ur.UserId == dto.UserId && ur.RoleId == dto.RoleId);

            if (existingUserRole != null)
            {
                if (existingUserRole.UserRoleIsActive)
                {
                    throw new ArgumentException("User already has this role assigned");
                }
                else
                {
                    // Reactivate the role using UnitOfWork
                    existingUserRole.UserRoleIsActive = true;
                    existingUserRole.UserRoleCreatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<TblUserRole>().Update(existingUserRole);
                    await _unitOfWork.Repository<TblUserRole>().SaveAsync();
                    return true;
                }
            }

            // Create new user role assignment using UnitOfWork
            var userRoleId = await _unitOfWork.Repository<TblUserRole>().GetMaxID("tblUserRole", "UserRoleID") + 1;
            var userRole = new TblUserRole
            {
                UserRoleId = Convert.ToInt32(userRoleId),
                UserId = dto.UserId,
                RoleId = dto.RoleId,
                UserRoleIsActive = true,
                UserRoleCreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<TblUserRole>().AddAsync(userRole);
            await _unitOfWork.Repository<TblUserRole>().SaveAsync();
            return true;
        }

        public async Task<bool> CreateRoleAsync(CreateRoleDto dto)
        {
            // Check if role already exists using UnitOfWork
            var existingRole = await _unitOfWork.Repository<TblRole>().GetByIdAsync(dto.RoleId);
            if (existingRole != null)
            {
                throw new ArgumentException($"Role with ID {dto.RoleId} already exists");
            }

            var role = new TblRole
            {
                RoleId = dto.RoleId,
                RoleTitle = dto.RoleTitle,
                RoleIsActive = true,
                RoleCreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<TblRole>().AddAsync(role);
            await _unitOfWork.Repository<TblRole>().SaveAsync();

            // Add role hierarchy if parent roles are specified
            // Note: Role hierarchy requires EF Core navigation properties, so we still use context here
            if (dto.ParentRoleIds != null && dto.ParentRoleIds.Any())
            {
                var createdRole = await _context.TblRoles.FindAsync(dto.RoleId);
                if (createdRole != null)
                {
                    foreach (var parentRoleId in dto.ParentRoleIds)
                    {
                        var parentRole = await _context.TblRoles.FindAsync(parentRoleId);
                        if (parentRole != null)
                        {
                            createdRole.ParentRoles.Add(parentRole);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<object> GetAllResourcesWithPermissionsAsync()
        {
            var resources = await _context.TblResources
                .Where(r => r.ResourceIsActive)
                .Select(r => new
                {
                    r.ResourceId,
                    r.ResourceName,
                    Permissions = r.TblPermissions
                        .Where(p => p.PermissionIsActive)
                        .Select(p => new
                        {
                            p.PermissionId,
                            ActionType = p.ActionType.ActionTypeTitle,
                            p.ActionTypeId
                        })
                        .ToList()
                })
                .ToListAsync();

            return resources;
        }

        public async Task<object> GetAllRolesWithPermissionsAsync()
        {
            var roles = await _context.TblRoles
                .Where(r => r.RoleIsActive)
                .Select(r => new
                {
                    r.RoleId,
                    r.RoleTitle,
                    Permissions = r.TblRolePermissions
                        .Where(rp => rp.RolePermissionIsActive && rp.Permission.PermissionIsActive)
                        .Select(rp => new
                        {
                            rp.PermissionId,
                            Resource = rp.Permission.Resource.ResourceName,
                            ActionType = rp.Permission.ActionType.ActionTypeTitle
                        })
                        .ToList()
                })
                .ToListAsync();

            return roles;
        }

        public async Task<object> GetUserPermissionsAsync(long userId)
        {
            var userPermissions = await _context.TblUsers
                .Where(u => u.Userid == userId)
                .Select(u => new
                {
                    u.Userid,
                    u.Username,
                    u.Email,
                    Roles = u.TblUserRoles
                        .Where(ur => ur.UserRoleIsActive)
                        .Select(ur => new
                        {
                            ur.RoleId,
                            RoleTitle = ur.Role.RoleTitle,
                            Permissions = ur.Role.TblRolePermissions
                                .Where(rp => rp.RolePermissionIsActive && rp.Permission.PermissionIsActive)
                                .Select(rp => new
                                {
                                    Resource = rp.Permission.Resource.ResourceName,
                                    ActionType = rp.Permission.ActionType.ActionTypeTitle
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (userPermissions == null)
            {
                return new { Message = "User not found" };
            }
            return userPermissions;
        }

    }
}
