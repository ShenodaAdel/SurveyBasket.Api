namespace SurveyBasket.Application.Services.PollService.Dto
{
    public record PollRequest (
        string Title,
        string Summary,
        DateOnly StarstAt,
        DateOnly EndsAt
        );
}
