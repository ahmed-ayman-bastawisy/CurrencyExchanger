using CurrencyExchanger.Models;
using CurrencyExchanger.Repositories.ExchangeRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchanger.Test
{
    public class TestServiceProvider
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public TestServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddDbContext<ExchangeServiceDBContext>(opt =>
            opt.UseSqlServer("Server=.;Database=ExchangeRate;Integrated Security=TRUE;Trusted_Connection=TRUE;TrustServerCertificate=True;"));
            serviceCollection.AddScoped<IExchangeServiceRepo, ExchangeServiceRepo>();
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
