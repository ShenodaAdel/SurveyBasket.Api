namespace SurveyBasket.Infrastructure.Repositories
{
    partial class VoteRepository(ApplicationDbContext context) : IVoteRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<bool> CheckUserVoted(string userId , int pollId)
        {
            return await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId);
        }

        public async Task AddAsync(Vote vote)
        {
            await _context.Votes.AddAsync(vote);
        }
    }
}
