using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IBModels;
using IBStatementReader;
using IBToolBox;
using ScheduleD;

namespace BizcadIBAccount
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttonStart.Enabled = true;
            buttonStart.Visible = true;
            buttonDone.Visible = false;

        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;
            //ProcessTodaysTrades();
            ProcessHtmlFiles();
            //ProcessExecutions();
            buttonStart.Visible = false;
            buttonDone.Visible = true;
            this.Refresh();
        }

        private void ProcessHtmlFiles()
        {
            List<DailyActivityTrade> trades = new List<DailyActivityTrade>();
            DirectoryInfo dir = new DirectoryInfo(@"C:\Users\bizca\OneDrive - Bizcad Systems\ProWin16\InteractiveBrokers\");
            IEnumerable<FileInfo> files = dir.GetFiles("*Trade*").OrderBy(f => f.CreationTime);
            int tradeid = 1;
            foreach (var fileinfo in files)
            {
                TradeReportReader reader = new TradeReportReader(fileinfo);
                List<DailyActivityTrade> ret = reader.GetTrades();

                trades.AddRange(ret);
                this.labelFileName.Text = fileinfo.Name;

                this.Refresh();
            }
            var tradesSorted = trades.OrderBy(d => d.TradeDateTime);
            foreach (var t in tradesSorted)
                t.Id = tradeid++;
            var tradescsv = CsvSerializer.Serialize(",", tradesSorted).ToList();
            IO.WriteStringList(tradescsv, "Trades.csv");
            //IO.SerializeJson(trades, "Trades.json");

            var otl = OrderTransactionListFactory.Create(trades);
            List<string> serialized = CsvSerializer.Serialize(",", otl).ToList();
            IO.WriteStringList(serialized, "OrderTransactionList.csv");

            // process the sorted list.
            OrderTransactionProcessor processor = new OrderTransactionProcessor();
            foreach (var item in otl)
            {
                processor.ProcessTransaction(item);
            }
            
            // Save the trades
            SaveMatchedTrades(ref processor);

            // save the inventory
            SaveInventory(ref processor, "Inventory.csv");
        }

 
        private static void SaveMatchedTrades(ref OrderTransactionProcessor processor)
        {
            var closedtrades = processor.Trades;
            var closedTradesSerialized = CsvSerializer.Serialize(",", closedtrades).ToList();
            IO.WriteStringList(closedTradesSerialized, "MatchedTrades.csv");
        }

        private static void SaveInventory(ref OrderTransactionProcessor processor, string outfilename)
        {
            List<string> serialized;
            var open = processor.OpenPositions;
            List<OrderTransaction> openlist = new List<OrderTransaction>();

            
            foreach (IPositionInventory o in open)
            {
                if (o.GetType().Name == "PositionInventoryLifo")
                {
                    PositionInventoryLifo positionInventoryLifo = (PositionInventoryLifo) o;

                    foreach (OrderTransaction b in positionInventoryLifo.Buys.ToList())
                    {
                        openlist.Add(b);
                    }
                    foreach (OrderTransaction b in positionInventoryLifo.Sells.ToList())
                    {
                        openlist.Add(b);
                    }
                }
                else
                {
                    PositionInventoryFifo positionInventoryFifo = (PositionInventoryFifo) o;

                    foreach (OrderTransaction b in positionInventoryFifo.Buys.ToList())
                    {
                        openlist.Add(b);
                    }
                    foreach (OrderTransaction b in positionInventoryFifo.Sells.ToList())
                    {
                        openlist.Add(b);
                    }
                }
            }
            serialized = CsvSerializer.Serialize(",", openlist).ToList();
            IO.WriteStringList(serialized, outfilename);
        }


        /// <summary>
        /// In today's trades all amounts are positive so we have to adjust for the
        /// direction.  
        ///     Buys are converted to negative amounts (negative cash flow)
        ///     Sells are converted to positive amounts (positive cash flow)
        /// </summary>
        private void ProcessTodaysTrades(string filepath)
        {
            List<OrderTransaction> OrderTransactionList = new List<OrderTransaction>();
            var trades = new List<TradesWindowItem>();
            TradesWindowReader reader = new TradesWindowReader();
            trades = reader.GetTrades(filepath);
            int orderid = 1;
            foreach (var trade in trades)
            {
                OrderTransaction trans = new OrderTransaction();
                trans.OrderId = orderid++;
                trans.Symbol = trade.Description;
                // If we buy, we gain inventory and pay money.
                // the quantity is positive
                trans.Quantity = trade.Side == "BOT"
                    ? Convert.ToInt32(trade.Quantity)
                    : Convert.ToInt32(trade.Quantity * -1);
                trans.Price = trade.Price;
                // The amount (money) is negative for a buy and positive for a sell
                trans.Amount = trans.Quantity * trans.Price * -1;

                // Temporary fix for multiplier until I can look up the contract in the db.
                if (trans.Symbol.Contains("ES "))
                    trans.Amount *= 50;
                
                trans.Direction = trade.Side == "BOT" ? OrderDirection.Buy : OrderDirection.Sell;
                trans.Broker = "IB";
                // Commission is always negative.  (that is the way it works in the html, but here everything is positive)
                trans.Commission = trade.Commission * -1;
                trans.Fees = 0;
                trans.TradeDate = trade.TransDateTime;
                trans.Description = trade.Description;
                trans.Exchange = trade.Exchange;
                trans.OrderType = OrderType.Limit;
                // Add commission and fees.  Add negative to negative on a buy, and negative to a positive on a sell.
                //  We receive less on a sell or pay more on a buy
                trans.Net = trans.Amount + trans.Commission + trans.Fees;
                OrderTransactionList.Add(trans);
            }
            var list = OrderTransactionList.OrderBy(n => n.TradeDate);
            IO.SerializeJson(list,"TodaysOrderTransactionList.json");



            OrderTransactionProcessor processor = new OrderTransactionProcessor();
            foreach (var item in list)
            {
                processor.ProcessTransaction(item);
            }
            SaveInventory(ref processor, "TodaysInventory.csv");

        }

        private void fromHtmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fromHtmlToolStripMenuItem.Enabled = false;
            ProcessHtmlFiles();
            fromHtmlToolStripMenuItem.Enabled = true;
        }

        private void todaysTradesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            todaysTradesToolStripMenuItem.Enabled = false;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.FileName = "TradesToday.csv";
            openFileDialog1.InitialDirectory = @"H:\IBData\";
            var dlg = openFileDialog1.ShowDialog(this);
            if (dlg == DialogResult.OK)
            {
                FileInfo info = new FileInfo(openFileDialog1.FileName);
                if (info.Exists)
                    ProcessTodaysTrades(info.Name);
            }
            
                
               
            todaysTradesToolStripMenuItem.Enabled = true;
        }

        private void requestExecutionsToolStripMenuItem_Click(object sender, EventArgs e)
        {


            buttonStart.Enabled = false;
            ProcessExecutions();
            ProcessHtmlFiles();
            buttonStart.Visible = false;
            buttonDone.Visible = true;
            this.Refresh();
        }

        private void ProcessExecutions()
        {
            TradeExecutionReader reader = new TradeExecutionReader();
            List<DailyActivityTrade> list = reader.GetTrades("ExecutionList.json");

            List<OrderTransaction> otl = OrderTransactionListFactory.Create(list);
            List<string> serialized = CsvSerializer.Serialize(",", otl).ToList();
            IO.WriteStringList(serialized, "OrderTransactionList.csv");

            OrderTransactionProcessor processor = new OrderTransactionProcessor();
            foreach (var item in otl)
            {
                Debug.WriteLine(item.ConId + "-" + item.TradeDate);
                processor.ProcessTransaction(item);
            }
            SaveInventory(ref processor, "TodaysInventory.csv");
        }
    }
}
