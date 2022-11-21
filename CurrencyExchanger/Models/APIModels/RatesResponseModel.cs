using System.Text.Json.Serialization;

namespace CurrencyExchanger.Models.APIModels
{
    public class RatesResponseModel
    {
        public long timestamp { get; set; }
        public bool success { get; set; }
        public DateOnly date { get; set; }
        [JsonPropertyName("base")]
        public string baseCurrecny { get; set; }
        public Dictionary<string, string> rates { get; set; } = new();
    }
}
