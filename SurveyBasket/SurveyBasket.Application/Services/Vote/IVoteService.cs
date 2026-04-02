using SurveyBasket.Application.Services.Vote.Dtos;

namespace SurveyBasket.Application.Services.Vote
{
    public interface IVoteService
    {
        Task<ApiResponse<object?>> CreateAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default);
    }
}
