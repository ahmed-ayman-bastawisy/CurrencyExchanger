using System.Text.Json.Serialization;

namespace CurrencyExchanger.Models.APIModels
{
    public class RatesResponseModel : ICloneable
    {
        public long timestamp { get; set; }
        public bool success { get; set; }
        public DateOnly date { get; set; }
        [JsonPropertyName("base")]
        public string baseCurrecny { get; set; }
        public Dictionary<string, decimal> rates { get; set; } = new();

        public object Clone()
        {
            var ratesResponseModel = (RatesResponseModel)MemberwiseClone();
            ratesResponseModel.rates = new Dictionary<string, decimal>(rates);
            return ratesResponseModel;
        }
    }
}
