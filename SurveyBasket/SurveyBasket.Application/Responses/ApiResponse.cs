namespace SurveyBasket.Application.Responses
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public T? Data { get; set; }
        public List<ApiResponseMessage> Messages { get; set; } = new();

        public ApiResponse(int status, T? data, List<ApiResponseMessage> messages)
        {
            Status = status;
            Data = data;
            Messages = messages;
        }

        public ApiResponse(int status, List<ApiResponseMessage> messages)
        {
            Status = status;
            Messages = messages;
        }
    }
}
