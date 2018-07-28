namespace KuCoinApi.NetCore.Entities
{
    public class CoinInfo
    {
        public decimal withdrawMinFee { get; set; }
        public string coinType { get; set; }
        public string txUrl { get; set; }
        public decimal withdrawMinAmount { get; set; }
        public decimal withdrawFeeRate { get; set; }
        public int confirmationCount { get; set; }
        public string name { get; set; }
        public int tradePrecision { get; set; }
        public string coin { get; set; }
        public string infoUrl { get; set; }
        public bool enableWithdraw { get; set; }
        public bool enableDeposit { get; set; }
        public string depositRemark { get; set; }
        public string withdrawRemark { get; set; }
    }
}
