namespace ScheduleD
{
    public enum StockState
    {
        shortPosition,  // The Portfolio has short position in this bar.
        longPosition,   // The Portfolio has long position in this bar.
        noInvested,     // The Portfolio hasn't any position in this bar.
        orderSent       // An order has been sent in this same bar, skip analysis.
    };

    public enum OrderSignal
    {
        goShort, goLong,                // Entry to the market orders.
        goShortLimit, goLongLimit,      // Entry with limit order.
        closeShort, closeLong,          // Exit from the market orders.
        revertToShort, revertToLong,    // Reverse a position when in the wrong side of the trade.
        doNothing
    };

    public enum RevertPositionCheck
    {
        vsTrigger,
        vsClosePrice,
    }

    public enum PositionInventoryMethod
    {
        Lifo, Fifo
    }
    public enum OrderType
    {
        /// <summary>
        /// Market Order Type
        /// </summary>
        Market,

        /// <summary>
        /// Limit Order Type
        /// </summary>
        Limit,

        /// <summary>
        /// Stop Market Order Type - Fill at market price when break target price
        /// </summary>
        StopMarket,

        /// <summary>
        /// Stop limit order type - trigger fill once pass the stop price; but limit fill to limit price.
        /// </summary>
        StopLimit,

        /// <summary>
        /// Market on open type - executed on exchange open
        /// </summary>
        MarketOnOpen,

        /// <summary>
        /// Market on close type - executed on exchange close
        /// </summary>
        MarketOnClose
    }

    public enum OrderDirection
    {

        /// <summary>
        /// Buy Order 
        /// </summary>
        Buy,

        /// <summary>
        /// Sell Order
        /// </summary>
        Sell,

        /// <summary>
        /// Default Value - No Order Direction
        /// </summary>
        /// <remarks>
        /// Unfortunately this does not have a value of zero because
        /// there are backtests saved that reference the values in this order
        /// </remarks>
        Hold
    }

}
