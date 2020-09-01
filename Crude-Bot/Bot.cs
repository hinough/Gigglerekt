using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Crude_Bot
{
    class Bot
    {
        private TwitchClient client;

        public EventHandler<string> onConnected = null;
        public EventHandler<string> onJoined = null;

        public Bot(string username, string token, string channel)
        {
            ConnectionCredentials credentials = new ConnectionCredentials(username, token);
            
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, channel);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            onConnected?.Invoke(this, "Connected");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            //Not used
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            //Not used
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            //Check for command
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            onJoined?.Invoke(this, "Joined channel");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.Out.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }
    }
}
