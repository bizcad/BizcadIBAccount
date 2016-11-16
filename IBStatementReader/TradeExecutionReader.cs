using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBModels;
using IBToolBox;
using Newtonsoft.Json;

namespace IBStatementReader
{
    public class TradeExecutionReader : ITradeReader<DailyActivityTrade>
    {
        public List<Contract> Contracts;
        public List<DailyActivityTrade> GetTrades(string whereSerialized)
        {
            List<DailyActivityTrade> trades = new List<DailyActivityTrade>();

            var executionList = JsonConvert.DeserializeObject<List<Execution>>(IO.ReadFileIntoString(whereSerialized));

            foreach (Execution e in executionList)
            {
                DBContract contract = ContractConverter.Convert(GetContractByConId(Convert.ToInt32(e.orderRef)));

                var trade = DailyActivityTradeFactory.Create(e, contract);
                trades.Add(trade);
                
            }
            var serialized = CsvSerializer.Serialize(",", trades).ToList();
            IO.WriteStringList(serialized, "TradeExectionActivityList.csv");
            return trades;

        }

       
        private Contract GetContractByConId(int conId)
        {
            if (Contracts == null)
            {
                ReadContracts();
            }
            
            return Contracts.FirstOrDefault(c => c.ConId == conId);
        }
        public static List<Contract> GetContractListJson(string whereSerialized)
        {
            string Basepath = IO.Basepath;  // Get the basepath from IO
            string json;
            using (var sr = new StreamReader(Basepath + whereSerialized))
            {
                json = sr.ReadToEnd();
                sr.Close();
            }
            var list = (List<Contract>)JsonConvert.DeserializeObject(json, typeof(List<Contract>));
            return list;
        }
        public List<Contract> ReadContracts()
        {
            Contracts = GetContractListJson("Contract.json");

            return Contracts;

        }
    }
}
