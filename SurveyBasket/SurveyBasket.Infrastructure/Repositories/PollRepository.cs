namespace SurveyBasket.Infrastructure.Repositories
{
    public class PollRepository : IPollRepository
    {
        private readonly ApplicationDbContext _context;

        public PollRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Poll> GetByIdAsync (int id)
        {
            return await _context.Polls.FirstOrDefaultAsync(p => p.Id == id); 
        }
        public async Task<ApiResponseData<Poll>> GetListAsync()
        {
            var query = _context.Polls.AsQueryable();
            // make a filteration or pagaination 

            var totalRecords = await query.CountAsync();

            return new ApiResponseData<Poll>(await query.ToListAsync(), totalRecords);
        }
        public async Task AddAsync (Poll poll)
        {
            await _context.Polls.AddAsync(poll);
        }
        public async Task Update (Poll poll)
        {
             _context.Update(poll);
        } 
        public async Task Delete (Poll poll)
        {
             _context.Remove(poll);
        }
    }
}
