using Mixer.Chat.Models;
using Mixer.Chat.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Mixer.Chat
{
    public class Client
    {
        private CancellationToken mCancellationToken;
        private Uri mChatUri = null;
        private String mAuthKey = null;
        private event EventHandler<MessageEventArgs> SocketEvent;

        public event EventHandler<WelcomeEventArgs> WelcomeEvent;
        public event EventHandler<UserJoinEventArgs> UserJoinEvent;
        public event EventHandler<UserLeaveEventArgs> UserLeaveEvent;
        public event EventHandler<ChatMessageEventArgs> ChatMessageEvent;
        public event EventHandler<PollStartEventArgs> PollStartEvent;
        public event EventHandler<PollEndEventArgs> PollEndEvent;
        public event EventHandler<DeleteMessageEventArgs> DeleteMessageEvent;
        public event EventHandler<PurgeMessageEventArgs> PurgeMessageEvent;
        public event EventHandler<ClearMessagesEventArgs> ClearMessagesEvent;
        public event EventHandler<UserUpdateEventArgs> UserUpdateEvent;
        public event EventHandler<UserTimeoutEventArgs> UserTimeoutEvent;
        public event EventHandler<SkillAttributionEventArgs> SkillAttributionEvent;
        public event EventHandler<DeleteSkillAttributionEventArgs> DeleteSkillAttributionEvent;


        private ClientWebSocket mClient = null;
        private Int32 mMessageId = 0;

        private Int64 mChannelId = 0;
        private Int64 mUserId = 0;

        private Dictionary<Int32, Action<String>> mReplyHandler = null;

        public Client()
        {
            mCancellationToken = new CancellationToken();
            mReplyHandler = new Dictionary<int, Action<string>>();
            SocketEvent += Client_SocketEvent;
        }

        public async Task ConnectWithToken(String accessToken, String channelName)
        {
            var channelInfo = await GetChannelInfo(channelName);
            await ConnectWithToken(accessToken, channelInfo.Id);
        }

        private async Task<HttpChannelInfo> GetChannelInfo(String channelName)
        {
            var url = $"https://mixer.com/api/v1/channels/{channelName}?fields=id,userId";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var channel = JsonConvert.DeserializeObject<HttpChannelInfo>(responseBody);
                    this.mUserId = channel.UserId;
                    this.mChannelId = channel.Id;
                    return channel;
                }
                else
                {
                    var error = JsonConvert.DeserializeObject<HttpError>(responseBody);
                    throw new Exception($"{error.Error} - {error.ErrorDesciption}");
                }
            }
        }

        public async Task ConnectWithToken(String accessToken, Int64 channelId)
        {
            var channelInfo = await GetChannelInfo(accessToken, channelId);
            if (String.IsNullOrEmpty(channelInfo.AuthKey))
            {
                throw new Exception("Cannot Retrieve AuthKey From Token");
            }
            await Connect(channelInfo.AuthKey, channelId, mUserId, channelInfo.EndPoints[0]);
        }

        private async Task<ChannelInfo> GetChannelInfo(String accessToken, Int64 channelId)
        {
            var url = $"https://mixer.com/api/v1/chats/{channelId}";
            using (var httpClient = new HttpClient())
            {
                var header = httpClient.DefaultRequestHeaders;
                header.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var channelInfo = JsonConvert.DeserializeObject<ChannelInfo>(responseBody);
                    return channelInfo;
                }
                else
                {
                    var error = JsonConvert.DeserializeObject<HttpError>(responseBody);
                    throw new Exception($"{error.Error} - {error.ErrorDesciption}");
                }
            }
        }

        public async Task Connect(String authKey, Int64 channelId, Int64 userId, String endPoint)
        {
            this.mAuthKey = authKey;
            this.mChatUri = new Uri(endPoint);
            this.mChannelId = channelId;
            this.mUserId = userId;
            await this.Connect();
        }

        private async Task Connect()
        {
            mClient = new ClientWebSocket();
            await mClient.ConnectAsync(mChatUri, mCancellationToken);
            ReceiveThread(mClient);
        }

        private void Client_SocketEvent(object sender, MessageEventArgs e)
        {
            switch (e.Message.MessageType)
            {
                case WsMessageType.@event:
                    try
                    {
                        HandleSocketEvent(e);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Handle Event Exception - {ex.Message}");
                    }
                    break;
                case WsMessageType.reply:
                    try
                    {
                        HandleSocketReply(e);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Handle Reply Exception  - {ex.Message}");
                    }
                    break;
            }
        }

        private void HandleSocketEvent(MessageEventArgs e)
        {
            var eventMessage = JsonConvert.DeserializeObject<WsEvent<Object>>(e.MessageString);
            switch (eventMessage.Event)
            {
                case WsEventType.WelcomeEvent:
                    var welcomeEvent = JsonConvert.DeserializeObject<WsEvent<WsWelcomeEvent>>(e.MessageString);
                    this.WelcomeEvent?.Invoke(this, new WelcomeEventArgs() { Server = welcomeEvent.Data.Server });
                    break;
                case WsEventType.UserJoin:
                    var userJoinEvent = JsonConvert.DeserializeObject<WsEvent<WsUserJoinEvent>>(e.MessageString);
                    this.UserJoinEvent?.Invoke(this, new UserJoinEventArgs()
                    {
                        EventData = userJoinEvent.Data
                    });
                    break;
                case WsEventType.UserLeave:
                    var userLeaveEvent = JsonConvert.DeserializeObject<WsEvent<WsUserLeaveEvent>>(e.MessageString);
                    this.UserLeaveEvent?.Invoke(this, new UserLeaveEventArgs()
                    {
                        EventData = userLeaveEvent.Data
                    });
                    break;
                case WsEventType.ChatMessage:
                    var chatMessageEvent = JsonConvert.DeserializeObject<WsEvent<WsChatMessageEvent>>(e.MessageString);
                    this.ChatMessageEvent?.Invoke(this, new ChatMessageEventArgs()
                    {
                        EventData = chatMessageEvent.Data
                    });
                    break;

                case WsEventType.PollStart:
                    var pollStartEvent = JsonConvert.DeserializeObject<WsEvent<WsPollStartEvent>>(e.MessageString);
                    this.PollStartEvent?.Invoke(this, new PollStartEventArgs()
                    {
                        EventData = pollStartEvent.Data
                    });
                    break;

                case WsEventType.PollEnd:
                    var pollEndEvent = JsonConvert.DeserializeObject<WsEvent<WsPollEndEvent>>(e.MessageString);
                    this.PollEndEvent?.Invoke(this, new PollEndEventArgs()
                    {
                        EventData = pollEndEvent.Data
                    });
                    break;

                case WsEventType.DeleteMessage:
                    var deleteMessageEvent = JsonConvert.DeserializeObject<WsEvent<WsDeleteMessageEvent>>(e.MessageString);
                    this.DeleteMessageEvent?.Invoke(this, new DeleteMessageEventArgs()
                    {
                        EventData = deleteMessageEvent.Data
                    });
                    break;

                case WsEventType.PurgeMessage:
                    var purgeMessageEvent = JsonConvert.DeserializeObject<WsEvent<WsPurgeMessageEvent>>(e.MessageString);
                    this.PurgeMessageEvent?.Invoke(this, new PurgeMessageEventArgs()
                    {
                        EventData = purgeMessageEvent.Data
                    });
                    break;

                case WsEventType.ClearMessages:
                    var clearMessagesEvent = JsonConvert.DeserializeObject<WsEvent<WsClearMessagesEvent>>(e.MessageString);
                    this.ClearMessagesEvent?.Invoke(this, new ClearMessagesEventArgs()
                    {
                        EventData = clearMessagesEvent.Data
                    });
                    break;

                case WsEventType.UserUpdate:
                    var userUpdateEvent = JsonConvert.DeserializeObject<WsEvent<WsUserUpdateEvent>>(e.MessageString);
                    this.UserUpdateEvent?.Invoke(this, new UserUpdateEventArgs()
                    {
                        EventData = userUpdateEvent.Data
                    });
                    break;

                case WsEventType.UserTimeout:
                    var userTimeoutEvent = JsonConvert.DeserializeObject<WsEvent<WsUserTimeoutEvent>>(e.MessageString);
                    this.UserTimeoutEvent?.Invoke(this, new UserTimeoutEventArgs()
                    {
                        EventData = userTimeoutEvent.Data
                    });
                    break;

                case WsEventType.SkillAttribution:
                    var skillAttributionEvent = JsonConvert.DeserializeObject<WsEvent<WsSkillAttributionEvent>>(e.MessageString);
                    this.SkillAttributionEvent?.Invoke(this, new SkillAttributionEventArgs()
                    {
                        EventData = skillAttributionEvent.Data
                    });
                    break;

                case WsEventType.DeleteSkillAttribution:
                    var deleteSkillAttributionEvent = JsonConvert.DeserializeObject<WsEvent<WsDeleteSkillAttributionEvent>>(e.MessageString);
                    this.DeleteSkillAttributionEvent?.Invoke(this, new DeleteSkillAttributionEventArgs()
                    {
                        EventData = deleteSkillAttributionEvent.Data
                    });
                    break;


            }
        }

        private void HandleSocketReply(MessageEventArgs e)
        {
            if (mReplyHandler.ContainsKey(e.MessageId))
            {
                mReplyHandler[e.MessageId](e.MessageString);
                mReplyHandler.Remove(e.MessageId);
            }
        }

        private void ReceiveThread(ClientWebSocket client)
        {
            var byteBuffer = new Byte[65535];
            var buffer = new ArraySegment<Byte>(byteBuffer);
            var messageBuffer = new List<Byte>();
            WebSocketMessageType messageType = WebSocketMessageType.Text;
            _ = Task.Run(async () =>
            {
                while (!mCancellationToken.IsCancellationRequested)
                {
                    var result = await client.ReceiveAsync(buffer, mCancellationToken);
                    messageType = result.MessageType;
                    if (messageType == WebSocketMessageType.Close)
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", mCancellationToken);
                        return;
                    }

                    messageBuffer.AddRange(buffer.Array.Skip(buffer.Offset).Take(result.Count).ToArray());

                    if (result.EndOfMessage)
                    {
                        if (messageType == WebSocketMessageType.Text)
                        {
                            var messageContent = System.Text.Encoding.UTF8.GetString(messageBuffer.ToArray());

                            var message = JsonConvert.DeserializeObject<WsMessage>(messageContent);
                            if (SocketEvent != null)
                            {
                                SocketEvent(this, new MessageEventArgs()
                                {
                                    Message = message,
                                    MessageString = messageContent,
                                    MessageId = message.MessageId
                                });
                            }
                        }

                        messageBuffer.Clear();
                    }
                }
            });
        }


        #region Method
        public async Task<Int32> Auth(Func<WsAuthReply, Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "auth",
                Arguments = new object[] { this.mChannelId, this.mUserId, this.mAuthKey }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<WsAuthReply>>(message);
                await callBack(result.Data);
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;
        }

        public async Task<Int32> Msg(String chatMessage, Func<WsMessageReply, Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "msg",
                Arguments = new object[] { chatMessage }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<WsMessageReply>>(message);
                await callBack(result.Data);
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;
        }

        public async Task<Int32> Whipser(String targetUsername, String chatMessage, Func<WsWhisperReply, Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "whisper",
                Arguments = new object[] { targetUsername, chatMessage }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<WsWhisperReply>>(message);
                await callBack(result.Data);
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        public async Task<Int32> VoteChoose(Int32 voteIndex, Func<String, Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "vote:choose",
                Arguments = new object[] { voteIndex }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<String>>(message);
                await callBack(result.Data);
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        public async Task<Int32> VoteStart(String question, String[] answer, Int32 durationSeconds, Func<String, Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "vote:start",
                Arguments = new object[] { question, answer, durationSeconds }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<String>>(message);
                await callBack(result.Data);
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        public async Task<Int32> Timeout(String username, Int32 durationSeconds, Func<String, Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "timeout",
                Arguments = new object[] { username, durationSeconds }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<String>>(message);
                await callBack(result.Data);
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        public async Task<Int32> Purge(String username, Func<Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "purge",
                Arguments = new object[] { username }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<Object>>(message);
                await callBack();
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        public async Task<Int32> DeleteMessage(String id, Func<String, Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "deleteMessage",
                Arguments = new object[] { id }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<String>>(message);
                await callBack(result.Data);
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        public async Task<Int32> ClearMessage(Func<String, Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "clearMessages",
                Arguments = new object[] { }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<String>>(message);
                await callBack(result.Data);
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        public async Task<Int32> History(Int32 messageCount, Func<WsHistoryReply[], Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "history",
                Arguments = new object[] { messageCount }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<WsHistoryReply[]>>(message);
                await callBack(result.Data);
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;
        }

        public async Task<Int32> GiveawayStart(Func<String, Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "giveaway:start",
                Arguments = new object[] { }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<String>>(message);
                await callBack(result.Data);
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        public async Task<Int32> Ping(Func<Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "ping",
                Arguments = new object[] { }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<Object>>(message);
                await callBack();
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        public async Task<Int32> Attachemotes(Func<Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "attachEmotes",
                Arguments = new object[] { }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<Object>>(message);
                await callBack();
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        public async Task<Int32> CancelSkill(String skillId, Func<String, Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "cancelSkill",
                Arguments = new object[] { skillId }
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<String>>(message);
                await callBack(result.Data);
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        public async Task<Int32> OptOutEvents(String[] events, Func<Task> callBack)
        {
            var messageId = mMessageId++;
            WsMethod method = new WsMethod()
            {
                MessageId = messageId,
                MessageType = WsMessageType.method,
                Method = "optOutEvents",
                Arguments = events
            };
            var jsonString = JsonConvert.SerializeObject(method);
            var jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonString);
            mReplyHandler.Add(messageId, async message =>
            {
                var result = JsonConvert.DeserializeObject<WsReply<Object>>(message);
                await callBack();
            });
            await mClient.SendAsync(new ArraySegment<byte>(jsonByte), WebSocketMessageType.Text, true, mCancellationToken);


            return messageId;

        }

        #endregion

        
    }
}
