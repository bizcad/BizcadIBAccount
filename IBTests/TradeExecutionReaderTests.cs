using System;
using IBStatementReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBTests
{
    [TestClass]
    public class TradeExecutionReaderTests
    {
        [TestMethod]
        public void GetsTrades()
        {
            TradeExecutionReader reader = new TradeExecutionReader();
            var result = reader.GetTrades("ExecutionList.json");
            Assert.IsTrue(result.Count > 0);
        }
    }
}
