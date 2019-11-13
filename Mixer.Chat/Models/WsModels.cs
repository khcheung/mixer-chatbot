using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mixer.Chat.Models
{
    internal enum WsMessageType
    {
        method,
        reply,
        @event
    }

    internal class WsMessage
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public WsMessageType MessageType { get; set; }

        [JsonProperty("id")]
        public Int32 MessageId { get; set; }
    }


    internal class WsMethod : WsMessage
    {
        [JsonProperty("method")]
        public String Method { get; set; }

        [JsonProperty("arguments")]
        public Object[] Arguments { get; set; }
    }



    internal class WsEvent<T> : WsMessage
    {
        [JsonProperty("event")]
        [JsonConverter(typeof(StringEnumConverter))]
        public WsEventType Event { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    internal class WsReply<T> : WsMessage
    {
        [JsonProperty("error")]
        public String Error { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    internal enum WsEventType
    {
        WelcomeEvent, 
        ChatMessage,
        UserJoin,
        UserLeave,
        PollStart,
        PollEnd,
        DeleteMessage,
        PurgeMessage,
        ClearMessages,
        UserUpdate,
        UserTimeout,
        SkillAttribution,
        DeleteSkillAttribution,


        
    }



 


}
