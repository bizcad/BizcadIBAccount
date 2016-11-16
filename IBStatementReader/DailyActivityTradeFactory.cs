using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBModels;

namespace IBStatementReader
{
    public class DailyActivityTradeFactory
    {
        public static DailyActivityTrade Create(Execution e, DBContract contract)
        {
            DailyActivityTrade trade = new DailyActivityTrade();
            trade.Id = 0;
            trade.AcctID = e.acctNumber;
            trade.Symbol = contract.LocalSymbol;
            trade.TradeDateTime = FormatExecutionTime(e.time);
            trade.Exchange = e.exchange;
            if (e.side == "BOT")
            {
                trade.Quantity = e.shares;
                trade.TradeType = "BUY";
            }
            else  // "SLD"
            {
                trade.Quantity = e.shares*-1;
                trade.TradeType = "SELL";
            }
            trade.Price = Math.Abs(Convert.ToDecimal(e.price));
            trade.Multiplier = Convert.ToDecimal(contract.Multiplier);
            trade.Comm = 0;
            trade.Proceeds = trade.Quantity * trade.Price * trade.Multiplier * -1;
            trade.ConId = contract.ConId;
            return trade;
        }
        private static string FormatExecutionTime(string etime)
        {

            if (etime.Contains("-"))
                return etime;
            StringBuilder sb = new StringBuilder();
            string t = etime.Trim();
            sb.Append(t.Substring(0, 4));
            sb.Append("-");
            sb.Append(t.Substring(4, 2));
            sb.Append("-");
            sb.Append(t.Substring(6));
            return sb.ToString();
        }
    }
}
