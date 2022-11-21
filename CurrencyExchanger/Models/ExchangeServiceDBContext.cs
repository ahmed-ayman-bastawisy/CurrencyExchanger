using CurrencyExchanger.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchanger.Models
{
    public class ExchangeServiceDBContext : DbContext
    {
        public ExchangeServiceDBContext(DbContextOptions<ExchangeServiceDBContext> options) : base(options)
        {
        }
        public DbSet<Exchange> exchanges { get; set; }


    }
}
