using System;
using System.Collections.Generic;
using System.Text;

namespace Mixer.Chat.Models
{
    public abstract class EventArgsBase<T> : EventArgs
    {
        public T EventData { get; set; }
    }

    public class WelcomeEventArgs : EventArgs
    {
        public String Server { get; set; }
    }

    public class UserJoinEventArgs : EventArgsBase<WsUserJoinEvent> { }

    public class UserLeaveEventArgs : EventArgsBase<WsUserLeaveEvent> { }
    public class ChatMessageEventArgs : EventArgsBase<WsChatMessageEvent> { }

    public class PollStartEventArgs : EventArgsBase<WsPollStartEvent> { }
    public class PollEndEventArgs : EventArgsBase<WsPollEndEvent> { }
    public class DeleteMessageEventArgs : EventArgsBase<WsDeleteMessageEvent> { }
    public class PurgeMessageEventArgs : EventArgsBase<WsPurgeMessageEvent> { }
    public class ClearMessagesEventArgs : EventArgsBase<WsClearMessagesEvent> { }
    public class UserUpdateEventArgs : EventArgsBase<WsUserUpdateEvent> { }
    public class UserTimeoutEventArgs : EventArgsBase<WsUserTimeoutEvent> { }
    public class SkillAttributionEventArgs : EventArgsBase<WsSkillAttributionEvent> { }
    public class DeleteSkillAttributionEventArgs : EventArgsBase<WsDeleteSkillAttributionEvent> { }


}
