namespace SurveyBasket.Application.RepositoriesInterfaces
{
    public interface IPollRepository
    {
        Task<Poll> GetByIdAsync(int id);
        Task<ApiResponseData<Poll>> GetListAsync();
        Task AddAsync(Poll poll);
        Task Update(Poll poll);
        Task Delete(Poll poll);
    }
}
