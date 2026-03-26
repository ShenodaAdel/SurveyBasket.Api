using SurveyBasket.Application.Responses;

namespace SurveyBasket.API.Middleware
{
    // ده لو انت شغال قبل .NET 8 
    // هخلي الكود ده عشان لو عايز ترجع ليه تتعلم منو  حاجة ولا لا 
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext httpContext) 
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred : {Message}" , ex.Message);
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var messages = new List<ApiResponseMessage>
                {
                    new ApiResponseMessage("error", "ExceptionHandling", ex.Message)
                };

                var apiResponse = new ApiResponse<object>(
                    StatusCodes.Status500InternalServerError,
                    messages
                );

                await httpContext.Response.WriteAsJsonAsync(apiResponse);
            }
        }
    }
}
