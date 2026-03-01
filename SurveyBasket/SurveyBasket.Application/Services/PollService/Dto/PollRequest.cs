namespace SurveyBasket.Application.Services.PollService.Dto
{
    public record PollRequest (
        string Title,
        string Summary,
        bool Ispublished,
        DateOnly StarstAt,
        DateOnly EndsAt
        );
}
