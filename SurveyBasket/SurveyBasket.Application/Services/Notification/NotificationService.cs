using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Email;

namespace SurveyBasket.Application.Services.Notification
{
    public class NotificationService(IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IEmailService emailService) : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IEmailService _emailService = emailService;
        public async Task SendNewPollsNotification(int? pollId = null)
        {
            var polls = new List<Poll>();

            if (pollId.HasValue)
            {
                var poll = await _unitOfWork.PollRepository.GetById(pollId.Value);
                polls = [poll!];
            }
            else
            {
                polls = await _unitOfWork.PollRepository.GetAllIsPublished();
            }

            // TODO : Select only users to send notification

            var users = await _userManager.Users.ToListAsync();
            var origin = _httpContextAccessor.HttpContext?.Request.Headers["Origin"].ToString();
            foreach (var poll in polls)
            {
                foreach (var user in users)
                {
                    var Placeholders = new Dictionary<string, string>
                    {
                        { "{name}", user.FirstName },
                        { "{pollTitle}", poll.Title },
                        { "{endDate}", poll.EndsAt.ToString("MMMM d, yyyy") },
                        { "{url}", $"{origin}/polls/start/{poll.Id}"   }
                    };

                    var body = EmailBodyBuilder.BuildEmailConfirmationBody("PollNotification", Placeholders);

                    await _emailService.SendEmailAsync(user.Email!, $"Survey Basket : New Poll Available - {poll.Title}", body);
                }
            }

        }
    }
}
