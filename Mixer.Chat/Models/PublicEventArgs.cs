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

    
}
