namespace SurveyBasket.Application.Services.Result.Dtos
{
    public record VotesPerQuestionResponse(string Question, IEnumerable<VotesPerAnswerResponse> SelectedAnswers);

    public record VotesPerAnswerResponse(string Answer, int Count);
}
