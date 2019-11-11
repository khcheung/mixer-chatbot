using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mixer.Chat.Models
{
    public class WsWelcomeEvent
    {
        [JsonProperty("server")]
        public String Server { get; set; }
    }

    public class WsUserJoinEvent
    {
        [JsonProperty("originatingChannel")]
        public Int64 OriginatingChannel { get; set; }

        [JsonProperty("username")]
        public String Username { get; set; }

        [JsonProperty("roles")]
        public String[] Roles { get; set; }

        [JsonProperty("id")]
        public Int64 UserId { get; set; }
    }

    public class WsUserLeaveEvent
    {
        [JsonProperty("originatingChannel")]
        public Int64 OriginatingChannel { get; set; }

        [JsonProperty("username")]
        public String Username { get; set; }

        [JsonProperty("id")]
        public Int64 UserId { get; set; }
    }
}
