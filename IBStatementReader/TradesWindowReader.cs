using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBModels;
using IBToolBox;

namespace IBStatementReader
{
    public class TradesWindowReader
    {
        public List<TradesWindowItem> GetTrades(string whereSerialized)
        {
            List<TradesWindowItem> trades = new List<TradesWindowItem>();
            List<string> lines = IO.ReadTextList(whereSerialized);
            foreach (string line in lines)
            {
                
                string[] arr = line.Split(',');
                if (arr.Length > 1)
                    if (arr[1] == string.Empty)
                        continue;

                if (arr[0].Trim() == "-")
                {
                    continue;
                }
                
                // Parse the date time from the time in column 2.
                DateTime dt = DateTime.Today;
                string[] tarray = arr[1].Split(':');
                dt = dt.AddHours(Convert.ToDouble(tarray[0]));
                dt = dt.AddMinutes(Convert.ToDouble(tarray[1]));
                dt = dt.AddSeconds(Convert.ToDouble(tarray[2]));

                var trade = new TradesWindowItem
                {
                    ComboDash = arr[0],
                    TransDateTime = dt,
                    Description = arr[2],
                    Side = arr[3],
                    Quantity = System.Convert.ToDecimal(arr[4]),
                    Price = System.Convert.ToDecimal(arr[5]),
                    Exchange = arr[6],
                    Combo = arr[7].ToUpper() == "TRUE",
                    Commission = System.Convert.ToDecimal(arr[8])
                };


                trades.Add(trade);

            }
            return trades;
        }
    }
}
