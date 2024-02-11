using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualHFT.Commons.API.Twitter
{
    public class TwitterMessage
    {
        [JsonProperty("text")]
        public string? Text { get; set; }
    }
}
