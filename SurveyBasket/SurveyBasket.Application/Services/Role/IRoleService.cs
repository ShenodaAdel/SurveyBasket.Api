using SurveyBasket.Application.Services.Role.Dtos;

namespace SurveyBasket.Application.Services.Role
{
    public interface IRoleService
    {
        Task<ApiResponse<object?>> CreateAsync(RoleRequest request);
        Task<ApiResponse<object?>> GetAllAsync(bool? includeDisabled = false);
        Task<ApiResponse<RoleDetailResponse?>> GetByIdAsync(string id);
        Task<ApiResponse<object?>> ToggleAsync(string id);
        Task<ApiResponse<object?>> UpdateAsync(string id, RoleRequest request);
    }
}
