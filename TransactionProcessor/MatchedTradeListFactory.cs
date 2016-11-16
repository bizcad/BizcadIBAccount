using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleD;

namespace TransactionProcessor
{
    public static class MatchedTradeListFactory
    {
        public static IList<MatchedTrade> Create()
        {
            return new List<MatchedTrade>();
        }
    }
}
