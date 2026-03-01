namespace SurveyBasket.Application.Services.PollService
{
    public class PollService : IPollService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<PollRequest> _validator;

        public PollService(IUnitOfWork unitOfWork, IValidator<PollRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }


        public async Task<ApiResponse<object?>> GetById(int id)
        {
            var messages = new List<ApiResponseMessage>();

            if (id <= 0 || id == null)
            {
                messages.Add(new ApiResponseMessage("validation", "Id", $" Id : {id} Is Required."));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status400BadRequest,
                       messages: messages);
            }

            var poll = await _unitOfWork.PollRepository.GetByIdAsync(id);

            if(poll == null)
            {
                messages.Add(new ApiResponseMessage("error", "Id", $"No Poll found with Id : {id}."));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status404NotFound,
                       messages: messages);
            }

            messages.Add(new ApiResponseMessage("success", "Poll fetched successfully."));
            return new ApiResponse<object?>(
            data: new object[] { poll },
            status: StatusCodes.Status200OK,
                    messages: messages)
            { }
            ;



        }

        public async Task<ApiResponse<object?>> GetList()
        {
            var messages = new List<ApiResponseMessage>();

            var polls = await _unitOfWork.PollRepository.GetListAsync();

            if(polls == null)
            {
                messages.Add(new ApiResponseMessage("error", "No Poll found."));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status404NotFound,
                       messages: messages);
            }

            var response = polls.Items.Adapt<List<PollResponse>>();

            messages.Add(new ApiResponseMessage("success", "GAP Types fetched Successfully."));
            return new ApiResponse<object?>(
            data: new object[] { response },
            status: StatusCodes.Status200OK,
                       messages: messages)
            { }
            ;
        }

        public async Task<ApiResponse<object?>> CreateAsync(PollRequest request)
        {
            var messages = new List<ApiResponseMessage>();

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

            var poll = request.Adapt<Poll>();

            await _unitOfWork.PollRepository.AddAsync(poll);
            await _unitOfWork.SaveChangesAsync();

            messages.Add(new ApiResponseMessage("success", "Poll Created successfully."));
            return new ApiResponse<object?>(
            status: StatusCodes.Status201Created,
                       messages: messages)
            { }
            ;
        }

        public async Task<ApiResponse<object?>> UpdateAsync(int id , PollRequest request)
        {
            var messages = new List<ApiResponseMessage>();

            if (id <= 0 || id == null)
            {
                messages.Add(new ApiResponseMessage("validation", "Id", $" Id : {id} Is Required."));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status400BadRequest,
                       messages: messages);
            }

            var poll = await _unitOfWork.PollRepository.GetByIdAsync(id);

            if (poll == null)
            {
                messages.Add(new ApiResponseMessage("error", "Id", $"No Poll found with Id : {id}."));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status404NotFound,
                       messages: messages);
            }

            poll.Title = request.Title;
            poll.Summary = request.Summary;
            poll.StarstAt = request.StarstAt;
            poll.EndsAt = request.EndsAt;

            await _unitOfWork.PollRepository.Update(poll);
            await _unitOfWork.SaveChangesAsync();

            messages.Add(new ApiResponseMessage("success", "Poll Updated successfully."));
            return new ApiResponse<object?>(
            status: StatusCodes.Status200OK,
            messages: messages);

        }

        public async Task<ApiResponse<object?>> DeleteAsync( int id )
        {
            var messages = new List<ApiResponseMessage>();

            if( id <= 0 || id == null)
            {
                messages.Add(new ApiResponseMessage("validation", "Id", $" Id : {id} Is Required."));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status400BadRequest,
                       messages: messages);
            }

            var poll = await _unitOfWork.PollRepository.GetByIdAsync(id);

            if(poll == null)
            {
                messages.Add(new ApiResponseMessage("error", "Id", $"No Poll found with Id : {id}."));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status404NotFound,
                       messages: messages);
            } 

            await _unitOfWork.PollRepository.Delete(poll);
            await _unitOfWork.SaveChangesAsync();

            messages.Add(new ApiResponseMessage("success", "Poll deleted successfully."));
            return new ApiResponse<object?>(
                   status: StatusCodes.Status200OK,
                   messages: messages);

        }

        public async Task<ApiResponse<object?>> TogglePublishStatusAsync(int id)
        {
            var messages = new List<ApiResponseMessage>();

            if (id <= 0 || id == null)
            {
                messages.Add(new ApiResponseMessage("validation", "Id", $" Id : {id} Is Required."));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status400BadRequest,
                       messages: messages);
            }

            var poll = await _unitOfWork.PollRepository.GetByIdAsync(id);

            if (poll == null)
            {
                messages.Add(new ApiResponseMessage("error", "Id", $"No Poll found with Id : {id}."));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status404NotFound,
                       messages: messages);
            }
            poll.Ispublished = !poll.Ispublished;
            await _unitOfWork.PollRepository.Update(poll);
            await _unitOfWork.SaveChangesAsync();

            messages.Add(new ApiResponseMessage("success", "Poll Published Updated successfully."));
            return new ApiResponse<object?>(
            status: StatusCodes.Status200OK,
            messages: messages);
        }
    }
}
