using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using HtmlAgilityPack;
using IBToolBox;
using IBModels;

namespace IBStatementReader
{
    public class TradeReportReader
    {
        public List<DailyActivityTrade> Trades;
        public List<ContractInformation> ContractInformationList;
        public List<DBContract> Contracts;

        public FileInfo _activityFileInfo { get; set; }

        private string contractinfotable = "tblContractInfoU1523863Body";
        private string tradesbodytable = "tblTradesBody";
        //private string tblCodestable = "tblCodesBody";
        public ContractsReader contractsReader;

        public TradeReportReader(FileInfo activityFileInfo)
        {
            _activityFileInfo = activityFileInfo;
            contractsReader = new ContractsReader();
            contractsReader.ReadContracts();
        }

        public Dictionary<string, string> AccountInformationDictionary { get; set; }
        public HtmlDocument document { get; set; }
        public HtmlWeb web { get; set; }
        public HtmlNode ppage { get; set; }


        public List<string> ReadLinesFromTable(string tablename)
        {
            string ret = string.Empty;
            List<string> lines = new List<string>();
            if (!_activityFileInfo.Exists)
                throw new FileNotFoundException($"File not found {_activityFileInfo.FullName}");
            try
            {
                GetDocumentNode(_activityFileInfo);

                var nodes = ppage.SelectNodes(@"//div[@id='" + tablename + "']//table//tr");
                var trs = nodes.Where(n => n.Name == "tr");
                foreach (var d in trs)
                {
                    StringBuilder sb = new StringBuilder();
                    var children = d.ChildNodes.Where(n => n.Name != "#text");
                    foreach (var td in children)
                    {
                        sb.Append(",");
                        sb.Append(td.InnerText.Replace(",", string.Empty));
                    }

                    string line = sb.ToString();
                    lines.Add(line.Substring(1));
                    sb.Clear();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            IO.WriteStringList(lines, tablename + ".csv");
            return lines;
        }

        public void GetDocumentNode(FileInfo info)
        {
            if (web == null)
                web = new HtmlWeb();
            if (document == null)
                document = web.Load(info.FullName);
            ppage = document.DocumentNode;
        }

        public Dictionary<string, string> GetAccountInformationDictionary()
        {
            Dictionary<string, string> ai = new Dictionary<string, string>();
            //HtmlNode div = GetDiv(@"tblAccountInformation_U1523863Body");
            GetDocumentNode(_activityFileInfo);
            //tblAccountInformation_U1523863Body
            string divId = @"tblAccountInformation_U1523863Body";
            string xpath = @"//div[@id='" + divId + "']//table//tr";
            HtmlNodeCollection rows = ppage.SelectNodes(xpath);
            foreach (var row in rows)
            {
                string key = string.Empty;
                string value = string.Empty;
                var children = row.ChildNodes.Where(n => n.Name == "td");
                foreach (var td in children)
                {
                    if (td.HasAttributes)
                    {
                        key = td.InnerText;
                    }
                    else
                    {
                        value = td.InnerText;
                    }

                }
                ai.Add(key, value);

            }
            IO.SerializeStringDictionary(ai, "AccountInformation.txt", false);
            return ai;
        }

        /// <summary>
        /// Gets the trades and incidentally the AccountInfoDictionary and Contract (ContractInformationList) Number
        /// </summary>
        /// <returns>A dictionary of </returns>
        public List<DailyActivityTrade> GetTrades()
        {
            string contractType = string.Empty;
            List<DailyActivityTrade> trades = new List<DailyActivityTrade>();
            if (AccountInformationDictionary == null)
            {
                AccountInformationDictionary = GetAccountInformationDictionary();
            }
            if (ContractInformationList == null)
            {
                GetContractInformationList();
            }
            List<string> lines = ReadLinesFromTable(tradesbodytable);


            foreach (string line in lines)
            {
                string[] arr = line.Split(',');

                DailyActivityTrade trade = new DailyActivityTrade();

                if (IsTradeLine(arr[0]))
                {

                    trade.Id = 0;
                    trade.AcctID = arr[0];
                    trade.Symbol = arr[1];
                    if (trade.Symbol.Contains("ES"))
                        Debug.WriteLine("ES line");
                    trade.TradeDateTime = arr[2];
                    trade.SettleDate = PossiblyBlankCellToDateTime(arr[3]);
                    trade.Exchange = arr[4];
                    trade.TradeType = arr[5];
                    trade.Quantity = Convert.ToInt32(arr[6]);
                    trade.Price = Convert.ToDecimal(arr[7]);
                    var contractInfo = MatchTradeWithContract(trade, contractType);
                    
                    trade.ConId = contractInfo.ConId;
                    if (trade.ConId <= 0)
                        throw new InvalidDataException("Trade.ConId cannot be 0");

                    Contract contract = contractsReader.GetContractByConId(contractInfo.ConId);
                    if (contract == null)
                        throw new Exception("Could not find the contract.");
                    trade.Multiplier = contractInfo.Multiplier;
                    // On an ES the arr[8] is the notional value not the proceeds so we need to calculate it.
                    //trade.Proceeds = System.Convert.ToDecimal(arr[8]);
                    trade.Comm = Convert.ToDecimal(arr[9]);
                    if (trade.Comm < 0)
                        throw new InvalidDataException("Trade.Comm cannot be negative");
                    trade.Fee = Convert.ToDecimal(arr[10]);
                    if (trade.Fee < 0)
                        throw new InvalidDataException("Trade.Fee cannot be negative");
                    trade.Proceeds = (contractInfo.Multiplier * trade.Quantity * trade.Price * -1) + trade.Comm + trade.Fee;    // fees and commissions should be negative
                    trade.Code = arr[11];

                    if (trade.Code == "&nbsp;")
                        trade.Code = string.Empty;
                    trade.Filename = _activityFileInfo.Name;

                    if (trade.TradeType == "BUY")
                        System.Diagnostics.Debug.Assert(trade.Proceeds <= 0);
                    else
                        System.Diagnostics.Debug.Assert(trade.Proceeds >= 0);

                    trades.Add(trade);

                }
                else
                {
                    if (IsTradeTypeLine(arr[0]))
                    {
                        contractType = arr[0];
                    }
                }
            }


            return trades;
        }

        private bool IsTradeTypeLine(string arr0)
        {
            if (arr0.Contains("Total"))
                return false;


            switch (arr0)
            {
                case "Symbol":
                    return false;
                case "Acct ID":
                    return false;
                case "Stocks":
                    return true;
                case "Options On Futures":
                    return true;
                case "Futures":
                    return true;
                case "Forex":
                    return true;
                case "Equity and Index Options":
                    return true;
                case "USD":
                    return false;
                case "":
                    return false;
                default:
                    throw new KeyNotFoundException($"Non Trade line not found: {arr0}");
            }
        }
        private bool IsContractLine(string arr0)
        {
            if (arr0.Contains("Total"))
                return false;


            switch (arr0)
            {
                case "Symbol":
                    return false;
                case "Acct ID":
                    return false;
                case "Stocks":
                    return false;
                case "Options On Futures":
                    return false;
                case "Futures":
                    return false;
                case "Forex":
                    return false;
                case "Equity and Index Options":
                    return false;
                case "USD":
                    return false;
                case "":
                    return false;
                default:
                    return true;
            }
        }
        private DateTime PossiblyBlankCellToDateTime(string cellText)
        {
            //if(cellText == "&nbsp;")
            //    return DateTime.MinValue;
            //if (cellText == string.Empty)
            //    return DateTime.MinValue;
            DateTime retval;
            if (DateTime.TryParse(cellText, out retval))
            {
                return retval;
            }
            return DateTime.MinValue;


        }
        private bool IsTradeLine(string symbol)
        {
            if (symbol == AccountInformationDictionary["Account"])
                return true;
            return false;
        }
        private ContractInformation MatchTradeWithContract(DailyActivityTrade trade, string contractType)
        {
            try
            {
                ContractInformation contractInformation = new ContractInformation();
                if (contractType.Contains("Total"))
                    return contractInformation;

                switch (contractType)
                {
                    case "Acct ID":
                        return contractInformation;
                    case "Stocks":
                        contractInformation = ContractInformationList.FirstOrDefault(f => f.Symbol == trade.Symbol);
                        if (contractInformation != null) return contractInformation;
                        break;
                    case "Options On Futures":
                        contractInformation = ContractInformationList.FirstOrDefault(f => f.Description == trade.Symbol);
                        if (contractInformation != null) return contractInformation;
                        break;
                    case "Futures":
                        contractInformation = ContractInformationList.FirstOrDefault(f => f.Symbol == trade.Symbol);
                        if (contractInformation != null) return contractInformation;
                        break;
                    case "Forex":
                        contractInformation = ContractInformationList.FirstOrDefault(f => f.Symbol == trade.Symbol);
                        if (contractInformation != null) return contractInformation;
                        break;
                    case "Equity and Index Options":
                        contractInformation = ContractInformationList.FirstOrDefault(f => f.Description == trade.Symbol);
                        if (contractInformation != null) return contractInformation;
                        break;
                    case "USD":
                        return contractInformation; // no contract for this line
                    default:
                        throw new KeyNotFoundException(
                            $"Financial Instrument not found for symbol: {trade.Symbol} on {trade.TradeDateTime}");
                }
            }
            catch (KeyNotFoundException e)
            {
                throw new KeyNotFoundException(e.Message);
            }
            return new ContractInformation();

        }

        private void GetContractInformationList()
        {
            ContractInformationList = new List<ContractInformation>();

            List<string> lines = ReadLinesFromTable(contractinfotable);
            foreach (string line in lines)
            {
                string[] arr = line.Split(',');
                if (IsContractLine(arr[0]))
                {
                    ContractInformation fin = new ContractInformation();
                    fin.Symbol = arr[0];
                    fin.Description = arr[1];
                    fin.ConId = System.Convert.ToInt32(arr[2]);
                    fin.SecurityId = arr[3];
                    fin.Multiplier = System.Convert.ToInt32(arr[4]);
                    fin.Expiry = PossiblyBlankCellToDateTime(arr[5]);
                    fin.DeliveryMonth = arr[6];
                    fin.SecurityType = arr[7];
                    fin.Strike = arr[8];
                    // arr[8:10] contain "&nbsp;"
                    fin.Code = arr[11];


                    if (fin.Code == "&nbsp;")
                        fin.Code = string.Empty;
                    if (fin.SecurityId == "&nbsp;")
                        fin.SecurityId = string.Empty;
                    ContractInformationList.Add(fin);
                }
            }

        }
        public HtmlNode GetDiv(string divId)
        {
            GetDocumentNode(_activityFileInfo);
            var ppage = document.DocumentNode;
            string xpath = @"//div[@id='" + divId + "']";
            var divs = ppage.SelectNodes(xpath);
            HtmlNode div = divs.FirstOrDefault();
            return div;
        }
        public string GetHtmlFileText(FileInfo info)
        {
            try
            {
                return RawDataFileReader.ImportRaw(info.FullName);
            }
            catch (IOException fioex)
            {
                System.Diagnostics.Debug.WriteLine($"Could not read {info.FullName}");
                throw new IOException(fioex.Message);
            }
        }



    }
}
