using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyBasket.Application.Services.Result.Dtos
{
    public record VotesPerQuestionResponse(string Question , IEnumerable<VotesPerAnswerResponse> SelectedAnswers);

    public record VotesPerAnswerResponse(string Answer , int Count);
}
