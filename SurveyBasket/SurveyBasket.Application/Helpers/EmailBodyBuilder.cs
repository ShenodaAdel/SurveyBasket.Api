namespace SurveyBasket.Application.Helpers
{
    public static class EmailBodyBuilder
    {
        public static string BuildEmailConfirmationBody(string template, Dictionary<string, string> templateModel)
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", $"{template}.html");

            var body = File.ReadAllText(templatePath);

            foreach (var item in templateModel)
            {
                body = body.Replace(item.Key, item.Value);
            }

            return body;
        }
    }
}
