using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace CSharpJs.Test.Api.Model
{
    public class CreditNotes
    {
        public CreditNotes()
        {
            PaymentGroupCode = -1;
            DocDate = DateTime.Now;
            DocDueDate = DateTime.Now;
            TaxDate = DateTime.Now;
            TransportationCode = ConfigurationManager.AppSettings["TransportationCode"];
        }

        [JsonProperty(PropertyName = "TransportationCode")]
        public string TransportationCode { get; set; }

        [JsonProperty(PropertyName = "POSCashierNumber")]
        public int POSCashierNumber { get; set; }

        [JsonProperty(PropertyName = "DiscountPercent")]
        public double DiscountPercent { get; set; }

        [JsonProperty(PropertyName = "PaymentGroupCode")]
        public int PaymentGroupCode { get; set; }

        [JsonProperty(PropertyName = "CardCode")]
        public string CardCode { get; set; }

        [JsonProperty(PropertyName = "BPL_IDAssignedToInvoice")]
        public int BPL_IDAssignedToInvoice { get; set; }

        //public int SequenceCode { get; set; }
        [JsonProperty(PropertyName = "SalesPersonCode")]
        public string SalesPersonCode { get; set; }

        [JsonProperty(PropertyName = "U_OPEN_DevolutionInvoice")]
        public int U_OPEN_DevolutionInvoice { get; set; }

        [JsonProperty(PropertyName = "U_OPEN_DevolutionItem")]
        public string U_OPEN_DevolutionItem { get; set; }

        [JsonProperty(PropertyName = "DocDate")]
        public DateTime DocDate { get; set; }

        [JsonProperty(PropertyName = "DocDueDate")]
        public DateTime DocDueDate { get; set; }

        [JsonProperty(PropertyName = "TaxDate")]
        public DateTime TaxDate { get; set; }

        [JsonProperty(PropertyName = "DocumentLines")]
        public List<CreditNotesItem> DocumentLines { get; set; }

        [JsonProperty(PropertyName = "DocumentReferences")]
        public List<DocumentReferences> DocumentReferences { get; set; }
    }
}
