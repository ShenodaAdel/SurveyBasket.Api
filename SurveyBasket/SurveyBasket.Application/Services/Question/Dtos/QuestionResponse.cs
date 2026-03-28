using SurveyBasket.Application.Services.Answer.Dros;

namespace SurveyBasket.Application.Services.Question.Dtos
{
    public record QuestionResponse (int id , string content, IEnumerable<AnswerResponse>Answers);
}
