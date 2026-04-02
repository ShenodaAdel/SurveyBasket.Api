namespace SurveyBasket.Application.Services.Vote.Dtos
{
    public record VoteRequest (IEnumerable<VoteAnswerRequest> Answers); 
}
