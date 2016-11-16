using System;
using System.IO;
using IBStatementReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBTests
{
    [TestClass]
    public class ActivityStatementTests
    {
        private FileInfo activityFileInfo = new FileInfo(@"I:\Dropbox\ProWin16\InteractiveBrokers\ActivityStatement.20160819.html");
        private string contractinfotable = "tblContractInfoU1523863Body";
        private string codestable = "tblCodes_U1523863Body";
        private string navtable = "tblNAV_U1523863Body";
        private string markToMarkettable = "tblMtmPerfSumByUnderlying_U1523863Body";
        private string fifoPerfSum = "tblFIFOPerfSumByUnderlyingU1523863Body";
        private string _mtdytdPerfSum = "tblMTDYTDPerfSum_U1523863Body";

        #region Common
        [TestMethod]
        public void ReadsContractInfo()
        {
            TradeReportReader reader = new TradeReportReader(activityFileInfo);
            var ret = reader.ReadLinesFromTable(contractinfotable);
            Assert.IsTrue(ret.Count > 0);
        }
        #endregion
        #region ActivityStatment
        [TestMethod]
        public void ReadsCodesActivityFile()
        {
            //tblCodes_U1523863Body
            TradeReportReader reader = new TradeReportReader(activityFileInfo);
            var ret = reader.ReadLinesFromTable(codestable);
            Assert.IsTrue(ret.Count > 0);
        }
        [TestMethod]
        public void ReadsNav()
        {
            TradeReportReader reader = new TradeReportReader(activityFileInfo);
            var ret = reader.ReadLinesFromTable(navtable);
            Assert.IsTrue(ret.Count > 0);
        }
        [TestMethod]
        public void ReadsMTM()
        {
            TradeReportReader reader = new TradeReportReader(activityFileInfo);
            var ret = reader.ReadLinesFromTable(markToMarkettable);
            Assert.IsTrue(ret.Count > 0);
        }
        [TestMethod]
        public void ReadsPerfSum()
        {
            TradeReportReader reader = new TradeReportReader(activityFileInfo);
            var ret = reader.ReadLinesFromTable(fifoPerfSum);
            Assert.IsTrue(ret.Count > 0);
        }
        [TestMethod]
        public void ReadstblMtdytdPerfSum()
        {
            TradeReportReader reader = new TradeReportReader(activityFileInfo);
            var ret = reader.ReadLinesFromTable(_mtdytdPerfSum);
            Assert.IsTrue(ret.Count > 0);
        }
        #endregion


    }
}
