namespace SurveyBasket.Application.Services.PollService.Dto
{
    public record PollRequest(
        string Title,
        string Summary,
        DateOnly StartsAt,
        DateOnly EndsAt
    );
}
