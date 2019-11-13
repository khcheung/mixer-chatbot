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

    public class WsPollStartEvent
    {
        [JsonProperty("originatingChannel")]
        public Int64 OriginatingChannel { get; set; }

        [JsonProperty("q")]
        public String Question { get; set; }

        [JsonProperty("answers")]
        public String[] Answers { get; set; }

        [JsonProperty("author")]
        public WsPollAuthor Author { get; set; }

        [JsonProperty("duration")]
        public Int64 Duration { get; set; }

        [JsonProperty("endsAt")]
        public Int64 EndsAt { get; set; }

        [JsonProperty("voters")]
        public Int64 Voters { get; set; }

        [JsonProperty("responses")]
        public Dictionary<String, Int64> Responses { get; set; }

    }

    public class WsPollAuthor
    {
        [JsonProperty("user_name")]
        public String Username { get; set; }
        [JsonProperty("user_id")]
        public Int64 UserId { get; set; }
        [JsonProperty("user_roles")]
        public String[] UserRoles { get; set; }
    }

    public class WsPollEndEvent
    {
        [JsonProperty("originatingChannel")]
        public Int64 OriginatingChannel { get; set; }

        [JsonProperty("q")]
        public String Question { get; set; }

        [JsonProperty("answers")]
        public String[] Answers { get; set; }

        [JsonProperty("author")]
        public WsPollAuthor Author { get; set; }

        [JsonProperty("duration")]
        public Int64 Duration { get; set; }

        [JsonProperty("endsAt")]
        public Int64 EndsAt { get; set; }

        [JsonProperty("voters")]
        public Int64 Voters { get; set; }

        [JsonProperty("responses")]
        public Dictionary<String, Int64> Responses { get; set; }
    }

    public class WsDeleteMessageEvent
    {
        [JsonProperty("moderator")]
        public WsModerator Moderator { get; set; }
        [JsonProperty("id")]
        public String MessageId { get; set; }
    }

    public class WsModerator
    {
        [JsonProperty("user_name")]
        public String Username { get; set; }
        [JsonProperty("user_id")]
        public Int64 UserId { get; set; }
        [JsonProperty("user_roles")]
        public String[] UserRoles { get; set; }
        [JsonProperty("user_level")]
        public Int32 UserLevel { get; set; }
    }

    public class WsPurgeMessageEvent
    {
        [JsonProperty("moderator")]
        public WsModerator Moderator { get; set; }
        [JsonProperty("user_id")]
        public Int64 UserId { get; set; }
    }

    public class WsClearMessagesEvent
    {
        [JsonProperty("clearer")]
        public WsClearer Clearer { get; set; }
    }

    public class WsClearer
    {
        [JsonProperty("user_name")]
        public String Username { get; set; }
        [JsonProperty("user_id")]
        public Int64 UserId { get; set; }
        [JsonProperty("user_roles")]
        public String[] UserRoles { get; set; }
        [JsonProperty("user_level")]
        public Int32 UserLevel { get; set; }
    }

    public class WsUserUpdateEvent
    {
        [JsonProperty("user")]
        public Int64 UserId { get; set; }
        [JsonProperty("roles")]
        public String[] Roles { get; set; }
        [JsonProperty("permissions")]
        public String[] Permissions { get; set; }
        [JsonProperty("username")]
        public String Username { get; set; }
    }

    public class WsUserTimeoutEvent
    {
        [JsonProperty("users")]
        public WsUser User { get; set; }

        [JsonProperty("duration")]
        public Int64 Duration { get; set; }
    }

    public class WsUser
    {
        [JsonProperty("user_name")]
        public String Username { get; set; }
        [JsonProperty("user_id")]
        public Int64 UserId { get; set; }
        [JsonProperty("user_roles")]
        public String[] UserRoles { get; set; }

    }

    public class WsSkillAttributionEvent
    {
        [JsonProperty("id")]
        public String Id { get; set; }
        [JsonProperty("skill")]
        public WsSkill Skill { get; set;  }
        [JsonProperty("user_name")]
        public String Username { get; set; }
        [JsonProperty("user_id")]
        public Int64 UserId { get; set; }
        [JsonProperty("user_roles")]
        public String UserRoles { get; set; }
        [JsonProperty("user_level")]
        public Int64 UserLevel { get; set; }
        [JsonProperty("user_avatar")]
        public String UserAvatar { get; set; }

    }

    public class WsSkill
    {
        [JsonProperty("skill_id")]
        public String SkillId { get; set; }
        [JsonProperty("skill_name")]
        public String SkillName { get; set; }
        [JsonProperty("execution_id")]
        public String ExecutionId { get; set; }
        [JsonProperty("icon_url")]
        public String IconUrl { get; set; }
        [JsonProperty("cost")]
        public Int32 Cost { get; set; }
        [JsonProperty("currency")]
        public String Currency { get; set; }

        [JsonProperty("message")]
        public WsChatMessage Message { get; set; }

    }

    public class WsDeleteSkillAttributionEvent
    {
        [JsonProperty("moderator")]
        public WsModerator Moderator { get; set; }
        [JsonProperty("execution_id")]
        public String ExecutionId { get; set; }
    }

}

