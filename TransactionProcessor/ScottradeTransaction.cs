using System;

namespace TransactionProcessor
{
    public class ScottradeTransaction
    {
        public string Symbol { get; set; }	
        public int Quantity	 { get; set; }	
        public decimal Price  { get; set; }	
        public string ActionNameUS { get; set; }	
        public DateTime TradeDate { get; set; }	
        public DateTime SettledDate	 { get; set; }	
        public decimal Interest { get; set; }	
        public decimal Amount { get; set; }	
        public decimal Commission { get; set; }	
        public decimal Fees { get; set; }	
        public string CUSIP { get; set; }	
        public string Description { get; set; }	
        public int ActionId { get; set; }
        public string TradeNumber { get; set; }
        public string RecordType { get; set; }	
        public string TaxLotNumber { get; set; }
        public string Comment { get; set; }	
    }
}
