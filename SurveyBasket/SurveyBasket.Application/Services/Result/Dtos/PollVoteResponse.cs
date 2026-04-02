namespace SurveyBasket.Application.Services.Result.Dtos
{
    public record PollVoteResponse( string Title ,  IEnumerable<VoteResponse> Votes);
    public record VoteResponse( string VoterName ,
        DateTime VoteDate,
        IEnumerable<QuestionAnswerResponse> SelectedAnswers);
    public record QuestionAnswerResponse(string Question , string Answer);
}
