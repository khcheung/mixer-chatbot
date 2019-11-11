using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mixer.Chat.Security
{
    public class ChannelInfo
    {
        [JsonProperty("roles")]
        public String[] Roles { get; set; }
        
        [JsonProperty("authkey")]
        public String AuthKey { get; set; }

        [JsonProperty("permissions")]
        public String[] Permissions { get; set; }

        [JsonProperty("endpoints")]
        public String[] EndPoints { get; set; }

        [JsonProperty("isLoadShed")]
        public Boolean IsLoadShed { get; set; }

    }
}
