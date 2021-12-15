using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace CSharpJs.Test.Api.Model
{
    public class RequestedCreditNotes
    {
        [JsonProperty(PropertyName = "odata.metadata")]
        public string metadata { get; set; }


        [JsonProperty(PropertyName = "odata.nextLink")]
        public string NextLink { get; set; }


        [JsonProperty(PropertyName = "value")]
        public List<Invoice> Values { get; set; }

        public class Invoice
        {
            public double TotalValue { get; set; }

            [JsonProperty(PropertyName = "DocEntry")]
            public int DocEntry { get; set; }

            [JsonProperty(PropertyName = "DocNum")]
            public int DocNum { get; set; }

            [JsonProperty(PropertyName = "TransportationCode")]
            public int? TransportationCode { get; set; }

            [JsonProperty(PropertyName = "POSCashierNumber")]
            public int? POSCashierNumber { get; set; }

            [JsonProperty(PropertyName = "DiscountPercent")]
            public double? DiscountPercent { get; set; }

            [JsonProperty(PropertyName = "PaymentGroupCode")]
            public int? PaymentGroupCode { get; set; }

            [JsonProperty(PropertyName = "CardCode")]
            public string CardCode { get; set; }

            [JsonProperty(PropertyName = "BPL_IDAssignedToInvoice")]
            public int? BPL_IDAssignedToInvoice { get; set; }

            [JsonProperty(PropertyName = "SalesPersonCode")]
            public string SalesPersonCode { get; set; }

            [JsonProperty(PropertyName = "DocDate")]
            public DateTime DocDate { get; set; }

            [JsonProperty(PropertyName = "DocDueDate")]
            public DateTime DocDueDate { get; set; }

            [JsonProperty(PropertyName = "TaxDate")]
            public DateTime TaxDate { get; set; }

            [JsonProperty(PropertyName = "DocumentLines")]
            public List<Item> DocumentLines { get; set; }

            [JsonProperty(PropertyName = "DocumentReferences")]
            public List<DocumentReferences> DocumentReferences { get; set; }
        }

        public class Item
        {
            [JsonProperty(PropertyName = "VisualOrder")]
            public int VisualOrder { get; set; }

            [JsonProperty(PropertyName = "ItemCode")]
            public string ItemCode { get; set; }

            [JsonProperty(PropertyName = "LineNum")]
            public string LineNum { get; set; }

            [JsonProperty(PropertyName = "Quantity")]
            public double? Quantity { get; set; }

            [JsonProperty(PropertyName = "UnitPrice")]
            public double? UnitPrice { get; set; }

            [JsonProperty(PropertyName = "U_OPEN_Returned")]
            public string U_OPEN_Returned { get; set; }

            [JsonProperty(PropertyName = "Usage")]
            public int? Usage { get; set; }
        }
    }
}
