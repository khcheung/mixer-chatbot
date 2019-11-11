using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mixer.Chat.Models
{
    public class WsChatMessageEvent
    {
        [JsonProperty("channel")]
        public Int64 Channel { get; set; }

        [JsonProperty("id")]
        public String Id { get; set; }
        [JsonProperty("user_name")]
        public String Username { get; set; }
        [JsonProperty("user_id")]
        public Int64 UserId { get; set; }
        [JsonProperty("user_roles")]
        public String[] UserRoles { get; set; }
        [JsonProperty("user_level")]
        public Int32 UserLevel { get; set; }
        [JsonProperty("user_avatar")]
        public String UserAvatar { get; set; }

        [JsonProperty("message")]
        public WsChatMessage Message { get; set; }
        [JsonProperty("target", NullValueHandling = NullValueHandling.Ignore)]
        public String Target { get; set; }
    }

    public class WsChatMessage
    {
        [JsonProperty("message")]
        public WsChatMessageItem[] MessageItem { get; set; }

        [JsonProperty("meta")]
        public WsChatMessageMeta Meta { get; set; }
    }

    public class WsChatMessageItem
    {
        [JsonProperty("type")]
        public ChatMessageType MessageType { get; set; }
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public String Data { get; set; }
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public String Text { get; set; }
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public String Source { get; set; }
        [JsonProperty("pack", NullValueHandling = NullValueHandling.Ignore)]
        public String Pack { get; set; }
        [JsonProperty("coords", NullValueHandling = NullValueHandling.Ignore)]
        public WsChatMessageCoordinate Coords { get; set; }
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public String Url { get; set; }

    }

    public enum ChatMessageType
    {
        text, emoticon, link, image
    }


    public class WsChatMessageCoordinate
    {
        [JsonProperty("x")]
        public Int32 X { get; set; }
        [JsonProperty("y")]
        public Int32 Y { get; set; }
        [JsonProperty("width")]
        public Int32 Width { get; set; }
        [JsonProperty("height")]
        public Int32 Height { get; set; }
    }

    public class WsChatMessageMeta
    {
        [JsonProperty("whisper", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Whisper { get; set; }

        [JsonProperty("me", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Me { get; set; }
    }


    public class WsMessageReply
    {
        [JsonProperty("channel")]
        public Int64 Channel { get; set; }
        [JsonProperty("id")]
        public String Id { get; set; }
        [JsonProperty("user_name")]
        public String Username { get; set; }
        [JsonProperty("user_id")]
        public Int64 UserId { get; set; }
        [JsonProperty("user_level")]
        public Int32 UserLevel { get; set; }
        [JsonProperty("message")]
        public WsChatMessage Message { get; set; }
    }
}
