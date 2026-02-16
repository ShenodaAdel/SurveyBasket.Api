using Microsoft.EntityFrameworkCore;

namespace SurveyBasket.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        // Define DbSets for your entities here
        
    }
}
