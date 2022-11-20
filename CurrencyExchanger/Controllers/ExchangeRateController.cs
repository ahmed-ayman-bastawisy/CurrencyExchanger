using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Web;

namespace CurrencyExchanger.Controllers
{
    [Route("api")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly HttpClient _client;

        public ExchangeRateController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("ExchangeRatesAPI");
        }
        /// <summary>
        /// integrate with ExchangRates API, that returns real-time exchange rate data updated every 60 minutes, every 10 minutes or every 60 seconds.
        /// </summary>
        /// <param name="baseCurrency">Enter the three-letter currency code of your preferred base currency.</param>
        /// <param name="symbols">Enter a list of comma-separated currency codes to limit output currencies.</param>
        /// <returns>
        /// success boolean, 
        /// time stamp,
        /// base currency,
        /// date of the rates,
        /// rates dictionary with currency symbol as a key and the rate as a value
        /// </returns>
        [HttpGet("rates")]
        public async Task<ActionResult> GetRealTimeRates([FromQuery]string? baseCurrency = null, [FromQuery] string? symbols = null)
        {
            var uriBuilder = new UriBuilder(_client.BaseAddress + "/latest");
            var requestQueryStrings = HttpUtility.ParseQueryString(string.Empty);
            requestQueryStrings["symbols"] = symbols;
            requestQueryStrings["base"] = baseCurrency;
            uriBuilder.Query = requestQueryStrings.ToString();
            var response = await _client.GetAsync(uriBuilder.ToString());
            var responseConent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, responseConent);
        }

        /// <summary>
        /// Currency conversion endpoint, which integrate with ExchangeRates API,
        /// that can be used to convert any amount from one currency to another. 
        /// In order to convert currencies, 
        /// append the from and to parameters and set them to your preferred base and target currency codes.
        /// </summary>
        /// <param name="amount">The amount to be converted.</param>
        /// <param name="from">The three-letter currency code of the currency you would like to convert from.</param>
        /// <param name="to">The three-letter currency code of the currency you would like to convert to.</param>
        /// <param name="date">Specify a date (format YYYY-MM-DD) to use historical rates for this conversion.</param>
        /// <returns>
        /// date if not specified then today date, 
        /// info which holds the rate and time stamp.
        /// query which holds the query string parameters amount, from, and to,
        /// the result of the conversion,
        /// and success boolean
        /// </returns>
        [HttpGet("exchange")]
        public async Task<ActionResult> Convert([FromQuery] string amount, [FromQuery] string from, [FromQuery] string to, [FromQuery] string? date = null)
        {
            //bool requestValid = decimal.TryParse(amount , out decimal decAmount) && !string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to);
            
            var uriBuilder = new UriBuilder(_client.BaseAddress + "/convert");
            var requestQueryStrings = HttpUtility.ParseQueryString(string.Empty);
            requestQueryStrings["amount"] = amount;
            requestQueryStrings["from"] = from;
            requestQueryStrings["to"] = to;
            requestQueryStrings["date"] = date;
            uriBuilder.Query = requestQueryStrings.ToString();
            var response = await _client.GetAsync(uriBuilder.ToString());
            var responseConent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, responseConent);
        }
    }
}
