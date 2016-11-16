using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBModels
{
    public class TradesWindowItem
    {
        public string ComboDash { get; set; }
        public DateTime TransDateTime { get; set; }
        public string Description { get; set; }
        public string Side { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Exchange { get; set; }
        public bool Combo { get; set; }
        public decimal Commission { get; set; }

    }
}
