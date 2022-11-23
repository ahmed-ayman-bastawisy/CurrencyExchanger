using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using CurrencyExchanger.Models.APIModels;
using System.Text.Json;
using System.Net;
using CurrencyExchanger.Repositories;
using CurrencyExchanger.Models.Database;
using CurrencyExchanger.Services;
using CurrencyExchanger.Models.Constants;

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
        /// Get Exchanges rate from cache and if not found send API request to retrieve the latest rates
        /// </summary>
        /// <param name="baseCurrency">Enter the three-letter currency code of your preferred base currency.</param>
        /// <returns>RatesResponseModel which have : success boolean, 
        /// time stamp,
        /// base currency,
        /// date of the rates,
        /// rates dictionary with currency symbol as a key and the rate as a value
        /// </returns>

        [HttpGet("rates")]
        public async Task<ActionResult> GetRealTimeRates([FromQuery] string? baseCurrency = null, [FromQuery] string? symbols = null)
        {
            try
            {
                var result = _cache.TryGetValue(CacheKeys.RealTimeRatesKey, out RatesResponseModel? cachedRealTimeRates);
                RatesResponseModel? realTimeRates = cachedRealTimeRates?.Clone() as RatesResponseModel;
                int statusCode = (int)HttpStatusCode.OK;
                bool differentBase = !string.IsNullOrEmpty(baseCurrency) && realTimeRates?.baseCurrecny != baseCurrency;
                string message = string.Empty;
                if (result && realTimeRates != null)
                {
                    message = ResponseMessages.FoundInCache;
                    Log.Logger.Information("Rates" + message);
                    if (!string.IsNullOrEmpty(symbols))
                    {
                        Log.Logger.Information("Specific symbols required, start filtering the rates to get required symbols");
                        realTimeRates.FilterSymbols(symbols, baseCurrency);
                    }
                    if (differentBase)
                    {
                        Log.Logger.Information("Base currency is different from the cached rates, start calculating the new currency from the cached model");
                        if (!realTimeRates.GetExchangeRatesForDifferentCurrency(baseCurrency))
                            return BadRequest(new ResponseModel() { Message = "Cannot Get the base currency rate" });
                    }
                }
                else
                {
                    message = ResponseMessages.NotFoundInCache;
                    Log.Logger.Information("Rates" + message);

                    var response = await _client.SendRequestToRealTimeRates(baseCurrency);
                    statusCode = (int)response.statusCode;
                    var responseContent = response.response?.ResponseObj;

                    if (statusCode != (int)HttpStatusCode.OK)
                        return StatusCode(statusCode, new ResponseModel() { ResponseObj = JsonSerializer.Deserialize<object?>((string?)responseContent)});

                    RatesResponseModel LiveRealTimeRates = responseContent != null ? JsonSerializer.Deserialize<RatesResponseModel>((string)responseContent) : null;
                    realTimeRates = LiveRealTimeRates == null ? null : (RatesResponseModel)LiveRealTimeRates.Clone();
                    if (!string.IsNullOrEmpty(symbols))
                    {
                        Log.Logger.Information("Specific symbols required, start filtering the rates to get required symbols");
                        realTimeRates?.FilterSymbols(symbols, baseCurrency);
                    }
                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetPriority(CacheItemPriority.High)
                        .SetSlidingExpiration(TimeSpan.FromMinutes(20))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(29));

                    _cache.Set(CacheKeys.RealTimeRatesKey, LiveRealTimeRates, cacheEntryOptions);

                }
                return StatusCode(statusCode, new ResponseGeneric<RatesResponseModel>() { ResponseObj = realTimeRates, Message = message });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return BadRequest(new ResponseModel() { Message = "Something went wrong while trying to get exchange rates" });
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
        public async Task<ActionResult> Convert([FromQuery] ConvertQueryParameters parameters)
        {

            try
            {
                if (!decimal.TryParse(parameters.amount, out decimal fromAmount))
                {
                    string message = "amount not in a correct format";
                    Log.Error(message);
                    return BadRequest(new ResponseModel() { Message = message });
                }
                var clienttrades = _exchangeServiceRepo.GetClientExchangeTrades(parameters.clientId)?.Count(e => e.PerformedAt > DateTime.UtcNow.AddHours(-1));
                if (clienttrades >= 10)
                {
                    string message = $"client with id {parameters.clientId} exceeded the exchange trade amount per hour";
                    Log.Information(message);
                    return BadRequest(new ResponseModel() { Message = message });
                }
                Exchange? exchange = new()
                {
                    ClientId = parameters.clientId,
                    From = parameters.from,
                    To = parameters.to,
                    FromAmount = fromAmount,
                    PerformedAt = DateTime.UtcNow,
                };
                var exchangeRate = _cache.TryGetValue(CacheKeys.RealTimeRatesKey, out RatesResponseModel? realTimeRates) ? realTimeRates?.GetExchangeRate(parameters.from, parameters.to) : null;
                if (exchangeRate != null)
                {
                    exchange.ToAmount = fromAmount * exchangeRate.Rate;
                    exchange.Rate = exchangeRate.Rate;
                    exchange.Succeded = true;
                    _exchangeServiceRepo.AddExchnageTrade(exchange);
                }
                else
                {
                    var response = await GetRealTimeRates(parameters.from) as ObjectResult;
                    realTimeRates = (response?.Value as ResponseGeneric<RatesResponseModel>)?.ResponseObj ?? null;
                    exchangeRate = realTimeRates?.GetExchangeRate(parameters.from, parameters.to);
                    if (exchangeRate != null)
                    {
                        exchange.ToAmount = fromAmount * exchangeRate.Rate;
                        exchange.Rate = exchangeRate.Rate;
                        exchange.Succeded = true;
                        _exchangeServiceRepo.AddExchnageTrade(exchange);
                    }

                }
                if (exchange.Succeded) return Ok(new ResponseGeneric<Exchange>() { ResponseObj = exchange });
                else
                    return BadRequest(new ResponseGeneric<Exchange>() { ResponseObj = exchange, Message = "Something went wrong when trying to make exchange trade" });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return BadRequest(new ResponseModel() { Message = "Something went wrong while trying to make exchange trade" });
            }
        }
    }
}
