using SurveyBasket.Application.Services.Question.Dtos;


namespace SurveyBasket.Application.Services.Question
{
    public class QuestionService(IUnitOfWork unitOfWork, IValidator<QuestionRequest> validator , TypeAdapterConfig config) : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<QuestionRequest> _validator = validator;
        private readonly TypeAdapterConfig _config = config;

        public async Task<ApiResponse<object?>> CreateAsync(int pollId , QuestionRequest request , CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            var pollIsExist = await _unitOfWork.PollRepository.GetByIdAsync(pollId);
            if (pollIsExist == null)
            {
                messages.Add(new ApiResponseMessage("error", "Id", $"No Poll found with Id : {pollId}."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors
                    .Select(e => new ApiResponseMessage("Bad Request", e.ErrorMessage))
                    .ToList();

                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: errorMessages,
                    data: null
                );
            }


            var questionIsExist = await _unitOfWork.QuestionRepository.CheckIsExistAsync(request.Content, pollId);
            if (questionIsExist)
            {
                messages.Add(new ApiResponseMessage("error", "Content", $"Question with content : {request.Content} already exists in Poll with Id : {pollId}."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }


            var question = request.Adapt<Domain.Entities.Question>(_config);
            question.PollId = pollId;


            await _unitOfWork.QuestionRepository.AddAsync(question);
            await _unitOfWork.SaveChangesAsync();

            messages.Add(new ApiResponseMessage("success", "Question Created successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status201Created,
                messages: messages);
        }
    }
}
