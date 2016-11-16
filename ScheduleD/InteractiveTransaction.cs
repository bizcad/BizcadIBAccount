using System;

namespace ScheduleD
{
    public class InteractiveTransaction
    {
        public int Id { get; set; }
        public string ClientAccountID { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public DateTime TradeDate { get; set; }
        public int Quantity { get; set; }
        public decimal TradePrice { get; set; }
        public decimal IBCommission { get; set; }
        public string CUSIP { get; set; }
        public string UnderlyingSymbol { get; set; }
        public int TradeID { get; set; }
        public DateTime TradeTime { get; set; }
        public DateTime SettleDateTarget { get; set; }
        public string TransactionType { get; set; }
        public string Exchange { get; set; }
        public decimal Proceeds { get; set; }
        public decimal CostBasis { get; set; }
        public decimal FifoPnlRealized { get; set; }
        public decimal Strike { get; set; }
        public DateTime Expiry { get; set; }
        public string PutCall { get; set; }
        public decimal TradeMoney { get; set; }
        public int Taxes { get; set; }
        public DateTime ReportDate { get; set; }
        public string SecurityID { get; set; }
        public string SecurityIDType { get; set; }
        public int Conid { get; set; }
    }
}
