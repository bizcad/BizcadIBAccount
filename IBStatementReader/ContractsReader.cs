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
    public class ContractsReader
    {
        public List<Contract> Contracts { get; set; }
        public string WhereSerialized = "Contract.json";
        public List<Contract> ReadContracts()
        {
            Contracts = GetContractListJson(WhereSerialized);

            return Contracts;

        }

        public List<Contract> GetContractListJson(string whereSerialized)
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

        public Contract GetContractByConId(int conId)
        {
            if (Contracts == null)
            {
                ReadContracts();
            }

            return Contracts.FirstOrDefault(c => c.ConId == conId);
        }
    }
}
