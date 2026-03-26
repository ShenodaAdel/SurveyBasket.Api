using Microsoft.AspNetCore.Diagnostics;
using SurveyBasket.Application.Responses;

namespace SurveyBasket.API.Middleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler>logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred : {Message}", exception.Message);
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var messages = new List<ApiResponseMessage>
                {
                    new ApiResponseMessage("error", "ExceptionHandling", exception.Message)
                };

            var apiResponse = new ApiResponse<object>(
                StatusCodes.Status500InternalServerError,
                messages
            );

            await httpContext.Response.WriteAsJsonAsync(apiResponse , cancellationToken);
            return true;
        }
    }
}
