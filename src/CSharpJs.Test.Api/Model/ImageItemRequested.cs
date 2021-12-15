using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace CSharpJs.Test.Api.Model
{
    public class ImageItemRequested
    {
        [JsonProperty(PropertyName = "odata.metadata")]
        public string Metadata { get; set; }

        [JsonProperty(PropertyName = "odata.mediaReadLink")]
        public string MediaReadLink { get; set; }

        [JsonProperty(PropertyName = "odata.mediaContentType")]
        public string MediaContentType { get; set; }

        [JsonProperty(PropertyName = "ItemCode")]
        public string ItemCode { get; set; }

        [JsonProperty(PropertyName = "Picture")]
        public string Picture { get; set; }
    }
}
