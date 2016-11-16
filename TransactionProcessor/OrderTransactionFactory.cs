using System;
using Models;
using TransactionProcessor;

namespace ScheduleD
{
    /// <summary>
    /// Creates an OrderTransaction
    /// </summary>
    public static class OrderTransactionFactory
    {
        public static OrderTransaction CreateScottrade(ScottradeTransaction s, int id)
        {
            // According to Scottrade a Buy is a negative amount (funds flow from my account to the seller's)
            //  However the Quantity filled is a negative number for Sell/Short and a positive for Buy/Long
            //  So multiply by -1 to give order value the correct sign

            #region "Create OrderTransaction"
            OrderTransaction t = new OrderTransaction
            {
                ActionId = s.ActionId,
                ActionNameUS = s.ActionNameUS,
                Amount = s.Amount,
                Broker = "Scottrade",
                CUSIP = s.CUSIP,
                Commission = s.Commission,
                Description = s.Description,
                Direction = TranslateOrderDirection(s.ActionId, s.Quantity),
                Exchange = "Unk",
                Fees = s.Fees,
                Id = id,
                Interest = s.Interest,
                Net = s.Amount + s.Commission + s.Fees,
                OrderId = 0,
                OrderType = OrderType.Limit,
                Price = s.Price,
                Quantity = s.Quantity,
                RecordType = s.RecordType,
                SettledDate = s.SettledDate,
                Symbol = s.Symbol,
                TaxLotNumber = s.TaxLotNumber,
                TradeDate = s.TradeDate,
                TradeNumber = s.TradeNumber.Length > 0 ? s.TradeNumber : "0"
            };
            #endregion

            return t;
        }

        private static OrderDirection TranslateOrderDirection(int actionId, int quantity)
        {
            switch (actionId)
            {
                case 48:                // Option sell to open
                case 49:                // Option sell to close
                case 13:                // Stock sell
                    return OrderDirection.Sell;
                case 24:                // Expired
                    return quantity > 0 ? OrderDirection.Buy : OrderDirection.Sell;
                case 47:                // Option buy to close
                case 46:                // Option buy to open
                case 1:                 // Stock buy
                    return OrderDirection.Buy;
                case 52:                // Option assigned
                    return quantity > 0 ? OrderDirection.Buy : OrderDirection.Sell;
                default:
                    return OrderDirection.Hold;
            }
        }

        public static OrderTransaction CreateInteractive(InteractiveBrokersTransaction s, int id)
        {

            #region "Create OrderTransaction"

            int actionid = CreateActionId(s);
            string actionNameUs = "Buy";
            if (s.Quantity < 0)
                actionNameUs = "Sell";

            OrderTransaction t = new OrderTransaction
            {
                ActionId = actionid,
                ActionNameUS = actionNameUs,
                Amount = s.Proceeds,
                Broker = "IB",
                CUSIP = "",
                Commission = s.CommissionFees,
                Description = s.Code,
                Direction = TranslateOrderDirection(actionid, s.Quantity),
                Exchange = "Unk",
                Fees = 0,
                Id = id,
                Interest = 0,
                Net = s.Basis,
                OrderId = 0,
                OrderType = OrderType.Limit,
                Price = s.Tprice,
                Quantity = s.Quantity,
                RecordType = "Trade",
                SettledDate = s.TransactionTime,        // I do not think I use the Settled Date anywhere so make it the trade date
                Symbol = s.Symbol,
                TaxLotNumber = "",
                TradeDate = s.TransactionTime,
                TradeNumber = "0"
            };
            #endregion

            return t;
        }
        /*
         *      case 48:                // Option sell to open
                case 49:                // Option sell to close
                case 13:                // Stock sell
                    return OrderDirection.Sell;
                case 24:                // Expired
                    return quantity > 0 ? OrderDirection.Buy : OrderDirection.Sell;
                case 47:                // Option buy to close
                case 46:                // Option buy to open
                case 1:                 // Stock buy
                    return OrderDirection.Buy;
                case 52:                // Option assigned
                    return quantity > 0 ? OrderDirection.Buy : OrderDirection.Sell;
         */
        private static int CreateActionId(InteractiveBrokersTransaction s)
        {
            if (s.Symbol.Length < 4)
            {
                if (s.Quantity > 0) // buy
                {
                    return 1;
                }
                else
                {
                    return 13;
                }
            }
            else
            {

                if (s.Quantity > 0) // buy
                {
                    return 46;
                }
                else  // sell
                {
                    return 47;
                }

            }
        }

    }
}