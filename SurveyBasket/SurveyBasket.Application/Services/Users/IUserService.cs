using SurveyBasket.Application.Services.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyBasket.Application.Services.Users
{
    public interface IUserService
    {
        Task<ApiResponse<object?>> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<ApiResponse<object?>> GetUserProfileAsync(string userId);
        Task<ApiResponse<object?>> UpdateUserProfileAsync(string userId, UpdateProfileRequest request);
    }
}
