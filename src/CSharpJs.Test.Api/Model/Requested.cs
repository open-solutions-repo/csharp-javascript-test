using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace CSharpJs.Test.Api.Model
{
    public class Requested
    {
        [JsonProperty(PropertyName = "odata.metadata")]
        public string metadata { get; set; }


        [JsonProperty(PropertyName = "odata.nextLink")]
        public string NextLink { get; set; }


        [JsonProperty(PropertyName = "value")]
        public List<dynamic> Values { get; set; }
    }
}
