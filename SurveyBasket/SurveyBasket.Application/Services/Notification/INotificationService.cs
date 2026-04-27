namespace SurveyBasket.Application.Services.Notification
{
    public interface INotificationService
    {
        Task SendNewPollsNotification(int? pollId = null);
    }
}
