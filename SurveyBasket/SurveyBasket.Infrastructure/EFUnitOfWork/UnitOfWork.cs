namespace SurveyBasket.Infrastructure.EFUnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            PollRepository = new PollRepository(_context);
        }
        public IPollRepository PollRepository { get; set; }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
