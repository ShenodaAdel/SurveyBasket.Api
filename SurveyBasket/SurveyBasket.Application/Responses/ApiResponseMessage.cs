namespace SurveyBasket.Application.Responses
{
    public class ApiResponseMessage
    {
        public string Type { get; set; } = default!;
        public string? Field { get; set; }
        public string Text { get; set; } = default!;

        public ApiResponseMessage(string type, string text)
        {
            Type = type;
            Text = text;
        }

        public ApiResponseMessage(string type, string field, string text)
        {
            Type = type;
            Field = field;
            Text = text;
        }

    }
}
