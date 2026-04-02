using SurveyBasket.Application.Services.Vote.Dtos;
using System.Reflection;

namespace SurveyBasket.Application.Services.Vote
{
    public class VoteService(IUnitOfWork unitOfWork) : IVoteService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ApiResponse<object?>> CreateAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            var hasVote = await _unitOfWork.VoteRepository.CheckUserVoted(userId, pollId);

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

            var questionIds = await _unitOfWork.QuestionRepository.GetActiveQuestionIds(pollId);

            var invalidAnswers = request.Answers.Any(a => a.QuestionId <= 0 || a.AnswerId <= 0);

            if (invalidAnswers)
            {
                messages.Add(new ApiResponseMessage("validation", "Question Id and Answer Id must be greater than 0."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            if (!request.Answers.Select(a => a.QuestionId).SequenceEqual(questionIds))
            {
                messages.Add(new ApiResponseMessage("error", "Answers must include all poll questions with no duplicates ormissing entries."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var vote = new Domain.Entities.Vote
            {
                PollId = pollId,
                UserId = userId,
                VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
            };

            await _unitOfWork.VoteRepository.AddAsync(vote);
            await _unitOfWork.SaveChangesAsync();

            messages.Add(new ApiResponseMessage("success", "Vote submitted successfully."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status201Created,
                messages: messages);
        }
    }
}
