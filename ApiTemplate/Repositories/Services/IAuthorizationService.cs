namespace Repositories.Services
{
    public interface IAuthorizationService
    {
        Task<bool> HasPermissionAsync(long userId, string resourceName, string actionType);
    }
}
