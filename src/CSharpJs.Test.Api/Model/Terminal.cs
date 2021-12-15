using Newtonsoft.Json;

namespace CSharpJs.Test.Api.Model
{
    public class Terminal
    {
        [JsonProperty("U_OPEN_TerminalCode")]
        public string U_OPEN_TerminalCode { get; set; }

        [JsonProperty("U_OPEN_BPLId")]
        public string U_OPEN_BPLId { get; set; }

        [JsonProperty("DocEntry")]
        public string DocEntry { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }
    }
}
