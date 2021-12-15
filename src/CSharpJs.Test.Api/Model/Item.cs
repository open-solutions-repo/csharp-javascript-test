using Newtonsoft.Json;
using System;
using System.Configuration;

namespace CSharpJs.Test.Api.Model
{
    public class Item
    {
        public Item()
        {
            Usage = Convert.ToInt32(ConfigurationManager.AppSettings["UsageItem"]);
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
    }
}
