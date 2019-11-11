using System;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var chat = new MixerChat();
            await chat.Connect();

            while(true)
            {
                var message =  System.Console.ReadLine();
                await chat.SendMessage(message);
                
            }
        }


    }

    public class MixerChat
    {
        Mixer.Chat.Client client = null;
        public MixerChat()
        {
            client = new Mixer.Chat.Client();
            client.WelcomeEvent += Client_WelcomeEvent;
            client.UserJoinEvent += Client_UserJoinEvent;
            client.UserLeaveEvent += Client_UserLeaveEvent;
            client.ChatMessageEvent += Client_ChatMessageEvent;
            
        }

        public async Task SendMessage(String message)
        {
            await client.Msg(message, async r => { });
        }

        private void Client_UserLeaveEvent(object sender, Mixer.Chat.Models.UserLeaveEventArgs e)
        {
            Console.WriteLine($"User Leave: {e.EventData.OriginatingChannel} - {e.EventData.Username}");
        }

        private async void Client_WelcomeEvent(object sender, Mixer.Chat.Models.WelcomeEventArgs e)
        {
            Console.WriteLine($"Connected To: {e.Server}");
            var messageId = await client.Auth(async authResult =>
            {
                Console.WriteLine($"Authenticated Result: {authResult.Authenticated}");
            });
        }

        private void Client_UserJoinEvent(object sender, Mixer.Chat.Models.UserJoinEventArgs e)
        {
            Console.WriteLine($"User Join: {e.EventData.OriginatingChannel} - {e.EventData.Username}");       

            client.Msg($"Hello, {e.EventData.Username}", async r =>
            {
                
            });
        }

        private void Client_ChatMessageEvent(object sender, Mixer.Chat.Models.ChatMessageEventArgs e)
        {
            Console.WriteLine($"Message: {e.EventData.Username} - {e.EventData.Message.MessageItem[0].Text}");
        }


        public async Task Connect()
        {
            await client.ConnectWithToken("[TOKEN]", "[CHANNEL NAME]");
        }
    }
}
