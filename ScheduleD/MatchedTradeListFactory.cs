using System.Collections.Generic;

namespace ScheduleD
{
    public static class MatchedTradeListFactory
    {
        public static IList<MatchedTrade> Create()
        {
            return new List<MatchedTrade>();
        }
    }
}
