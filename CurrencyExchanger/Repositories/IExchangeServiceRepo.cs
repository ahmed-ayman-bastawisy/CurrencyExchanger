using CurrencyExchanger.Models.Database;

namespace CurrencyExchanger.Repositories
{
    public interface IExchangeServiceRepo
    {
        void AddExchnageTrade(Exchange exchange);
        IQueryable<Exchange>? GetClientExchangeTrades(long clientId);
    }
}
