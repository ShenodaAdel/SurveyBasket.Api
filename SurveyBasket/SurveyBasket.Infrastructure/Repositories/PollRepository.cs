using Mapster;
using SurveyBasket.Application.Services.PollService.Dto;
using SurveyBasket.Application.Services.Result.Dtos;

namespace SurveyBasket.Infrastructure.Repositories
{
    public class PollRepository : IPollRepository
    {
        private readonly ApplicationDbContext _context;

        public PollRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Poll?> GetByIdAsync(int id)
        {
            return await _context.Polls.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ApiResponseData<Poll>> GetListAsync()
        {
            var query = _context.Polls.AsQueryable();

            var totalRecords = await query.CountAsync();

            return new ApiResponseData<Poll>(await query.ToListAsync(), totalRecords);
        }
        public async Task<ApiResponseData<PollResponse>> GetCurrenrtListAsync()
        {
            var query = _context.Polls
                .Where(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .ProjectToType<PollResponse>()
                .AsQueryable();

            var totalRecords = await query.CountAsync();

            return new ApiResponseData<PollResponse>(await query.ToListAsync(), totalRecords);
        }

        public async Task AddAsync(Poll poll)
        {
            await _context.Polls.AddAsync(poll);
        }

        public async Task Update(Poll poll)
        {
            _context.Polls.Update(poll);
        }

        public Task Delete(Poll poll)
        {
            _context.Remove(poll);
            return Task.CompletedTask;
        }

        public async Task<bool> CheckIsActiveAsync(int id)
        {
            return await _context.Polls.AnyAsync(
               p => p.Id == id
                && p.IsPublished
                && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow));
        }
        public async Task<bool> CheckTitleAsync(string title)
        {
            return await _context.Polls.AnyAsync(p => p.Title == title);
        }
        public async Task<bool> CheckTitleAndNotTheSamePollAsync(string title, int id)
        {
            return await _context.Polls.AnyAsync(p => p.Title == title && p.Id != id);
        }



        // For DashBoard 
        public async Task<PollVoteResponse?> GetPollVoteResponseAsync(int pollId, CancellationToken cancellationToken = default)
        {
            return await _context.Polls
                .Where(p => p.Id == pollId)
                .Select(p => new PollVoteResponse(p.Title,
                        p.Votes.Select(v => new VoteResponse($"{v.User.FirstName} {v.User.LastName}", v.SubmittedOn,
                        v.VoteAnswers.Select(a => new QuestionAnswerResponse(a.Question.Content, a.Answer.Content))
                        ))
                )).SingleOrDefaultAsync(cancellationToken);
        }
        public async Task<List<Poll>> GetAllIsPublished()
        {
            return await _context.Polls
                .Where(p => p.IsPublished && p.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Poll?> GetById(int pollId)
        {
            return await _context.Polls.SingleOrDefaultAsync(p => p.Id == pollId && p.IsPublished);
        }
    }
}
