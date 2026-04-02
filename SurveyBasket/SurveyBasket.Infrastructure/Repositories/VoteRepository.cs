using SurveyBasket.Application.Services.Result.Dtos;

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

        // DashBoard
        public async Task<IEnumerable<VotesPerDayResponse>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
        {
            return await _context.Votes
                .Where(v => v.PollId == pollId)
                .GroupBy(v => new { Date = DateOnly.FromDateTime(v.SubmittedOn) })
                .Select(g => new VotesPerDayResponse(
                    g.Key.Date,
                    g.Count()
                    )).ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<VotesPerQuestionResponse>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
        {
            return await _context.Questions
                .Where(q => q.PollId == pollId)
                .Select(q => new VotesPerQuestionResponse(
                    q.Content,
                    q.VoteAnswers
                        .GroupBy(va => va.Answer.Content)
                        .Select(g => new VotesPerAnswerResponse(g.Key, g.Count()))
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
