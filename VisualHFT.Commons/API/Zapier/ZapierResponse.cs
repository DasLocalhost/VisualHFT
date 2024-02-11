using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.API;

namespace VisualHFT.Commons.API.Zapier
{
    public class ZapierBaseResponse : IBaseResponse
    {
        [JsonProperty("attempt")]
        public string? Attempt { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("request_id")]
        public string? RequestId { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }
    }
}
