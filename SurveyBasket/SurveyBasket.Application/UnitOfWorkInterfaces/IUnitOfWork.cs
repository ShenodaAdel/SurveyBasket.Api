namespace SurveyBasket.Application.UnitOfWork
{
    public interface IUnitOfWork
    {
        IPollRepository PollRepository { get; }
        IUserRepository UserRepository { get; }
        IQuestionRepository QuestionRepository { get; }
        Task SaveChangesAsync();
    }
}
