using CurrencyExchanger.Models.APIModels;
using CurrencyExchanger.Models.Database;
using Serilog;

namespace CurrencyExchanger.Services
{
    public static class ExchangeService
    {
        private static object _lockObj = new();

        public static ExchangeRate? GetExchangeRate(this RatesResponseModel rates, string from, string to)
        {
            try
            {
                lock (_lockObj)
                {
                    if (rates.baseCurrecny == from)
                    {
                        if (decimal.TryParse(rates.rates[to], out decimal rate))
                            return new() { From = from, To = to, Rate = rate };
                    }
                    else if (rates.baseCurrecny == to)
                    {
                        if (decimal.TryParse(rates.rates[from], out decimal rate))
                            return new() { From = to, To = from, Rate = 1 / rate };
                    }
                    else
                    {
                        if (decimal.TryParse(rates.rates[from], out decimal sourceRate) && decimal.TryParse(rates.rates[to], out decimal targetRate))
                            return new() { From = from, To = to, Rate = (1 / sourceRate) * targetRate };
                    }
                    return null;
                }
            }
            catch (Exception)
            {

                Log.Error("Something went wrong while calculating the exchange rate");
                return null;
            }
        }
    }
}
