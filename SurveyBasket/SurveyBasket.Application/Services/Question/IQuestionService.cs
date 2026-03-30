using SurveyBasket.Application.Services.Question.Dtos;

namespace SurveyBasket.Application.Services.Question
{
    public interface IQuestionService
    {
        Task<ApiResponse<object?>> CreateAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<object?>> GetByPollId(int pollId, int id);
        Task<ApiResponse<object?>> GetListByPollId(int pollId);
        Task<ApiResponse<object?>> ToggleStatusAsync(int pollId, int id);
        Task<ApiResponse<object?>> UpdateAsync(int pollId, int id , QuestionRequest request);
    }
}
    