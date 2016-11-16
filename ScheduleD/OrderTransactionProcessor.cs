using System;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleD
{
    public class OrderTransactionProcessor
    {

        public IList<OrderTransaction> TransactionHistory { get; set; }
        public IList<OrderTransaction> OpenTrades { get; set; }

        public IList<IPositionInventory> OpenPositions { get; set; }
        public IList<MatchedTrade> Trades { get; set; }

        public decimal LastTradeCommission { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TotalFees { get; set; }
        public decimal TotalProfit { get; set; }
        private int tradeId = 0;


        public OrderTransactionProcessor()
        {
            TransactionHistory = new List<OrderTransaction>();
            OpenTrades = new List<OrderTransaction>();

            OpenPositions = new List<IPositionInventory>();
            Trades = new List<MatchedTrade>();
        }

        public void ProcessTransaction(OrderTransaction trans)
        {
            if (trans.Quantity == 286)
                System.Diagnostics.Debug.WriteLine("here");

            IPositionInventory openPosition = OpenPositions.FirstOrDefault(p => p.GetSymbol() == trans.Symbol);
            if (openPosition == null)
            {
                openPosition = OpenPosition(trans, PositionInventoryMethod.Fifo);

                OpenPositions.Add(openPosition);
            }
            else
            {
                OrderTransaction transaction = ResolvePosition(openPosition, trans);
                if (openPosition.BuysCount() == 0 && openPosition.SellsCount() == 0)
                {
                    OpenPositions.Remove(openPosition);
                }
            }
        }

        private OrderTransaction ResolvePosition(IPositionInventory position, OrderTransaction trans)
        {
            OrderTransaction buytrans = new OrderTransaction();
            OrderTransaction selltrans = new OrderTransaction();
            OrderTransaction l = new OrderTransaction();





            if (trans.Direction == OrderDirection.Buy)
            {
                if (position.SellsCount() > 0)
                {
                    selltrans = position.RemoveSell();
                    if (Math.Abs(trans.Quantity) == Math.Abs(selltrans.Quantity))
                    {
                        return CreateTrade(trans, selltrans);
                    }
                    var unitcost = trans.Price;

                    // if the trans (the buy) qty is greater than the selltrans qty, split the buy
                    if (trans.Quantity > Math.Abs(selltrans.Quantity))
                    {
                        #region "Trans is Buy and buy greater than sell"
                        //var unitcost = Math.Abs(trans.Amount / trans.Quantity);
                        // Don't touch the sell trans as it will be resolved.

                        // split the (buy)trans to equalize with the selltrans quantity
                        // Create the passalong as a buy
                        l = CopyTransaction(trans);
                        l.Direction = OrderDirection.Buy;
                        l.Quantity = trans.Quantity + selltrans.Quantity;   // buy quantity will be positive
                        l.Amount = unitcost * l.Quantity * -1;              // the buy will have a negative cash flow amount
                        l.Commission = 0;
                        l.Fees = 0;
                        l.Interest = 0;
                        l.Net = l.Amount;

                        // create a new smaller buy
                        buytrans = CopyTransaction(trans);
                        buytrans.Quantity = Math.Abs(selltrans.Quantity);   // match the buy with the sell quantity as a positive
                        buytrans.Amount = unitcost * buytrans.Quantity * -1;    // a buy has a negative cash flow
                        buytrans.Net = buytrans.Amount + buytrans.Commission + buytrans.Fees; // assign all the commission and fees to the buy trans.

                        CreateTrade(buytrans, selltrans);
                        return ResolvePosition(position, l);

                        #endregion
                    }
                    else
                    {
                        #region "Trans is Buy and sell greater than buy"
                        // Split the sell and create a passalong as a sell
                        l = CopyTransaction(selltrans);
                        l.Quantity = selltrans.Quantity + trans.Quantity; // sell quantity will be negative
                        l.Amount = unitcost * l.Quantity * -1;              // the sell will have positive cash flow
                        l.Commission = 0;
                        l.Fees = 0;
                        l.Interest = 0;
                        l.Net = l.Amount;

                        // adjust the sell 
                        selltrans.Quantity = -1 * (Math.Abs(selltrans.Quantity) - Math.Abs(l.Quantity));       // sell qty is negative
                        selltrans.Amount = unitcost * Math.Abs(selltrans.Quantity);                            // sell amount is positive
                        selltrans.Net = selltrans.Amount + selltrans.Commission + selltrans.Fees;

                        CreateTrade(trans, selltrans);
                        return ResolvePosition(position, l);

                        #endregion
                    }
                }
                else
                {
                    position.Add(trans);
                }
            }
            else
            {
                // trans.Direction == OrderDirection.Sell
                if (position.BuysCount() > 0)
                {
                    buytrans = position.RemoveBuy();
                    if (Math.Abs(trans.Quantity) == Math.Abs(buytrans.Quantity))
                    {
                        return CreateTrade(buytrans, trans);
                    }

                    Decimal unitcost = trans.Price;
                    if (Math.Abs(trans.Quantity) > buytrans.Quantity)
                    {
                        #region "Trans is sell and sell is greater than buy"
                        //unitcost = Math.Abs(trans.Amount / trans.Quantity);
                        // split the trans and pass along the remaining as a sell
                        l = CopyTransaction(trans);             // l is a selltrans
                        l.Direction = OrderDirection.Sell;      // and convert the sell to a sell
                        l.Quantity = -1 * (Math.Abs(trans.Quantity) - Math.Abs(buytrans.Quantity));            // a sell quantity is negative
                        l.Amount = unitcost * Math.Abs(l.Quantity);                                          // amount will be positive
                        l.Commission = 0;
                        l.Fees = 0;
                        l.Interest = 0;
                        l.Net = l.Amount;

                        selltrans = CopyTransaction(trans);
                        selltrans.Quantity = -1 * (Math.Abs(trans.Quantity) - Math.Abs(l.Quantity));            // sell qty is negative
                        selltrans.Amount = unitcost * Math.Abs(selltrans.Quantity);                             // amount will be positive cash flow
                        selltrans.Net = selltrans.Amount + selltrans.Commission + selltrans.Fees;

                        CreateTrade(buytrans, selltrans);
                        return ResolvePosition(position, l);


                        #endregion
                    }
                    else
                    {
                        #region "Trans is sell and buy is greater than sell"
                        // split the (buy)trans and create a passalong as a buy
                        // l is a buy
                        l = CopyTransaction(trans);
                        l.Direction = OrderDirection.Buy;                   // and change it to a sell
                        l.Quantity = buytrans.Quantity + trans.Quantity;    // buy quantity will be positive
                        l.Amount = unitcost * l.Quantity * -1;                   // buy amount will be negative
                        l.Commission = 0;
                        l.Fees = 0;
                        l.Interest = 0;
                        l.Net = l.Amount;

                        buytrans.Quantity = Math.Abs(buytrans.Quantity) - Math.Abs(l.Quantity);     // buy qty will be positive
                        buytrans.Amount = unitcost * buytrans.Quantity * -1;                        // buy amount will be negative cash flow
                        buytrans.Net = buytrans.Amount + buytrans.Commission + buytrans.Fees;

                        CreateTrade(buytrans, trans);
                        return ResolvePosition(position, l);

                        #endregion
                    }
                }
                else
                {
                    position.Add(trans);
                }
            }
            return l;


        }

        private OrderTransaction CreateTrade(OrderTransaction buytrans, OrderTransaction selltrans)
        {
            var l = new OrderTransaction();
            if (buytrans == null && selltrans == null)
                return l;



            // final check to make sure the buys give cash
            //   and sells get cash
            //  A zero value for amount is ok because of options expiring worthless
            //  There may still be a commission
            if (buytrans != null && buytrans.Amount > 0)
                throw new ArgumentException("Buy trans amount > 0");
            if (selltrans.Amount < 0)
                throw new ArgumentException("Sell trans amount < 0");

            // The Buy quantity should be a -number
            if (buytrans != null && buytrans.Quantity <= 0)
                throw new ArgumentException("Buy quantity <= 0");
            // The Sell quantity should be a +number
            if (selltrans.Quantity >= 0)
                throw new ArgumentException("Sell quantity >= 0");


            MatchedTrade trade = new MatchedTrade
            {
                Id = ++tradeId,
                Symbol = buytrans.Symbol,
                DescriptionOfProperty = $"{buytrans.Quantity} {buytrans.Symbol}",
                DateAcquired = buytrans.TradeDate,
                DateSoldOrDisposed = selltrans.TradeDate,
                AdjustmentAmount = 0,
                ReportedToIrs = true,
                ReportedToMe = true,
                Brokerage = selltrans.Broker,
                BuyOrderId = buytrans.OrderId,
                SellOrderId = selltrans.OrderId
            };


            // Buy quantities are positive, sell quantities are negative
            // Buy Amount is negative, sell Amount is positive.
            // commission and fees are always negative
            if (Math.Abs(buytrans.Quantity) == Math.Abs(selltrans.Quantity))
            {
                trade.Quantity = buytrans.Quantity;
                trade.Proceeds = Math.Abs(selltrans.Net);
                trade.CostOrBasis = Math.Abs(buytrans.Net);

                //Long Term Short Term
                TimeSpan diff = trade.DateSoldOrDisposed.Subtract(trade.DateAcquired);
                if (diff.TotalDays > 365)
                    trade.LongTermGain = true;

                //if (trade.DateSoldOrDisposed.Year == 2014)
                TotalCommission += buytrans.Commission;
                TotalCommission += selltrans.Commission;
                LastTradeCommission = buytrans.Commission + selltrans.Commission;
                TotalProfit += trade.GainOrLoss;
                if (Math.Abs(trade.GainOrLoss) > 10000)
                    throw new Exception("Invalid gain or loss");
                trade.CumulativeProfit = TotalProfit;
                Trades.Add(trade);

            }
            return l;
        }


        public IPositionInventory OpenPosition(OrderTransaction trans, PositionInventoryMethod positionResolution)
        {
            IPositionInventory position;

            if (positionResolution == PositionInventoryMethod.Fifo)
                position = new PositionInventoryFifo();
            else
            {
                position = new PositionInventoryLifo();
            }
            position.Add(trans);
            return position;
        }
        private OrderTransaction CopyTransaction(OrderTransaction trans)
        {
            OrderTransaction l = new OrderTransaction();
            l.Symbol = trans.Symbol;
            l.Exchange = trans.Exchange;
            l.Broker = trans.Broker;
            l.Quantity = trans.Quantity;
            l.Price = trans.Price;
            l.ActionNameUS = trans.ActionNameUS;
            l.TradeDate = trans.TradeDate;
            l.SettledDate = trans.SettledDate;
            l.Interest = trans.Interest;
            l.Amount = trans.Amount;
            l.Commission = trans.Commission;
            l.Fees = trans.Fees;
            l.CUSIP = trans.CUSIP;
            l.Description = trans.Description;
            l.ActionId = trans.ActionId;
            l.TradeNumber = trans.TradeNumber;
            l.RecordType = trans.RecordType;
            l.TaxLotNumber = trans.TaxLotNumber;

            l.OrderType = trans.OrderType;
            l.OrderId = trans.OrderId;
            l.Direction = trans.Direction;


            return l;
        }
        public bool IsLong(Symbol symbol)
        {
            if (OpenPositions.Count > 0)
            {
                var position = OpenPositions.FirstOrDefault(b => b.Symbol == symbol);
                if (position != null && position.BuysCount() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsShort(Symbol symbol)
        {
            if (OpenPositions.Count > 0)
            {
                var position = OpenPositions.FirstOrDefault(b => b.Symbol == symbol);
                if (position != null && position.SellsCount() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        internal int GetPosition(Symbol symbol)
        {
            var openPosition = OpenPositions.FirstOrDefault();
            if (openPosition != null && openPosition.GetBuysQuantity(symbol) > 0)
                return openPosition.GetBuysQuantity(symbol);

            if (openPosition != null && openPosition.GetSellsQuantity(symbol) < 0)
                return openPosition.GetSellsQuantity(symbol);

            return 0;
        }

        public decimal CalculateLastTradePandL(Symbol symbol)
        {
            var matchedTrade = Trades.LastOrDefault(p => p.Symbol == symbol);
            if (matchedTrade != null)
                return matchedTrade.GainOrLoss;
            return 0;
        }
    }
}
