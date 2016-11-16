using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ScheduleD
{
    public interface IPositionInventory
    {
        void Add(OrderTransaction transaction);
        OrderTransaction Remove(string direction);
        OrderTransaction RemoveBuy();
        OrderTransaction RemoveSell();
        int BuysCount();
        int SellsCount();   
        int GetBuysQuantity(Symbol symbol);
        int GetSellsQuantity(Symbol symbol);
        Symbol GetSymbol();
        IList<OrderTransaction> GetBuys();
        IList<OrderTransaction> GetSells();
        
        Symbol Symbol { get; set; }
        
        
    }
}