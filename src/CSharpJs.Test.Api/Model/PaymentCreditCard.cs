using Newtonsoft.Json;
using System;

namespace CSharpJs.Test.Api.Model
{
    public class PaymentCreditCard
    {
        [JsonProperty(PropertyName = "CreditCard")]
        public int CreditCard { get; set; }

        [JsonProperty(PropertyName = "CardValidUntil")]
        public DateTime CardValidUntil { get; set; }

        [JsonProperty(PropertyName = "FirstPaymentDue")]
        public DateTime FirstPaymentDue { get; set; }

        [JsonProperty(PropertyName = "CreditCardNumber")]
        public string CreditCardNumber { get; set; }

        [JsonProperty(PropertyName = "VoucherNum")]
        public string VoucherNum { get; set; }

        [JsonProperty(PropertyName = "PaymentMethodCode")]
        public int PaymentMethodCode { get; set; }

        [JsonProperty(PropertyName = "NumOfPayments")]
        public int NumOfPayments { get; set; }

        [JsonProperty(PropertyName = "FirstPaymentSum")]
        public double FirstPaymentSum { get; set; }

        [JsonProperty(PropertyName = "AdditionalPaymentSum")]
        public double AdditionalPaymentSum { get; set; }

        [JsonProperty(PropertyName = "NumOfCreditPayments")]
        public int NumOfCreditPayments { get; set; }

        [JsonProperty(PropertyName = "CreditType")]
        public string CreditType { get; set; }

        [JsonProperty(PropertyName = "CreditSum")]
        public double CreditSum { get; set; }

        [JsonProperty(PropertyName = "SplitPayments")]
        public string SplitPayments { get; set; }

        [JsonProperty(PropertyName = "U_OPEN_POSID")]
        public string U_OPEN_POSID { get; set; }

        [JsonProperty(PropertyName = "U_OPEN_CreditCar")]
        public string U_OPEN_CreditCar { get; set; }

        public PaymentCreditCard()
        {
            CardValidUntil = DateTime.Now.AddMonths(12);
            CreditType = "cr_Regular";
            SplitPayments = "tYES";
        }
    }
}
