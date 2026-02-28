namespace SurveyBasket.Application.Services.PollService
{
    public class PollService : IPollService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PollService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
