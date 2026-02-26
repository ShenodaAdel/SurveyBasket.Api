
namespace SurveyBasket.Application.Services.PollService
{
    public interface IPollService
    {
        Task<ApiResponse<object?>> DeleteAsync(int id);
        Task<ApiResponse<object?>> GetById(int id);
        Task<ApiResponse<object?>> GetList();
    }
}
