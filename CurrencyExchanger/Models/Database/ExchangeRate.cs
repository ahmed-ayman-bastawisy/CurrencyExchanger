using System.ComponentModel.DataAnnotations;

namespace CurrencyExchanger.Models.Database
{
    public class ExchangeRate
    {
        [Key]
        public long Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Rate { get; set; }
    }
}
