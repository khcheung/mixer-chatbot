using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mixer.Chat.Models
{

    public class WsAuthReply
    {
        [JsonProperty("authenticated")]
        public Boolean Authenticated { get; set; }

        [JsonProperty("roles")]
        public String[] Roles { get; set; }
    }
 
}
