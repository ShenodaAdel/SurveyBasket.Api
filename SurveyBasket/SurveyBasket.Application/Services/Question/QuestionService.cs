using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SurveyBasket.Application.Services.Caching;
using SurveyBasket.Application.Services.Question.Dtos;
using SurveyBasket.Domain.Entities;


namespace SurveyBasket.Application.Services.Question
{
    public class QuestionService(IUnitOfWork unitOfWork , ICacheService cacheService
        , IValidator<QuestionRequest> validator
        , ILogger<QuestionService> logger
        , TypeAdapterConfig config) 
        : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<QuestionRequest> _validator = validator;
        private readonly ILogger<QuestionService> _logger = logger;
        private readonly ICacheService _cacheService = cacheService;
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

            await _cacheService.RemoveAsync($"AvailableQuestions_Poll_{pollId}");

            messages.Add(new ApiResponseMessage("success", "Question Created successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status201Created,
                messages: messages);
        }

        public async Task<ApiResponse<object?>> UpdateAsync(int pollId  , int id , QuestionRequest request)
        {
            var messages = new List<ApiResponseMessage>();

            if (pollId <= 0 || id <= 0)
            {
                messages.Add(new ApiResponseMessage("validation", "Id and Poll Id", $"PollId and QuestionId are Required."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var questionIsExists =  await _unitOfWork.QuestionRepository.CheckIsExistWithSameContentBYPollIdAsync(request.Content, pollId, id);

            if (questionIsExists)
            {
                messages.Add(new ApiResponseMessage("error", "Content", $"Question with content : {request.Content} already exists in Poll with Id : {pollId}."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var question = await _unitOfWork.QuestionRepository.GetByPollIdAndIdAsync(pollId, id);

            if(question == null)
            {
                messages.Add(new ApiResponseMessage("error", "Id", $"No Question found with Id."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            question.Content = request.Content;

            var currentAnswers = question.Answers.Select(answer => answer.Content).ToList(); 

            // add new Answers in DB 
            var newAnswers = request.Answers.Except(currentAnswers).ToList();

            newAnswers.ForEach(answer =>
            {
                question.Answers.Add(new Domain.Entities.Answer { Content = answer });
            });

            question.Answers.ToList().ForEach(answer => { 
                answer.IsDeleted = request.Answers.Contains(answer.Content);
            });
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.RemoveAsync($"AvailableQuestions_Poll_{pollId}");

            messages.Add(new ApiResponseMessage("success", "Question updated successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages);

        }

        public async Task<ApiResponse<object?>> GetListByPollId( int pollId )
        {
            var messages = new List<ApiResponseMessage>();

            var poll = await _unitOfWork.PollRepository.GetByIdAsync(pollId);

            if (poll == null)
            {
                messages.Add(new ApiResponseMessage("error", $"No Poll found with id : {pollId}."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            var questions = await _unitOfWork.QuestionRepository.GetListByPollIdAsync(pollId);

            if( questions.Total == 0 )
            {
                messages.Add(new ApiResponseMessage("error", "No Questions found."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            messages.Add(new ApiResponseMessage("success", "Questions fetched successfully."));
            return new ApiResponse<object?>(
                data: questions,
                status: StatusCodes.Status200OK,
                messages: messages);
        }

        public async Task<ApiResponse<object?>> GetAvailableListByPollId( int pollId , string userId )
        {
            var messages = new List<ApiResponseMessage>();

            var hasVote = await _unitOfWork.VoteRepository.CheckUserVoted(userId,pollId);

            if (hasVote)
            {
                messages.Add(new ApiResponseMessage("error", "User has already voted on this poll."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var pollISExist = await _unitOfWork.PollRepository.CheckIsActiveAsync(pollId);

            if (!pollISExist)
            {
                messages.Add(new ApiResponseMessage("error", $"No Active Poll found with id : {pollId}."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            var cacheKey = $"AvailableQuestions_Poll_{pollId}";

            var cachedQuestions = await _cacheService.GetAsync<ApiResponseData<QuestionResponse>>(cacheKey);
            if (cachedQuestions != null)
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}. Returning questions from cache.", cacheKey);
                messages.Add(new ApiResponseMessage("success", "Questions fetched successfully from cache."));
                return new ApiResponse<object?>(
                    data: cachedQuestions,
                    status: StatusCodes.Status200OK,
                    messages: messages);
            }
            _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching questions from database.", cacheKey);
            var questions = await _unitOfWork.QuestionRepository.GetListByPollIdAsync(pollId);
            await _cacheService.SetAsync(cacheKey, questions);

            if (questions.Total == 0)
            {
                messages.Add(new ApiResponseMessage("error", "No Questions found."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            messages.Add(new ApiResponseMessage("success", "Questions fetched successfully."));
            return new ApiResponse<object?>(
                data: questions,
                status: StatusCodes.Status200OK,
                messages: messages);
        }
        public async Task<ApiResponse<object?>> GetByPollId(int pollId , int id )
        {
            var messages = new List<ApiResponseMessage>();

            var poll = await _unitOfWork.PollRepository.GetByIdAsync(pollId);

            if (poll == null)
            {
                messages.Add(new ApiResponseMessage("error", $"No Poll found with id : {pollId}."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            var question = await _unitOfWork.QuestionRepository.GetByPollIdAsync(pollId,id);

            if (question == null)
            {
                messages.Add(new ApiResponseMessage("error", "No Question found."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            messages.Add(new ApiResponseMessage("success", "Question fetched successfully."));
            return new ApiResponse<object?>(
                data: question,
                status: StatusCodes.Status200OK,
                messages: messages);
        }

        public async Task<ApiResponse<object?>> ToggleStatusAsync(int pollId , int id )
        {
            var messages = new List<ApiResponseMessage>();

            if (pollId <= 0 || id <= 0 )
            {
                messages.Add(new ApiResponseMessage("validation", "Id and Poll Id", $"PollId and QuestionId are Required."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var question = await _unitOfWork.QuestionRepository.GetByPollIdAndIdAsync(pollId,id);

            if (question == null)
            {
                messages.Add(new ApiResponseMessage("error", "Id", $"No Question found with Id."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            question.IsDeleted = !question.IsDeleted;
            _unitOfWork.QuestionRepository.Update(question);
            await _unitOfWork.SaveChangesAsync();

            messages.Add(new ApiResponseMessage("success", "Question status updated successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages);
        }
    }
}
