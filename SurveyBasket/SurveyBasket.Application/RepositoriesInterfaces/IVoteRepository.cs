

using SurveyBasket.Application.Services.Result.Dtos;

namespace SurveyBasket.Application.RepositoriesInterfaces
{
    public interface IVoteRepository
    {
        Task AddAsync(Vote vote);
        Task<bool> CheckUserVoted(string userId, int pollId);
        Task<IEnumerable<VotesPerDayResponse>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default);
        Task<IEnumerable<VotesPerQuestionResponse>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default);
    }
}
