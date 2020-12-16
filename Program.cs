using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Security;
using System.IO;
using System.Text.RegularExpressions;

namespace TwitchChatControl
{
    class Program
    {
        static string versionNumber = "2020.12.16.1";

        // Keymap - with case insensitivity
        static Dictionary<string, string> keyMap = new Dictionary<string, string>();
        static VirtualKeyboard keyboard = new VirtualKeyboard();

        static string username, userToken, twitchChannel;
        static Bot bot;

        static int maxCommandTimeSecs = 10;

        // If you plan to do this more than once, create and store a Regex instance. This will save the 
        // overhead of constructing it every time, which is more expensive than you might think.
        // https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string
        private static readonly Regex sWhitespace = new Regex(@"\s+"); 

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Twitch Chat Control {versionNumber}");
            Console.WriteLine("Specify keymap file:");
            Console.ResetColor();

            string fileMapXml = Console.ReadLine();

            keyMap = LoadKeyMap(fileMapXml);

            foreach (KeyValuePair<string, string> kvp in keyMap)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

            NameValueCollection settings = ConfigurationManager.AppSettings;

            username = settings.Get("username");
            userToken = settings.Get("token");
            twitchChannel = settings.Get("channel");

            bot = new Bot(username, userToken, twitchChannel);
            bot.OnBotMessageReceived += bot_OnBotMessageReceived;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Hit enter at any time to exit.");
            Console.ResetColor();

            Console.ReadLine();

            bot.sendMessage(twitchChannel, "Chat commands deactivated.");
        }

        /// <summary>
        /// Event handler for Twitch chat messages.
        /// </summary>
        static void bot_OnBotMessageReceived (object sender, string chatMessage)
        {
            // Decode commands into keystrokes here.
            MessageToKeystroke(chatMessage, 300);
        }

        /// <summary>
        /// Converts commands from Twitch chat into keystrokes
        /// </summary>
        /// <param name="chatMessage">A chat message string.</param>
        /// <param name="postKeyDelayMs">How long to wait between keystrokes (ms).</param>
        static void MessageToKeystroke(string chatMessage, int postKeyDelayMs)
        {
            /* Possible syntax:
             * [number of times to press][button to press][seconds to hold]
             * Examples:
             * 3x2 = "press and hold the x button 3 times, hold for 2 seconds each press"
             * 3x = "press x button 3 times"
             * x4 = "hold x button for 4 seconds"
             */

            // Format message
            chatMessage = ReplaceWhitespace(chatMessage.Trim(), "");

            // Don't waste resources decoding a chat message if it's too long.
            if (chatMessage.Length >= 10) return;

            // Begin processing message
            Console.WriteLine($"[DEBUG] Message: {chatMessage}");

            // Split the chat message
            (int repetitions, string keyStroke, int holdTimeS) = SplitString(chatMessage);

            // Abort on bad string.
            if (keyStroke == null) return;

            // Do we have a matching key in the user-defined keymap?
            if (!keyMap.ContainsKey(keyStroke)) return;

            repetitions = (repetitions > 0) ? repetitions : 1;

            // If keypresses are shorter than 75ms, some games don't pick them up.
            int holdTimeMs = (holdTimeS > 0) ? holdTimeS * 1000 : 75;

            double executionTimeSecs = repetitions * ((holdTimeMs + postKeyDelayMs) / 1000.0);

            // Prevent outrageous numbers
            if (executionTimeSecs > maxCommandTimeSecs)
            {
                bot.sendMessage(twitchChannel, $"Commands must take less than {maxCommandTimeSecs} seconds (with the required/hardcoded delays, yours would take {Math.Round(executionTimeSecs, 0)} seconds.)");
                return;
            }

            Console.WriteLine($"[DEBUG] keyMap contains {keyStroke}");

            string keyToPress = keyMap[keyStroke];

            // Send the keystroke 
            keyboard.SendRepeatKey(keyToPress, repetitions, holdTimeMs, postKeyDelayMs);
        }

        /// <summary>
        /// Splits a string from [number][word][number] into a Tuple.
        /// </summary>
        /// <remarks>
        /// Need to figure out how to handle symbols.
        /// </remarks>
        /// <param name="input">A string.</param>
        /// <returns>
        /// Tuple<int, string, int>
        /// </returns>
        static Tuple<int, string, int> SplitString(string input)
        {
            bool letterReached = false;

            string one = "";
            string two = "";
            string three = "";

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (char.IsLetter(c))
                {
                    letterReached = true;
                    two += c;
                }
                else if(char.IsNumber(c))
                {
                    // If we haven't had letters yet, it's the first number.
                    if (letterReached)
                    {
                        three += c;
                    }
                    // If we have had letters already, it's the second number.
                    else
                    {
                        one += c;
                    }
                }
                else
                {
                    // Neither letter nor number. Probably symbol so invalidate entire message.
                    return new Tuple<int, string, int>(0, null, 0);
                }
            }

            int intOne = (Int32.TryParse(one, out int valOne)) ? valOne : 0;
            int intThree = (Int32.TryParse(three, out int valThree)) ? valThree : 0;

            return new Tuple<int, string, int>(intOne, two, intThree);
        }

        /// <summary>
        /// Reads a keymap from an xml file
        /// </summary>
        /// <param name="filename">A filename string.</param>
        /// <returns>
        /// A dictionary containing the keys/values from the XML file.
        /// </returns>
        static Dictionary<string, string> LoadKeyMap(string filename)
        {
            if (filename.EndsWith(".xml")) filename = filename.Substring(0, filename.Length - 4);

            var doc = new XDocument();
            try
            {
                doc = XDocument.Load($@".\configs\{filename}.xml");
            }
            catch (ArgumentNullException)
            {
                // The input value is null.
                Console.Write($"[ERROR] No file specified.");
                Console.ReadLine();
                Environment.Exit(1);
            }
            catch (SecurityException)
            {
                // The XmlReader does not have sufficient permissions 
                // to access the location of the XML data.
                Console.Write($"[ERROR] Non-sufficient permissions to access {filename}.xml.");
                Console.ReadLine();
                Environment.Exit(1);
            }
            catch (FileNotFoundException)
            {
                // The underlying file of the path cannot be found
                Console.Write($"[ERROR] {filename}.xml not found in the configs directory.");
                Console.ReadLine();
                Environment.Exit(1);
            }
            
            var rootNodes = doc.Root.DescendantNodes().OfType<XElement>();
            var allItems = rootNodes.ToDictionary(n => n.Name.ToString().ToLower(), n => n.Value.ToString().ToLower(), StringComparer.OrdinalIgnoreCase);
            return allItems;
        }

        /// <summary>
        /// Replaces whitespace in a string
        /// </summary>
        /// <param name="input">A string.</param>
        /// <param name="replacement">A value that will replace all whitespace.</param>
        /// <returns>
        /// The string with whitespace replaced.
        /// </returns>
        static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }
    }
}
