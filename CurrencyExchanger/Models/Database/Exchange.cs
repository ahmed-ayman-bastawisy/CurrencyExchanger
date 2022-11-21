using System.ComponentModel.DataAnnotations;

namespace CurrencyExchanger.Models.Database
{
    public class Exchange
    {
        [Key]
        public long Id { get; set; }
        public long ClientId { get; set; }
        public DateTime PerformedAt { get; set; }
        public bool Historical { get; set; } = false;
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; } = 0;
        public string From { get; set; }
        public string To { get; set; }
        public bool Succeded { get; set; } = false;

    }
}
