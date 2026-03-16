namespace SurveyBasket.Application.UnitOfWork
{
    public interface IUnitOfWork
    {
        IPollRepository PollRepository { get; }
        IUserRepository UserRepository { get; }
        Task SaveChangesAsync();
    }
}
