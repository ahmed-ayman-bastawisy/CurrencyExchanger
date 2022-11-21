namespace CurrencyExchanger.Models.APIModels
{
    public class ExchangeResponseModel
    {
        public DateOnly date { get; set; }
        public bool historical { get; set; }
        public ExchangeInfo info { get; set; } = new();
        public ExchangeQuery query { get; set; } = new();
        public decimal result { get; set; }
        public bool success { get; set; }
    }

    public class ExchangeInfo
    {
        public long timestamp { get; set; }
        public decimal rate { get; set; }

    }

    public class ExchangeQuery
    {
        public string from { get; set; }
        public string to { get; set; }
        public decimal amount { get; set; }

    }
}
