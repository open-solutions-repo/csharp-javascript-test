namespace CSharpJs.Test.Api.Model
{
    public class Sale
    {
        public Invoice invoice { get; set; }
        public IncomingPayment incomingPayments { get; set; }
        public int cashierEntry { get; set; }
        public double docTotal { get; set; }
        public double accountCredit { get; set; }
    }
}
