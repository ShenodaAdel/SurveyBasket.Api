namespace SurveyBasket.Infrastructure.Repositories
{
    public class QuestionRepository(ApplicationDbContext context) : IQuestionRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions.FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<bool> CheckIsExistAsync(string content , int pollId)
        {
            return await _context.Questions.AnyAsync(q => q.Content == content && q.PollId == pollId);
        }
        public async Task AddAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
        }
    }
}
