namespace SurveyBasket.Application.Responses
{
    public class ApiResponseData<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
        public ApiResponseData(List<T> _Items, int _Total)
        {
            Items = _Items;
            Total = _Total;

        }
    }
}
