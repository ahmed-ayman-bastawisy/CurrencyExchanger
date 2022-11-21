using CurrencyExchanger.Models;
using CurrencyExchanger.Models.Database;
using Serilog;

namespace CurrencyExchanger.Repositories
{
    public class ExchangeServiceRepo : IExchangeServiceRepo
    {
        private readonly ExchangeServiceDBContext _dBContext;

        public ExchangeServiceRepo(ExchangeServiceDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public void AddExchnageTrade(Exchange exchange)
        {
            try
            {
                _dBContext.exchanges.Add(exchange);
                _dBContext.SaveChanges();
            }
            catch (Exception)
            {

                Log.Error("Something went wrong while trying to add Exchange Trade!");

            }
        }
        public IQueryable<Exchange>? GetClientExchangeTrades(long clientId)
        {
            try
            {
                return _dBContext.exchanges.Where(e => e.ClientId == clientId);
            }
            catch (Exception)
            {

                Log.Error("Something went wrong while trying to retrieve User Exchange Trades!");
                return null;
            }
        }

    }
}
