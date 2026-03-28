using SurveyBasket.Application.Services.Question.Dtos;

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
        }
    }
}
