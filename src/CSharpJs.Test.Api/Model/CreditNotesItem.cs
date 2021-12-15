using Newtonsoft.Json;
using System;
using System.Configuration;

namespace CSharpJs.Test.Api.Model
{
    public class CreditNotesItem
    {
        public CreditNotesItem()
        {
            Usage = Convert.ToInt32(ConfigurationManager.AppSettings["UsageItemDev"]);
        }

        [JsonProperty(PropertyName = "VisOrder")]
        public int VisOrder { get; set; }

        [JsonProperty(PropertyName = "ItemCode")]
        public string ItemCode { get; set; }

        [JsonProperty(PropertyName = "Quantity")]
        public double Quantity { get; set; }

        [JsonProperty(PropertyName = "UnitPrice")]
        public double UnitPrice { get; set; }

        [JsonProperty(PropertyName = "Usage")]
        public int Usage { get; set; }

        [JsonProperty(PropertyName = "U_OPEN_InvoicesLine")]
        public int U_OPEN_InvoicesLine { get; set; }

        [JsonProperty(PropertyName = "U_OPEN_InvoicesEntry")]
        public int U_OPEN_InvoicesEntry { get; set; }
    }
}
