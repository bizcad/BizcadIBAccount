using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBModels
{
    /// <summary>
    /// The Contract Information from the html file from IB.  It works for both the Activity Report
    /// and the Daily Trade Report.
    /// </summary>
    public class ContractInformation
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public string SecurityId { get; set; }
        public int ConId { get; set; }
        public int Multiplier { get; set; }
        public DateTime Expiry { get; set; }
        public string DeliveryMonth { get; set; }
        public string SecurityType { get; set; }
        public string Strike { get; set; }
        public string Code { get; set; }
    }
}
