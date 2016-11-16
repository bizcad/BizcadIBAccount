using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBModels
{
    public class DailyActivityTrade
    {
        public int Id { get; set; }
        public string AcctID { get; set; }
        public int ConId { get; set; }
        /// <summary>
        /// In FinancialInstruementInformation this is the Description
        /// </summary>
        public string Symbol { get; set; }
        public string TradeDateTime { get; set; }
        public DateTime SettleDate { get; set; }
        public string Exchange { get; set; }
        public string TradeType  { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Multiplier { get; set; }
        public decimal Proceeds { get; set; }
        public decimal Comm { get; set; }
        public decimal Fee { get; set; }
        public string Code { get; set; }
        public string Filename { get; set; }
        

    }
}
