namespace SurveyBasket.Application.UnitOfWork
{
    public interface IUnitOfWork
    {
        IPollRepository PollRepository { get; }
        IUserRepository UserRepository { get; }
        IQuestionRepository QuestionRepository { get; }
        IVoteRepository VoteRepository { get; }
        IRoleRepository RoleRepository { get; }
        Task SaveChangesAsync();
    }
}
