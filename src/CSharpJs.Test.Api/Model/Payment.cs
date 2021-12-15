namespace CSharpJs.Test.Api.Model
{
    public class Payment
    {
        public string POSCode { get; set; }
        public string SaleID { get; set; }
        public string TransactionID { get; set; }
        public string ServiceID { get; set; }
        public string POIID { get; set; }
        public float RequestedAmount { get; set; }
        public int TotalNbOfPayments { get; set; }
    }
}
