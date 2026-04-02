

namespace SurveyBasket.Application.RepositoriesInterfaces
{
    public interface IVoteRepository
    {
        Task AddAsync(Vote vote);
        Task<bool> CheckUserVoted(string userId, int pollId);
    }
}
