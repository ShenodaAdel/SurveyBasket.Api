using SurveyBasket.Application.Services.Result.Dtos;

namespace SurveyBasket.Application.Services.Result
{
    public class ResultService(IUnitOfWork unitOfWork) : IResultService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ApiResponse<object?>> GetPollVotesAsync(int pollId , CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            if (pollId <= 0)
            {
                messages.Add(new ApiResponseMessage("validation", "PollId", "Poll Id must be greater than zero."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var poll = await _unitOfWork.PollRepository.GetPollVoteResponseAsync(pollId,cancellationToken);

            if(poll == null)
            {
                messages.Add(new ApiResponseMessage("error", $"No Poll found with id : {pollId}.")); 
                return new ApiResponse<object?>(
                status: StatusCodes.Status404NotFound, messages: messages);
            }

            messages.Add(new ApiResponseMessage("success", "Poll votes fetched successfully."));
            return new ApiResponse<object?>(
                data: poll,
                status: StatusCodes.Status200OK,
                messages: messages);
        }
        public async Task<ApiResponse<object?>> GetVotesPerDayAsync(int pollId , CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            if (pollId <= 0)
            {
                messages.Add(new ApiResponseMessage("validation", "PollId", "Poll Id must be greater than zero."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var votes = await _unitOfWork.VoteRepository.GetVotesPerDayAsync (pollId, cancellationToken);

            if(!votes.Any())
            {
                messages.Add(new ApiResponseMessage("error", $"No votes found for poll with id : {pollId}."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }
            messages.Add(new ApiResponseMessage("success", "Votes per day fetched successfully."));
            return new ApiResponse<object?>(
                data: votes,
                status: StatusCodes.Status200OK,
                messages: messages);
        }
        public async Task<ApiResponse<object?>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken =default)
        {
            var messages = new List<ApiResponseMessage>();

            if (pollId <= 0)
            {
                messages.Add(new ApiResponseMessage("validation", "PollId", "Poll Id must be greater than zero."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var votes = await _unitOfWork.VoteRepository.GetVotesPerQuestionAsync(pollId, cancellationToken);

            if (!votes.Any())
            {
                messages.Add(new ApiResponseMessage("error", $"No votes found for poll with id : {pollId}."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            messages.Add(new ApiResponseMessage("success", "Votes per question fetched successfully."));
            return new ApiResponse<object?>(
                data: votes,
                status: StatusCodes.Status200OK,
                messages: messages);
        }
    }
}
