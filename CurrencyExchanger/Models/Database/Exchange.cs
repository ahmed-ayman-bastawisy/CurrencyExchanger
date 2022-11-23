using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyExchanger.Models.Database
{
    public class Exchange
    {
        [Key]
        public long Id { get; set; }
        public long ClientId { get; set; }
        public DateTime PerformedAt { get; set; }
        [Column(TypeName = "[decimal](20, 7)")]
        public decimal Rate { get; set; }
        [Column(TypeName = "[decimal](20, 7)")]
        public decimal FromAmount { get; set; }
        [Column(TypeName = "[decimal](20, 7)")]
        public decimal ToAmount { get; set; } = 0;
        [Column(TypeName = "[nvarchar](3)")]
        public string From { get; set; }
        [Column(TypeName = "[nvarchar](3)")]
        public string To { get; set; }
        public bool Succeded { get; set; } = false;

    }
}
