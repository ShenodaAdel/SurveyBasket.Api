namespace SurveyBasket.Application.Services.PollService.Dto
{
    public record PollResponse(
         int Id,
         string Title,
         string Summary,
         bool Ispublished,
         DateOnly StarstAt,
         DateOnly EndsAt
         ); // using priamary constructor syntax to create an immutable record type for PollResponse
}
