

namespace SurveyBasket.Application.Services.PollService
{
    public interface IPollService
    {
        Task<ApiResponse<object?>> CreateAsync(PollRequest request);
        Task<ApiResponse<object?>> DeleteAsync(int id);
        Task<ApiResponse<object?>> GetById(int id);
        Task<ApiResponse<object?>> GetList();
        Task<ApiResponse<object?>> UpdateAsync(int id, PollRequest request);
    }
}
