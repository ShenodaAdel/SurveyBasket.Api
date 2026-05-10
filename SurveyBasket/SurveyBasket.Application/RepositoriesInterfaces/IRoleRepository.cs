using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Application.RepositoriesInterfaces
{
    public interface IRoleRepository
    {
        Task AddRangeAsync(IEnumerable<IdentityRoleClaim<string>> permissions);
        Task ExecuteDeleteAsync(string roleId, IEnumerable<string> removedPermissions);
        Task<List<string>> GetAllRoles(string roleId);
    }
}
