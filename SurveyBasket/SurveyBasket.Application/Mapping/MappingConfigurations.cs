namespace SurveyBasket.Application.Mapping
{
    public class MappingConfigurations : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Poll, PollResponse>();               
        }
    }
}
