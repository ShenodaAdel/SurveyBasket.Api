namespace SurveyBasket.Application.Responses
{
    public class ApiResponseData<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
        public ApiResponseData(List<T> items, int total)
        {
            Items = items;
            Total = total;
        }
    }
}
