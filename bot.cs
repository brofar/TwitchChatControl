using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TwitchChatControl
{
    /// <summary>
    /// The main Bot class.
    /// Contains all methods and events for connecting to Twitch and handling commands.
    /// </summary>
    class Bot
    {
        public event EventHandler<string> OnBotMessageReceived; // event

        TwitchClient client;

        /// <summary>
        /// A Twitch bot client which automatically joins a channel upon instantiation.
        /// </summary>
        /// <param name="botName">A Twitch username.</param>
        /// <param name="oauthToken">The OAuth token for the username, without "oauth:".</param>
        /// <param name="twitchChannel">The channel for the bot to join.</param>
        public Bot(string botName, string oauthToken, string twitchChannel)
        {
            ConnectionCredentials credentials = new ConnectionCredentials(botName, oauthToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, twitchChannel);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to Twitch.");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"Joined #{e.Channel}. Ready to go!");
            client.SendMessage(e.Channel, "Chat commands activated.");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            // Fire event (call delegate)
            OnBotMessageReceived?.Invoke(this, e.ChatMessage.Message);
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {

        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {

        }
        public void sendMessage(string channel, string message)
        {
            client.SendMessage(channel, message);
        }
    }
}
