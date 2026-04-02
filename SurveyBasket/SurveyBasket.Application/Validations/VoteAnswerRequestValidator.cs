using SurveyBasket.Application.Services.Vote.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyBasket.Application.Validations
{
    public class VoteAnswerRequestValidator : AbstractValidator<VoteAnswerRequest>
    {
        public VoteAnswerRequestValidator()
        {
            RuleFor(v => v.QuestionId)
                .GreaterThan(0)
                .WithMessage("Question Id is required and must be greater than 0.");
            RuleFor(v => v.AnswerId)
                .GreaterThan(0)
                .WithMessage("Answer Id is required and must be greater than 0.");
        }
    }
}
