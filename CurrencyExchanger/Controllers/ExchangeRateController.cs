using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using CurrencyExchanger.Models;
using Serilog;
using CurrencyExchanger.Models.APIModels;
using System.Text.Json;
using System.Net;
using CurrencyExchanger.Repositories;
using CurrencyExchanger.Models.Database;
using CurrencyExchanger.Services;

namespace CurrencyExchanger.Controllers
{
    [Route("api")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly IMemoryCache _cache;
        private readonly IExchangeServiceRepo _exchangeServiceRepo;
        public ExchangeRateController(IHttpClientFactory clientFactory, IMemoryCache cache, IExchangeServiceRepo exchangeServiceRepo)
        {
            _client = clientFactory.CreateClient("ExchangeRatesAPI");
            _cache = cache;
            _exchangeServiceRepo = exchangeServiceRepo;
        }


        /// <summary>
        /// integrate with ExchangRates API, that returns real-time exchange rate data updated every 60 minutes, every 10 minutes or every 60 seconds,
        /// check the cache first and if the value exist and not outdated then the API return the response from the cache,
        /// otherwise it sends request to the ExchangRates API.
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
        public async Task<ActionResult?> SendRequestToRealTimeRates([FromQuery] string? baseCurrency = null, [FromQuery] string? symbols = null)
        {
            try
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
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return BadRequest(new ResponseModel() { Message = "Something went wrong while sending API request"});
            }
        }

        /// <summary>
        /// Get Exchanges rate from cache and if not found send API request to retrieve the latest rates
        /// </summary>
        /// <param name="baseCurrency">Enter the three-letter currency code of your preferred base currency.</param>
        /// <returns>RatesResponseModel which have : success boolean, 
        /// time stamp,
        /// base currency,
        /// date of the rates,
        /// rates dictionary with currency symbol as a key and the rate as a value
        /// </returns>
        private async Task<RatesResponseModel?> GetRealTimeRates([FromQuery] string? baseCurrency = null)
        {
            try
            {
                var result = _cache.TryGetValue(CacheKeys.RealTimeRatesKey, out RatesResponseModel? realTimeRates);
                if (result && realTimeRates != null)
                    Log.Logger.Information("Rates Found in the Cache");
                else
                {
                    Log.Logger.Information("Rates Not Found in the Cache");

                    var response = await SendRequestToRealTimeRates(baseCurrency) as ObjectResult;
                    realTimeRates = response?.StatusCode == (int)HttpStatusCode.OK && response?.Value != null ? JsonSerializer.Deserialize<RatesResponseModel>((string)response.Value) : null;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(29));

                    _cache.Set(CacheKeys.ReferenceEquals, realTimeRates, cacheEntryOptions);
                }
                return realTimeRates;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Currency conversion endpoint, which integrate with ExchangeRates API,
        /// that can be used to convert any amount from one currency to another. 
        /// In order to convert currencies, 
        /// append the from and to parameters and set them to your preferred base and target currency codes.
        /// </summary>
        /// <param name="clientId">The Client who perform the exchange.</param>
        /// <param name="amount">The amount to be converted.</param>
        /// <param name="from">The three-letter currency code of the currency you would like to convert from.</param>
        /// <param name="to">The three-letter currency code of the currency you would like to convert to.</param>
        /// <returns>
        /// exchange model which contains : client id who performed the exchange trade, 
        /// from which currency, to which currency
        /// the amount which will be converted, the converted amount
        /// when the exchange trade performed
        /// and whether the exchange trade succeeded or not
        /// </returns>
        [HttpGet("exchange")]
        public async Task<ActionResult> Convert([FromQuery] long clientId, [FromQuery] string amount, [FromQuery] string from, [FromQuery] string to)
        {

            try
            {
                if (decimal.TryParse(amount, out decimal fromAmount))
                {
                    string message = "amount not in a correct format";
                    Log.Error(message);
                    return BadRequest(new ResponseModel() { Message = message });
                }
                var clienttrades = _exchangeServiceRepo.GetClientExchangeTrades(clientId)?.Count(e => e.PerformedAt > DateTime.UtcNow.AddHours(-1));
                if (clienttrades >= 10)
                {
                    string message = $"client with id {clientId} exceeded the exchange trade amount per hour";
                    Log.Information(message);
                    return BadRequest(new ResponseModel() { Message = message });
                }
                Exchange? exchange = new()
                {
                    ClientId = clientId,
                    From = from,
                    To = to,
                    FromAmount = fromAmount,
                    PerformedAt = DateTime.UtcNow,
                };
                var exchangeRate = _cache.TryGetValue(CacheKeys.RealTimeRatesKey, out RatesResponseModel? realTimeRates) ? realTimeRates?.GetExchangeRate(from, to) : null;
                if (exchangeRate != null)
                {
                    exchange.ToAmount = fromAmount * exchangeRate.Rate;
                    exchange.Succeded = true;
                    _exchangeServiceRepo.AddExchnageTrade(exchange);
                }
                else
                {
                    realTimeRates = await GetRealTimeRates(from);
                    exchangeRate = realTimeRates?.GetExchangeRate(from, to);
                    if (exchangeRate != null)
                    {
                        exchange.ToAmount = fromAmount * exchangeRate.Rate;
                        exchange.Succeded = true;
                        _exchangeServiceRepo.AddExchnageTrade(exchange);
                    }

                }
                if (!exchange.Succeded) return Ok(new ResponseGeneric<Exchange>() { ResponseObj = exchange });
                else
                    return BadRequest(new ResponseGeneric<Exchange>() { ResponseObj = exchange, Message = "Something went wrong when trying to make exchange trade" });

                //bool requestValid = decimal.TryParse(amount , out decimal decAmount) && !string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to);

                //var uriBuilder = new UriBuilder(_client.BaseAddress + "/convert");
                //var requestQueryStrings = HttpUtility.ParseQueryString(string.Empty);
                //requestQueryStrings["amount"] = amount;
                //requestQueryStrings["from"] = from;
                //requestQueryStrings["to"] = to;
                //requestQueryStrings["date"] = date;
                //uriBuilder.Query = requestQueryStrings.ToString();
                //var response = await _client.GetAsync(uriBuilder.ToString());
                //var responseConent = await response.Content.ReadAsStringAsync();
                //return StatusCode((int)response.StatusCode, responseConent);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return BadRequest(new ResponseModel() { Message = "Something went wrong while trying to make exchange trade" });
            }
        }
    }
}
