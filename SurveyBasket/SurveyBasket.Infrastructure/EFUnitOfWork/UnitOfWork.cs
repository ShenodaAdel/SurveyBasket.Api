using Microsoft.AspNetCore.Identity;
using SurveyBasket.Domain.Entities;

namespace SurveyBasket.Infrastructure.EFUnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            PollRepository = new PollRepository(_context);
            UserRepository = new UserRepository(_userManager);
        }
        public IPollRepository PollRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
