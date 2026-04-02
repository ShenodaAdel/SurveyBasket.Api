

using SurveyBasket.Application.Services.Question.Dtos;

namespace SurveyBasket.Application.RepositoriesInterfaces
{
    public interface IQuestionRepository
    {
        Task AddAsync(Question question);
        Task<bool> CheckIsExistAsync(string content, int pollId);
        Task<bool> CheckIsExistWithSameContentBYPollIdAsync(string content, int pollId, int id);
        Task<List<int>> GetActiveQuestionIds(int pollId);
        Task<Question?> GetByIdAsync(int id);
        Task<Question?> GetByPollIdAndIdAsync(int pollId, int id);
        Task<QuestionResponse?> GetByPollIdAsync(int pollId, int id);
        Task<ApiResponseData<QuestionResponse>> GetListByPollIdAsync(int pollId);
        Task Update(Question question);
    }
}
