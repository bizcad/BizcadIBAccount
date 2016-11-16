using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using IBModels;
using Newtonsoft.Json;

namespace IBStatementReader
{
    public static class ContractConverter
    {
        public static DBContract Convert(IBModels.Contract c)
        {
            DBContract d = new DBContract
            {
                ConId = c.ConId,
                ComboLegs = c.ComboLegs,
                ComboLegsDescription = c.ComboLegsDescription,
                Currency = c.Currency,
                Exchange = c.Exchange,
                Expiry = c.Expiry,
                IncludeExpired = c.IncludeExpired,
                LocalSymbol = c.LocalSymbol,
                Multiplier = c.Multiplier,
                PrimaryExch = c.PrimaryExch,
                Right = c.Right,
                Strike = c.Strike,
                SecId = c.SecId,
                SecIdType = c.SecIdType,
                SecType = c.SecType,
                TradingClass = c.TradingClass
            };
            d.ComboLegsDescription = c.ComboLegsDescription;
            d.UnderComp = c.UnderComp;
            
            return d;
        }
    }
}
