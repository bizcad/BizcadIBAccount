using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBModels;
using ScheduleD;

namespace IBStatementReader
{
    public static class OrderTransactionListFactory
    {
        public static List<OrderTransaction> Create(List<DailyActivityTrade> trades)
        {
            List<OrderTransaction> orderTransactionList = new List<OrderTransaction>();
            foreach (var trade in trades)
            {
                OrderTransaction trans = new OrderTransaction();
                trans.Id = trade.Id;
                trans.Symbol = trade.Symbol;
                trans.ConId = trade.ConId;
                trans.FileName = trade.Filename;
                trans.TradeDate = TradeDateFromString(trade.TradeDateTime);
                trans.SettledDate = trade.SettleDate;
                trans.Exchange = trade.Exchange;
                trans.Direction = trade.TradeType == "BUY" ? OrderDirection.Buy : OrderDirection.Sell;
                trans.Quantity = trade.Quantity;
                trans.Price = trade.Price;
                trans.Amount = trade.Proceeds;
                trans.Commission = trade.Comm;
                trans.Fees = trade.Fee;
                trans.Code = trade.Code;
                trans.Broker = "IB";
                trans.Net = trans.Amount + trans.Commission + trans.Fees;
                trans.OrderId = trade.Id;
                trans.OrderType = OrderType.Limit;

                orderTransactionList.Add(trans);
            }

            // Save the order transaction list sorted by trade date
            List<OrderTransaction> otl = orderTransactionList.OrderBy(n => n.TradeDate).ToList();
            return otl;
        }
        private static DateTime TradeDateFromString(string delimited)
        {
            string s = delimited.Replace(",", string.Empty);

            DateTime retval = DateTime.Parse(s);
            return retval;
        }
    }
}
