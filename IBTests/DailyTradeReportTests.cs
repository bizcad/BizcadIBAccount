using System;
using System.IO;
using HtmlAgilityPack;
using IBStatementReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBTests
{
    [TestClass]
    public class DailyTradeReportTests
    {
        //DailyTradeReport.20160111.html
        //DailyTradeReport.20160819.html
        private FileInfo DailyTradeFileInfo = new FileInfo(@"I:\Dropbox\ProWin16\InteractiveBrokers\DailyTradeReport.20161110.html");
        //file:///I:/Dropbox/ProWin16/InteractiveBrokers/ActivityStatement.20160819.html
        private string contractinfotable = "tblContractInfoU1523863Body";
        private string tradesbodytable = "tblTradesBody";
        private string tblCodestable = "tblCodesBody";

        #region DailyTradeReport
        [TestMethod]
        public void ReadsTradeLines()
        {
            TradeReportReader reader = new TradeReportReader(DailyTradeFileInfo);
            var ret = reader.ReadLinesFromTable(tradesbodytable);
            Assert.IsTrue(ret.Count > 0);
        }
        [TestMethod]
        public void ReadsTrades()
        {
            TradeReportReader reader = new TradeReportReader(DailyTradeFileInfo);
            var ret = reader.GetTrades();
            Assert.IsTrue(ret.Count > 0);
        }

        [TestMethod]
        public void ReadsCodesTradeFile()
        {
            //tblCodes_U1523863Body
            TradeReportReader reader = new TradeReportReader(DailyTradeFileInfo);
            var ret = reader.ReadLinesFromTable(tblCodestable);
            Assert.IsTrue(ret.Count > 0);
        }
        [TestMethod]
        public void GetsAccountInformationDictionary()
        {
            TradeReportReader reader = new TradeReportReader(DailyTradeFileInfo);
            var rows = reader.GetAccountInformationDictionary();
            Assert.IsNotNull(rows);
            Assert.IsTrue(rows.Count > 0);
        }
        #endregion
        #region Common
        [TestMethod]
        public void ReadsContractInfoFromTradesFile()
        {
            TradeReportReader reader = new TradeReportReader(DailyTradeFileInfo);
            var ret = reader.ReadLinesFromTable(contractinfotable);
            Assert.IsTrue(ret.Count > 0);
        }
        #endregion
        //[TestMethod]
        //public void GetsTheAccountInformationDiv()
        //{
        //    TradeReportReader reader = new TradeReportReader(DailyTradeFileInfo);
        //    HtmlNode div = reader.GetDiv(@"tblAccountInformation_U1523863Body");
        //    Assert.IsNotNull(div);
        //    Assert.IsTrue(div.InnerText.Count > 0);
        //}

        //[TestMethod]
        //public void GetsGetAccountInformationDictionary()
        //{
        //    TradeReportReader reader = new TradeReportReader(DailyTradeFileInfo);
        //    var rows = reader.GetAccountInformationDictionary();
        //    Assert.IsNotNull(rows);
        //    Assert.IsTrue(rows.Count > 0);
        //}

        

    }
}
