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
    }
}
