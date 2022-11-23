using CurrencyExchanger.Models;
using CurrencyExchanger.Models.Database;
using Serilog;
using System.Collections.Generic;

namespace CurrencyExchanger.Repositories.ExchangeRepo
{
    public class ExchangeServiceRepo : IExchangeServiceRepo
    {
        private readonly ExchangeServiceDBContext _dBContext;

        public ExchangeServiceRepo(ExchangeServiceDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        #region Adding Exchange
        public (bool result, string? message) AddExchnageTrade(Exchange exchange, int limit = 10)
        {
            try
            {
                var count = _dBContext.exchanges.Where(e => e.ClientId == exchange.ClientId)?.Count(e => e.PerformedAt > DateTime.UtcNow.AddHours(-1));
                if (count >= limit) return (false, $"client with id {exchange.ClientId} exceeded the exchange trade amount per hour");

                _dBContext.exchanges.Add(exchange);
                return (_dBContext.SaveChanges() == 1, null);

            }
            catch (Exception)
            {

                Log.Error("Something went wrong while trying to add Exchange Trade!");
                return (false, null);
            }
        }
        public void AddExchnageTrades(List<Exchange> exchanges)
        {
            try
            {
                _dBContext.exchanges.AddRange(exchanges);
                _dBContext.SaveChanges();

            }
            catch (Exception)
            {

                Log.Error("Something went wrong while trying to add Exchange Trades!");
            }
        }
        #endregion

        #region Removing Exchange
        public void RemoveExchnageTrade(Exchange exchange)
        {
            try
            {
                _dBContext.exchanges.Remove(exchange);
                _dBContext.SaveChanges();
            }
            catch (Exception)
            {

                Log.Error("Something went wrong while trying to remove Exchange Trade!");
            }

        }
        public void RemoveExchnageTrades(List<Exchange> exchanges)
        {
            try
            {
                _dBContext.exchanges.RemoveRange(exchanges);
                _dBContext.SaveChanges();
            }
            catch (Exception)
            {

                Log.Error("Something went wrong while trying to remove Exchange Trades!");
            }

        }
        #endregion

        #region Getting Exchange
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
        public IQueryable<Exchange>? GetAllExchangeTrades()
        {
            try
            {
                return _dBContext.exchanges;
            }
            catch (Exception)
            {

                Log.Error("Something went wrong while trying to retrieve All Exchange Trades!");
                return null;
            }
        }
        #endregion

    }
}
