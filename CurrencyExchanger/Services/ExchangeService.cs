using CurrencyExchanger.Models;
using CurrencyExchanger.Models.APIModels;
using CurrencyExchanger.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;
using System.Web;

namespace CurrencyExchanger.Services
{
    public static class ExchangeService
    {
        /// <summary>
        /// a function that calculate the rates for a certain currency if it is not the same as the base currency in the cached real time rates
        /// </summary>
        /// <param name="cachedRates">RatesResponseModel cachedRates that has the rates info for a certain currency </param>
        /// <param name="baseCurrency"> the currency to get the rates for</param>
        /// <returns>true if required base currency rates calculated successfully, and false otherwise</returns>
        public static bool GetExchangeRatesForDifferentCurrency(this RatesResponseModel cachedRates, string baseCurrency)
        {
            try
            {
                if (!cachedRates.rates.ContainsKey(baseCurrency)) return false; 
                Dictionary<string, decimal> calculatedRates = new();
                var basecurrencyRate = 1 / cachedRates.rates[baseCurrency];
                calculatedRates[cachedRates.baseCurrecny] = basecurrencyRate;
                foreach (var rate in cachedRates.rates)
                    calculatedRates[rate.Key] = Math.Round(basecurrencyRate * rate.Value,9);
                if (calculatedRates.ContainsKey(baseCurrency)) calculatedRates.Remove(baseCurrency);
                cachedRates.rates = calculatedRates;
                cachedRates.baseCurrecny = baseCurrency;
                return true;
            }
            catch (Exception)
            {
                Log.Error($"Something went wrong while calculating the exchange rates for {baseCurrency} currency");
                return false;
            }
        }

        /// <summary>
        /// a function that filters the rates based on the provided symbols
        /// </summary>
        /// <param name="rates">RatesResponseModel rates that has the rates info</param>
        /// <param name="symbols">provided symbols that will be used to filter the rates based on</param>
        /// <param name="baseCurrency">base currency required, passed because if it is different than the original base currency in the model,
        /// it will be added to the symbols list so it will be used while calculating the rates</param>
        /// <returns>true if rates filtered successfully, and false otherwise</returns>
        public static bool FilterSymbols(this RatesResponseModel rates, string symbols, string? baseCurrency)
        {
            try
            {
                var symbolsList = symbols.Split(',').ToList();
                if (!string.IsNullOrEmpty(baseCurrency) && rates.rates.ContainsKey(baseCurrency)) symbolsList.Add(baseCurrency);
                rates.rates = rates.rates.Where(k => symbolsList?.Contains(k.Key) == true).ToDictionary(r => r.Key, v => v.Value);
                return true;
            }
            catch (Exception)
            {
                Log.Error($"Something went wrong while filtering the exchange rates");
                return false;
            }
        }

        /// <summary>
        ///  a function for calculating exchange rate from one currency to another
        /// </summary>
        /// <param name="rates">rates dictionary</param>
        /// <param name="from">from which currency</param>
        /// <param name="to">to which currency</param>
        /// <returns>Exchange rate model which contains from which to which currency and the conversion rate</returns>
        public static ExchangeRate? GetExchangeRate(this RatesResponseModel rates, string from, string to)
        {
            try
            {
                if (rates.baseCurrecny == from)
                {
                    return new() { From = from, To = to, Rate = Math.Round(rates.rates[to], 9) };
                }
                else if (rates.baseCurrecny == to)
                {
                    return new() { From = to, To = from, Rate = Math.Round(1 / rates.rates[from], 9)};
                }
                else
                {
                    return new() { From = from, To = to, Rate = Math.Round(rates.rates[to] / rates.rates[from], 9) };
                }
            }
            catch (Exception)
            {

                Log.Error("Something went wrong while calculating the exchange rate");
                return null;
            }
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
        public static async Task<(HttpStatusCode statusCode, ResponseModel? response)> SendRequestToRealTimeRates(this HttpClient client, string? baseCurrency = null, string? symbols = null)
        {
            try
            {
                var uriBuilder = new UriBuilder(client.BaseAddress + "/latest");
                var requestQueryStrings = HttpUtility.ParseQueryString(string.Empty);
                requestQueryStrings["symbols"] = symbols;
                requestQueryStrings["base"] = baseCurrency;
                uriBuilder.Query = requestQueryStrings.ToString();
                var response = await client.GetAsync(uriBuilder.ToString());
                var responseConent = await response.Content.ReadAsStringAsync();
                return (response.StatusCode, new ResponseModel() { ResponseObj = responseConent });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return (HttpStatusCode.BadRequest, new ResponseModel() { Message = "Something went wrong while sending API request" });
            }
        }

        /// <summary>
        /// (Added For Testing Conversion API) Currency conversion endpoint, which integrate with ExchangeRates API,
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
        public static async Task<(HttpStatusCode statusCode, ResponseModel? response)> SendRequestToConvertAPI(this HttpClient client, string amount, string from, string to)
        {

            try
            {

                bool requestValid = decimal.TryParse(amount, out decimal decAmount) && !string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to);

                var uriBuilder = new UriBuilder(client.BaseAddress + "/convert");
                var requestQueryStrings = HttpUtility.ParseQueryString(string.Empty);
                requestQueryStrings["amount"] = amount;
                requestQueryStrings["from"] = from;
                requestQueryStrings["to"] = to;
                requestQueryStrings["date"] = null;
                uriBuilder.Query = requestQueryStrings.ToString();
                var response = await client.GetAsync(uriBuilder.ToString());
                var responseConent = await response.Content.ReadAsStringAsync();
                return (response.StatusCode, new ResponseModel() { ResponseObj = responseConent });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return (HttpStatusCode.BadRequest, new ResponseModel() { Message = "Something went wrong while trying to make exchange trade" });
            }
        }
    }
}
