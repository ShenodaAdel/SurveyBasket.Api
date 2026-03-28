
namespace SurveyBasket.Application.RepositoriesInterfaces
{
    public interface IQuestionRepository
    {
        Task AddAsync(Question question);
        Task<bool> CheckIsExistAsync(string content, int pollId);
        Task<Question?> GetByIdAsync(int id);
    }
}
