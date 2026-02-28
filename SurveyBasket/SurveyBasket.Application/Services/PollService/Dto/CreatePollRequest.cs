namespace SurveyBasket.Application.Services.PollService.Dto
{
    public record CreatePollRequest (
        string Title,
        string Summary,
        bool Ispublished,
        DateOnly StarstAt,
        DateOnly EndsAt
        );
}
