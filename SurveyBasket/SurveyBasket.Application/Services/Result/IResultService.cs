
namespace SurveyBasket.Application.Services.Result
{
    public interface IResultService
    {
        Task<ApiResponse<object?>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default);
        Task<ApiResponse<object?>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default);
        Task<ApiResponse<object?>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default);
    }
}
