namespace IBModels
{
    /// <summary>
    /// An analog for a IB Contract.  The difference is that combolegs and undercomp are json strings instead of objects.
    /// </summary>
    public class Contract
    {
        public int ConId { get; set; }
        public string Symbol { get; set; }
        public string SecType { get; set; }
        public string Expiry { get; set; }
        public double Strike { get; set; }
        public string Right { get; set; }
        public string Multiplier { get; set; }
        public string Exchange { get; set; }
        public string Currency { get; set; }
        public string LocalSymbol { get; set; }
        public string PrimaryExch { get; set; }
        public string TradingClass { get; set; }
        public bool IncludeExpired { get; set; }
        public string SecIdType { get; set; }
        public string SecId { get; set; }
        public string ComboLegsDescription { get; set; }
        public string ComboLegs { get; set; }
        public string UnderComp { get; set; }

    }
}
