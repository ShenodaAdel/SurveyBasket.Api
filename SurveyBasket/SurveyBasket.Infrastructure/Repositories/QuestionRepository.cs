using Mapster;
using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Question.Dtos;
using System.Linq.Dynamic.Core;

namespace SurveyBasket.Infrastructure.Repositories
{
    public class QuestionRepository(ApplicationDbContext context) : IQuestionRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions.FirstOrDefaultAsync(q => q.Id == id);
        }
        public async Task<QuestionResponse?> GetByPollIdAsync(int pollId, int id)
        {
            return await _context.Questions.Where(q => q.PollId == pollId && q.Id == id)
                 .AsNoTracking()
                 .ProjectToType<QuestionResponse>()
                 .SingleOrDefaultAsync();

        }
        public async Task<Question?> GetByPollIdAndIdAsync(int pollId, int id)
        {
            return await _context.Questions
                 .Include(q => q.Answers)
                 .Where(q => q.PollId == pollId && q.Id == id)
                 .SingleOrDefaultAsync();

        }
        public async Task<bool> CheckIsExistAsync(string content, int pollId)
        {
            return await _context.Questions.AnyAsync(q => q.Content == content && q.PollId == pollId);
        }
        public async Task<bool> CheckIsExistWithSameContentBYPollIdAsync(string content, int pollId, int id)
        {
            return await _context.Questions.AnyAsync(
                q => q.PollId == pollId
                && q.Id != id
                && q.Content == content);
        }
        public async Task<List<int>> GetActiveQuestionIds(int pollId)
        {
            return await _context.Questions
                .Where(q => q.PollId == pollId && !q.IsDeleted)
                .Select(q => q.Id)
                .ToListAsync();
        }
        public async Task AddAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
        }
        public IQueryable<QuestionResponse> GetListByPollId(int pollId, RequestFillters fillters)
        {
            var query = _context.Questions
                .Where(q => q.PollId == pollId && !q.IsDeleted);

            if (!string.IsNullOrEmpty(fillters.SearchValue))
            {
                query.Where(q => q.Content.Contains(fillters.SearchValue));
            }

            if (!string.IsNullOrEmpty(fillters.SortColumn))
            {
                query.OrderBy($"{fillters.SortColumn} {fillters.SortDirection}");
            }

            var source = query
                .Include(q => q.Answers)
                .ProjectToType<QuestionResponse>()
                .AsNoTracking();
            return source;
        }
        public void Update(Question question)
        {
            _context.Questions.Update(question);
        }
    }
}
