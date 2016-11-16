namespace IBModels
{
    public class Execution
    {
        public int orderId{ get; set; }
        public int clientId{ get; set; }
        public string execId{ get; set; }
        public string time{ get; set; }
        public string acctNumber{ get; set; }
        public string exchange{ get; set; }
        public string side{ get; set; }
        public int shares{ get; set; }
        public double price{ get; set; }
        public int permId{ get; set; }
        public int liquidation{ get; set; }
        public int cumQty{ get; set; }
        public double avgPrice{ get; set; }
        public string orderRef{ get; set; }
        public string evRule{ get; set; }
        public double evMultiplier{ get; set; }
    }
}
