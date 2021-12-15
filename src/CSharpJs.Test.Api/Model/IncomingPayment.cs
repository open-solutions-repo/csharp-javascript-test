using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CSharpJs.Test.Api.Model
{
    public class IncomingPayment
    {
        public IncomingPayment()
        {
            DocDate = DateTime.Now;
            TaxDate = DateTime.Now;
        }

        [JsonProperty(PropertyName = "BPLID")]
        public int BPLID { get; set; }

        [JsonProperty(PropertyName = "CardCode")]
        public string CardCode { get; set; }

        [JsonProperty(PropertyName = "DocDate")]
        public DateTime DocDate { get; set; }

        [JsonProperty(PropertyName = "TaxDate")]
        public DateTime TaxDate { get; set; }

        [JsonProperty(PropertyName = "CashSum")]
        public double CashSum { get; set; }

        [JsonProperty(PropertyName = "PaymentCreditCards")]
        public List<PaymentCreditCard> PaymentCreditCards { get; set; }

        [JsonProperty(PropertyName = "PaymentInvoices")]
        public List<PaymentInvoice> PaymentInvoices { get; set; }
    }
}
