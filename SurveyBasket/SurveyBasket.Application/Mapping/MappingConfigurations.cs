using SurveyBasket.Application.Services.Question.Dtos;
using SurveyBasket.Application.Services.Users.Dtos;

namespace SurveyBasket.Application.Mapping
{
    public class MappingConfigurations : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<QuestionRequest, Question>()
                .Ignore(dest => dest.Answers)
                .AfterMapping((src, dest) =>
                {
                    dest.Answers = src.Answers
                        .Select(a => new Answer { Content = a })
                        .ToList();
                });

            config.NewConfig<(ApplicationUser user, IList<string> roles), UserResponse>()
                .Map(dest => dest, src => src.user)
                .Map(dest => dest.Roles, src => src.roles);

            config.NewConfig<UpdateUserRequest, ApplicationUser>()
                .Map(dest => dest.UserName, src => src.Email)
                .Map(dest => dest.NormalizedEmail, src => src.Email.ToUpper());
        }
    }
}
