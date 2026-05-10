using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Role.Dtos;

namespace SurveyBasket.Application.Services.Role
{
    public class RoleService(RoleManager<ApplicationRole> roleManager , IUnitOfWork unitOfWork ) : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ApiResponse<object?>> GetAllAsync(bool? includeDisabled = false)
        {
            var messages = new List<ApiResponseMessage>();

            var roles = await _roleManager.Roles
                 .Where(r => !r.IsDefault && (!r.IsDeleted || (includeDisabled.HasValue && includeDisabled.Value)))
                 .ProjectToType<RoleResponse>()
                 .ToListAsync();

            messages.Add(new ApiResponseMessage("Success", "Roles retrieved successfully."));

            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                data: roles,
                messages: messages);
        }
        public async Task<ApiResponse<RoleDetailResponse?>> GetByIdAsync(string id)
        {
            var messages = new List<ApiResponseMessage>();
            if (await _roleManager.FindByIdAsync(id) is not { } role)
            {
                messages.Add(new ApiResponseMessage("Not Found", "Role not found."));
                return new ApiResponse<RoleDetailResponse?>(
                    status: StatusCodes.Status404NotFound,
                    data: null,
                    messages: messages);
            }

            var permissions = await _roleManager.GetClaimsAsync(role);

            var response = new RoleDetailResponse
            (
                role.Id,
                role.Name!,
                role.IsDeleted,
                permissions.Select(c => c.Value).ToList()
            );

            messages.Add(new ApiResponseMessage("Success", "Role retrieved successfully."));
            return new ApiResponse<RoleDetailResponse?>(
                status: StatusCodes.Status200OK,
                data: response,
                messages: messages);
        }
        public async Task<ApiResponse<object?>> CreateAsync(RoleRequest request)
        {
            var messages = new List<ApiResponseMessage>();

            var roleIsExist = await _roleManager.RoleExistsAsync(request.Name);
            if (roleIsExist)
            {
                messages.Add(new ApiResponseMessage("Conflict", "Role with the same name already exists."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status409Conflict,
                    data: null,
                    messages: messages);
            }   

            var allowedPermissions = Permissions.GetAllPermissions();

            if(request.Permissions.Except(allowedPermissions).Any())
            {
                messages.Add(new ApiResponseMessage("Bad Request", "One or more permissions are invalid."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    data: null,
                    messages: messages);
            }

            var role = new ApplicationRole
            {
                Name = request.Name,
                ConcurrencyStamp= Guid.NewGuid().ToString(),
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                messages.AddRange(result.Errors.Select(e => new ApiResponseMessage("Error", e.Description)));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status500InternalServerError,
                    data: null,
                    messages: messages);
            }

            var permissions = request.Permissions.Select(p => new IdentityRoleClaim<string>
            {
                ClaimType = Permissions.Type,
                ClaimValue = p,
                RoleId = role.Id
            });
            
            await _unitOfWork.RoleRepository.AddRangeAsync(permissions);
            await _unitOfWork.SaveChangesAsync();

            messages.Add(new ApiResponseMessage("Success", "Role created successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status201Created,
                data: null,
                messages: messages);

        }
        public async Task<ApiResponse<object?>> UpdateAsync(string id , RoleRequest request)
        {
            var messages = new List<ApiResponseMessage>();

            if(await _roleManager.FindByIdAsync(id) is not { } role)
            {
                messages.Add(new ApiResponseMessage("Not Found", "Role not found."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    data: null,
                    messages: messages);
            }

            var roleIsExist = await _roleManager.Roles.AnyAsync(r => r.Name == request.Name && r.Id != id);
            if (roleIsExist)
            {
                messages.Add(new ApiResponseMessage("Conflict", "Role with the same name already exists."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status409Conflict,
                    data: null,
                    messages: messages);
            }

            var allowedPermissions = Permissions.GetAllPermissions();

            if (request.Permissions.Except(allowedPermissions).Any())
            {
                messages.Add(new ApiResponseMessage("Bad Request", "One or more permissions are invalid."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    data: null,
                    messages: messages);
            }

            role.Name = request.Name;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                messages.AddRange(result.Errors.Select(e => new ApiResponseMessage("Error", e.Description)));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status500InternalServerError,
                    data: null,
                    messages: messages);
            }

            var existingPermissions = await _unitOfWork.RoleRepository.GetAllRoles(id);

            var newPermissions = request.Permissions.Except(existingPermissions)
                .Select(p => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = p,
                    RoleId = role.Id
                });

            var removedPermissions = existingPermissions.Except(request.Permissions);

            await _unitOfWork.RoleRepository.ExecuteDeleteAsync(id, removedPermissions);

            await _unitOfWork.RoleRepository.AddRangeAsync(newPermissions);
            await _unitOfWork.SaveChangesAsync();

            messages.Add(new ApiResponseMessage("Success", "Role updated successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                data: null,
                messages: messages);
        }
        public async Task<ApiResponse<object?>> ToggleAsync(string id)
        {
            var messages = new List<ApiResponseMessage>();

            if (await _roleManager.FindByIdAsync(id) is not { } role)
            {
                messages.Add(new ApiResponseMessage("Not Found", "Role not found."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    data: null,
                    messages: messages);
            }

            role.IsDeleted = !role.IsDeleted;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                messages.AddRange(result.Errors.Select(e => new ApiResponseMessage("Error", e.Description)));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status500InternalServerError,
                    data: null,
                    messages: messages);
            }

            await _unitOfWork.SaveChangesAsync();

            messages.Add(new ApiResponseMessage("Success", "Role toggled successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                data: null,
                messages: messages);
        }
    }
}
