using Newtonsoft.Json;

namespace CSharpJs.Test.Api.Model
{
    public class PaymentInvoice
    {
        [JsonProperty(PropertyName = "InvoiceType")]
        public string InvoiceType { get; set; }

        [JsonProperty(PropertyName = "DocEntry")]
        public int DocEntry { get; set; }

        [JsonProperty(PropertyName = "SumApplied")]
        public double SumApplied { get; set; }

        [JsonProperty(PropertyName = "AppliedSys")]
        public double AppliedSys { get; set; }

        [JsonProperty(PropertyName = "InstallmentId")]
        public int InstallmentId { get; set; }
    }
}
