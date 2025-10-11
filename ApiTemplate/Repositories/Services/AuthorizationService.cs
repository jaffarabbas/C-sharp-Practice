using DBLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly TestContext _context;

        public AuthorizationService(TestContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPermissionAsync(long userId, string resourceName, string actionType)
        {
            try
            {
                // Query to check if user has the required permission
                var hasPermission = await _context.TblUsers
                    .Where(u => u.Userid == userId)
                    .Join(_context.TblUserRoles,
                        u => u.Userid,
                        ur => ur.UserId,
                        (u, ur) => new { User = u, UserRole = ur })
                    .Join(_context.TblRoles,
                        x => x.UserRole.RoleId,
                        r => r.RoleId,
                        (x, r) => new { x.User, x.UserRole, Role = r })
                    .Join(_context.TblRolePermissions,
                        x => x.Role.RoleId,
                        rp => rp.RoleId,
                        (x, rp) => new { x.User, x.UserRole, x.Role, RolePermission = rp })
                    .Join(_context.TblPermissions,
                        x => x.RolePermission.PermissionId,
                        p => p.PermissionId,
                        (x, p) => new { x.User, x.UserRole, x.Role, x.RolePermission, Permission = p })
                    .Join(_context.TblResources,
                        x => x.Permission.ResourceId,
                        res => res.ResourceId,
                        (x, res) => new { x.User, x.UserRole, x.Role, x.RolePermission, x.Permission, Resource = res })
                    .Join(_context.TblActionTypes,
                        x => x.Permission.ActionTypeId,
                        at => at.ActionTypeId,
                        (x, at) => new { x.Resource, ActionType = at, x.UserRole, x.RolePermission, x.Permission })
                    .Where(x =>
                        x.Resource.ResourceName.ToLower() == resourceName.ToLower() &&
                        x.ActionType.ActionTypeTitle.ToLower() == actionType.ToLower() &&
                        x.UserRole.UserRoleIsActive == true &&
                        x.RolePermission.RolePermissionIsActive == true &&
                        x.Permission.PermissionIsActive == true &&
                        x.Resource.ResourceIsActive == true &&
                        x.ActionType.ActionTypeIsActive == true)
                    .AnyAsync();

                return hasPermission;
            }
            catch (Exception)
            {
                // Log the exception if needed
                return false;
            }
        }
    }
}
