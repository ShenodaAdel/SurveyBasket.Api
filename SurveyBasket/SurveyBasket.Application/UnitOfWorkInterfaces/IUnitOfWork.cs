namespace SurveyBasket.Application.UnitOfWork
{
    public interface IUnitOfWork
    {
        IPollRepository PollRepository { get; set; }
        IUserRepository UserRepository { get; set; }
        Task SaveChangesAsync();
    }
}
