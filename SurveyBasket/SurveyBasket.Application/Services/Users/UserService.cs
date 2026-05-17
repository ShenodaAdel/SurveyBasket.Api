using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Role;
using SurveyBasket.Application.Services.Role.Dtos;
using SurveyBasket.Application.Services.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyBasket.Application.Services.Users
{
    public class UserService(UserManager<ApplicationUser> userManager, IRoleService roleService
        ,IUnitOfWork unitOfWork) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IRoleService _roleService = roleService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ApiResponse<object?>> GetUserProfileAsync(string userId)
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
                .ProjectToType<UserProfileResponse>()
                .SingleAsync();

            if (user == null)
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

        public async Task<ApiResponse<object?>> GetAllAsync()
        {
            var messages = new List<ApiResponseMessage>();

            var users = await _unitOfWork.UserRepository.GetAllAsync();

            if (users == null || !users.Any())
            {
                messages.Add(new ApiResponseMessage("info", "No users found."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status200OK,
                    messages: messages);
            }

            messages.Add(new ApiResponseMessage("success", "Users fetched successfully."));
            return new ApiResponse<object?>(
                data: users,
                status: StatusCodes.Status200OK,
                messages: messages);
        }

        public async Task<ApiResponse<object?>> GetByIdAsync(string userId)
        {
            var messages = new List<ApiResponseMessage>();

            var user = await _userManager.Users
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                messages.Add(new ApiResponseMessage("error", "User not found."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var response = (user, userRoles).Adapt<UserResponse>();

            messages.Add(new ApiResponseMessage("success", "User fetched successfully."));
            return new ApiResponse<object?>(
                data: response,
                status: StatusCodes.Status200OK,
                messages: messages);

        }
        public async Task<ApiResponse<object?>> CreateAsync(CreateUserRequest request)
        {
            var messages = new List<ApiResponseMessage>();

            var emailIsExists = await _userManager.Users.AnyAsync(u => u.Email == request.Email);

            if (emailIsExists)
            {
                messages.Add(new ApiResponseMessage("validation", "Email", $"Email : {request.Email} Is Already Taken."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var rolesResponse = await _roleService.GetAllAsync();
            var allowedRoles = (rolesResponse.Data as IEnumerable<RoleResponse>)?.Select(r => r.Name)
                ?? Enumerable.Empty<string>();

            if (request.Roles.Except(allowedRoles).Any())
            {
                messages.Add(new ApiResponseMessage("validation", "Roles", $"One Or More Roles Are Invalid."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var user = request.Adapt<ApplicationUser>();
            user.UserName = request.Email;
            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, request.Password);
            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    messages.Add(new ApiResponseMessage("error", error.Code, error.Description));
                }
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            await _userManager.AddToRolesAsync(user, request.Roles);
            messages.Add(new ApiResponseMessage("success", "User created successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status201Created,
                messages: messages);

        }
        public async Task<ApiResponse<object?>> UpdateAsync(string id, UpdateUserRequest request)
        {
            var messages = new List<ApiResponseMessage>();

            if (await _userManager.FindByIdAsync(id) is not { } user)
            {
                messages.Add(new ApiResponseMessage("error", "User not found."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            var emailIsExists = await _userManager.Users.AnyAsync(u => u.Email == request.Email && u.Id != id);
            if (emailIsExists)
            {
                messages.Add(new ApiResponseMessage("validation", "Email", $"Email : {request.Email} Is Already Taken."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var rolesResponse = await _roleService.GetAllAsync();
            var allowedRoles = (rolesResponse.Data as IEnumerable<RoleResponse>)?.Select(r => r.Name)
                ?? Enumerable.Empty<string>();

            if (request.Roles.Except(allowedRoles).Any())
            {
                messages.Add(new ApiResponseMessage("validation", "Roles", "One Or More Roles Are Invalid."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            user = request.Adapt(user);

            var result = await _userManager.UpdateAsync(user);
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

            var currentRoles = await _userManager.GetRolesAsync(user);

            var rolesToRemove = currentRoles.Except(request.Roles).ToList();
            var rolesToAdd = request.Roles.Except(currentRoles).ToList();

            if (rolesToRemove.Count > 0)
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (rolesToAdd.Count > 0)
                await _userManager.AddToRolesAsync(user, rolesToAdd);

            messages.Add(new ApiResponseMessage("success", "User updated successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages);
        }
        public async Task<ApiResponse<object?>> ToggleAsync(string id)
        {
            var messages = new List<ApiResponseMessage>();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null )
            {
                messages.Add(new ApiResponseMessage("info", "No user found."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status200OK,
                    messages: messages);
            }

            user.IsDisabled = !user.IsDisabled;
            var result = await _userManager.UpdateAsync(user);

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

            messages.Add(new ApiResponseMessage("success", "Users toggled successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages);
        }
        public async Task<ApiResponse<object?>> UnLockAsync(string id)
        {
            var messages = new List<ApiResponseMessage>();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                messages.Add(new ApiResponseMessage("info", "No user found."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status200OK,
                    messages: messages);
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

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

            messages.Add(new ApiResponseMessage("success", "User unlocked successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages);
        }
    }
}
