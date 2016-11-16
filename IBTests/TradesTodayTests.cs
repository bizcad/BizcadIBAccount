using System;
using IBStatementReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBTests
{
    [TestClass]
    public class TradesTodayTests
    {
        [TestMethod]
        public void GetsTrades()
        {
            TradesWindowReader reader = new TradesWindowReader();
            var result = reader.GetTrades("TradesToday.csv");
            Assert.IsTrue(result.Count > 0);
        }
    }
}
