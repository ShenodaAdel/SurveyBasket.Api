using SurveyBasket.Application.Services.Question.Dtos;

namespace SurveyBasket.Application.Services.Question
{
    public interface IQuestionService
    {
        Task<ApiResponse<object?>> CreateAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default);
    }
}
    