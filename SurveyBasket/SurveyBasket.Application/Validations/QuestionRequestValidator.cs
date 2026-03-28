using SurveyBasket.Application.Services.Question.Dtos;
namespace SurveyBasket.Application.Validations
{
    public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
    {
        public QuestionRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .Length(3,1000)
                .WithMessage("Question content is required.");

            RuleFor(x => x.Answers) 
                   .NotNull();

            RuleFor(x => x.Answers) // First Rule
                .Must(answers => answers.Count > 1)
                .WithMessage("At least two answers are required.")
                .When(x => x.Answers != null); // Ensure this rule is only applied when Answers is not null

            RuleFor(x => x.Answers) // Second Rule
                .Must(answers => answers.Distinct().Count() == answers.Count)
                .WithMessage("You cannot add dublicated answers for the same question")
                .When(x => x.Answers != null); 
        }
    }
}
