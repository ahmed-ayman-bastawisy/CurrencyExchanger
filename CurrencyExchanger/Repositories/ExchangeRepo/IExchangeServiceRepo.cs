using CurrencyExchanger.Models.Database;

namespace CurrencyExchanger.Repositories.ExchangeRepo
{
    public interface IExchangeServiceRepo
    {
        (bool result, string? message) AddExchnageTrade(Exchange exchange, int limit = 10);
        void AddExchnageTrades(List<Exchange> exchanges);
        void RemoveExchnageTrade(Exchange exchange);
        void RemoveExchnageTrades(List<Exchange> exchanges);
        IQueryable<Exchange>? GetClientExchangeTrades(long clientId);
        IQueryable<Exchange>? GetAllExchangeTrades();
    }
}
