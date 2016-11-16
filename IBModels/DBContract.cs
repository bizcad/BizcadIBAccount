using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBModels
{
    public class DBContract
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [Index("IX_ContractConIdIndex")]
        public int ConId { get; set; }
        [MaxLength(10)]
        public string Symbol { get; set; }
        [MaxLength(10)]
        public string SecType { get; set; }
        [MaxLength(10)]
        public string Expiry { get; set; }
        public double Strike { get; set; }
        [MaxLength(4)]
        public string Right { get; set; }
        [MaxLength(20)]
        public string Multiplier { get; set; }
        [MaxLength(50)]
        public string Exchange { get; set; }
        [MaxLength(3)]
        public string Currency { get; set; }
        [MaxLength(512)]
        public string LocalSymbol { get; set; }
        [MaxLength(50)]
        public string PrimaryExch { get; set; }
        [MaxLength(10)]
        public string TradingClass { get; set; }
        public bool IncludeExpired { get; set; }
        [MaxLength(128)]

        public string SecIdType { get; set; }
        [MaxLength(128)]
        public string SecId { get; set; }
        [MaxLength(512)]
        public string ComboLegsDescription { get; set; }
        public string ComboLegs { get; set; }
        public string UnderComp { get; set; }
    }
}
