using System;
using System.Configuration;
using System.Collections.Specialized;

namespace TwitchChatControl
{
    class Program
    {
        static void Main(string[] args)
        {
            NameValueCollection settings = ConfigurationManager.AppSettings;

            string username = settings.Get("username");
            string userToken = settings.Get("token");
            string twitchChannel = settings.Get("channel");

            var bot = new Bot(username, userToken, twitchChannel);
            bot.OnBotMessageReceived += bot_OnBotMessageReceived;
            Console.ReadLine();
        }

        static void bot_OnBotMessageReceived (object sender, string chatMessage)
        {

        }
    }
}
