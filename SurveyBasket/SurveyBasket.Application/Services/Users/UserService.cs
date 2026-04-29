using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Application.Services.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyBasket.Application.Services.Users
{
    public class UserService(UserManager<ApplicationUser> userManager) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ApiResponse<object?>> GetUserProfileAsync(string userId)
        {
            var messages = new List<ApiResponseMessage>();
            if(string.IsNullOrEmpty(userId))
            {
                messages.Add(new ApiResponseMessage("validation", "UserId", $"UserId : {userId} Is Required."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var user = await _userManager.Users
                .Where(u => u.Id == userId)
                .ProjectToType<UserProfileResponse>()
                .SingleAsync();

            if(user == null)
            {
                messages.Add(new ApiResponseMessage("Error", "UserId", $"User With Id : {userId} Not Found."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            messages.Add(new ApiResponseMessage("success", "User fetched successfully."));
            return new ApiResponse<object?>(
                data: user.Adapt<UserProfileResponse>(),
                status: StatusCodes.Status200OK,
                messages: messages);
            
        }

        public async Task<ApiResponse<object?>> UpdateUserProfileAsync(string userId, UpdateProfileRequest request)
        {
            var messages = new List<ApiResponseMessage>();

            if (string.IsNullOrEmpty(userId))
            {
                messages.Add(new ApiResponseMessage("validation", "UserId", $"UserId : {userId} Is Required."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            //var user = await _userManager.Users
            //    .Where(u => u.Id == userId)
            //    .SingleAsync();

            //user.FirstName = request.FirstName;
            //user.LastName = request.LastName;

            //await _userManager.UpdateAsync(user);

            await _userManager.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.FirstName, request.FirstName)
                    .SetProperty(u => u.LastName, request.LastName));

            messages.Add(new ApiResponseMessage("success", "User profile updated successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages);
        }

        public async Task<ApiResponse<object?>> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var messages = new List<ApiResponseMessage>();

            if (string.IsNullOrEmpty(userId))
            {
                messages.Add(new ApiResponseMessage("validation", "UserId", $"UserId : {userId} Is Required."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var user = await _userManager.Users
                .Where(u => u.Id == userId)
                .SingleAsync();

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    messages.Add(new ApiResponseMessage("error", error.Code, error.Description));
                }
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            messages.Add(new ApiResponseMessage("success", "Password changed successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages);
        }
    }
}
