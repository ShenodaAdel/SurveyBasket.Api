
using SurveyBasket.Application.Services.Result.Dtos;

namespace SurveyBasket.Application.RepositoriesInterfaces
{
    public interface IPollRepository
    {
        Task<Poll?> GetByIdAsync(int id);
        Task<ApiResponseData<Poll>> GetListAsync();
        Task AddAsync(Poll poll);
        Task Update(Poll poll);
        Task Delete(Poll poll);
        Task<bool> CheckTitleAsync(string title);
        Task<bool> CheckTitleAndNotTheSamePollAsync(string title , int id);
        Task<ApiResponseData<PollResponse>> GetCurrenrtListAsync();
        Task<bool> CheckIsActiveAsync(int id);
        Task<PollVoteResponse?> GetPollVoteResponseAsync(int pollId, CancellationToken cancellationToken = default);
    }
}
