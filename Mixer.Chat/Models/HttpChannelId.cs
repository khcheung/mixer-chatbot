using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mixer.Chat.Models
{
    internal class HttpChannelInfo
    {
        [JsonProperty("id")]
        public Int64 Id { get; set; }

        [JsonProperty("userId")]
        public Int64 UserId { get; set; }
    }
}
