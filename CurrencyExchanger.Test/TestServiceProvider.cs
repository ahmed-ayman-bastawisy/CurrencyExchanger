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
            //Docker
            //opt.UseSqlServer("Server=tcp:172.17.0.1,14330;Database=ExchangeRate;User=sa;Password=A-a_7276P@a$$wRD!;TrustServerCertificate=True;")
            //local
            opt.UseSqlServer("Server=.;Database=ExchangeRate;Integrated Security=TRUE;Trusted_Connection=TRUE;TrustServerCertificate=True;"));
            serviceCollection.AddScoped<IExchangeServiceRepo, ExchangeServiceRepo>();
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
