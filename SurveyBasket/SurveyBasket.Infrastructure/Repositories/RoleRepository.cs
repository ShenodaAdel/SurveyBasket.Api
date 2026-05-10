using SurveyBasket.Application.Helpers;

namespace SurveyBasket.Infrastructure.Repositories
{
    public class RoleRepository(ApplicationDbContext context) : IRoleRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task AddRangeAsync(IEnumerable<IdentityRoleClaim<string>> permissions)
        {
            await _context.AddRangeAsync(permissions);
        }
        public async Task ExecuteDeleteAsync(string roleId, IEnumerable<string> removedPermissions)
        {
            await _context.RoleClaims
                    .Where(rc => rc.RoleId == roleId && removedPermissions.Contains(rc.ClaimValue!))
                    .ExecuteDeleteAsync();
        }
        public async Task<List<string>> GetAllRoles(string roleId)
        {
            return await _context.RoleClaims
                .Where(rc => rc.RoleId == roleId && rc.ClaimType == Permissions.Type && rc.ClaimValue != null)
                .Select(rc => rc.ClaimValue!)
                .ToListAsync();
        }
    }
}
