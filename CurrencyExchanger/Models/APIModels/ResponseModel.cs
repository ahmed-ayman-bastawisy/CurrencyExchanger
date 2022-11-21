namespace CurrencyExchanger.Models.APIModels
{
    public class ResponseGeneric<T>
    {
        public T? ResponseObj { get; set; }
        public string? Message { get; set; }
    }

    public class ResponseModel : ResponseGeneric<object>
    {

    }
}
