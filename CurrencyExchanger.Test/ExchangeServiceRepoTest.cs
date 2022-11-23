using CurrencyExchanger.Models.Database;
using CurrencyExchanger.Repositories.ExchangeRepo;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchanger.Test
{
    public class ExchangeServiceRepoTest : IClassFixture<TestServiceProvider>
    {
        private readonly IExchangeServiceRepo _exchangeServiceRepo;

        public ExchangeServiceRepoTest(TestServiceProvider serviceProvider)
        {
            _exchangeServiceRepo = serviceProvider.ServiceProvider.GetRequiredService<IExchangeServiceRepo>();
        }

        [Fact]
        public void ExchangeServiceRepoTest_AddExchnageTrade_ObjectAddedSuccessfully()
        {
            var newClient = _exchangeServiceRepo.GetAllExchangeTrades()?.Max(c => c.ClientId) ?? 0;
            newClient++;
            var excahnge = new Exchange()
            {
                ClientId = newClient,
                From = "EUR",
                To = "USD",
                FromAmount = 1,
                ToAmount = 1.2M,
                PerformedAt = DateTime.UtcNow,
                Rate = 1.2M,
                Succeded = true
            };
            var clientTotalExchangeTrades = _exchangeServiceRepo.GetClientExchangeTrades(newClient)?.Count() ?? 0;
            _exchangeServiceRepo.AddExchnageTrade(excahnge);
            var clientTotalExchangeTradesAfterAdding = _exchangeServiceRepo.GetClientExchangeTrades(newClient)?.Count() ?? 0;
            _exchangeServiceRepo.RemoveExchnageTrade(excahnge);
            Assert.Equal(clientTotalExchangeTrades + 1, clientTotalExchangeTradesAfterAdding);
        }

        [Fact]
        public void ExchangeServiceRepoTest_AddExchnageTradeExceedLimit_MustRefuseAdding()
        {
            var newClient = _exchangeServiceRepo.GetAllExchangeTrades()?.Max(c => c.ClientId) ?? 0;
            newClient++;
            List<Exchange> exchanges = new();
            for (int i = 0; i < 10; i++)
                exchanges.Add(new Exchange()
                {
                    ClientId = newClient,
                    From = "EUR",
                    To = "USD",
                    FromAmount = 1,
                    ToAmount = 1.2M,
                    PerformedAt = DateTime.UtcNow,
                    Rate = 1.2M,
                    Succeded = true
                });
            _exchangeServiceRepo.AddExchnageTrades(exchanges);

            var exchange = new Exchange()
            {
                ClientId = newClient,
                From = "EUR",
                To = "USD",
                FromAmount = 1,
                ToAmount = 1.2M,
                PerformedAt = DateTime.UtcNow,
                Rate = 1.2M,
                Succeded = true
            };

            var AddExceededExchangeTrade = _exchangeServiceRepo.AddExchnageTrade(exchange);
            var clientTotalExchangeTradesAfterAdding = _exchangeServiceRepo.GetClientExchangeTrades(newClient)?.Count() ?? 0;
            _exchangeServiceRepo.RemoveExchnageTrades(exchanges);
            Assert.True(clientTotalExchangeTradesAfterAdding == 10 && AddExceededExchangeTrade.result == false 
                && AddExceededExchangeTrade.message == $"client with id {newClient} exceeded the exchange trade amount per hour");
        }
    }
}
