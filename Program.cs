using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using WindowsInput;
using WindowsInput.Native;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Linq;
using System.Security;
using System.IO;

namespace TwitchChatControl
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        // Keymap
        static Dictionary<string, string> keyMap;
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Twitch Chat Control");
            Console.WriteLine("Specify keymap file:");
            Console.ResetColor();

            string fileMapXml = Console.ReadLine();

            Program.keyMap = LoadKeyMap(fileMapXml);

            foreach (KeyValuePair<string, string> kvp in keyMap)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

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
            // Decode commands into keystrokes here.

            /* Possible syntax:
             * [number of times to press][button to press][seconds to hold]
             * Examples:
             * 3x2 = "press and hold the x button 3 times, hold for 2 seconds each press"
             * 3x = "press x button 3 times"
             * x4 = "hold x button for 4 seconds"
             */

            // Remove leading or trailing spaces.
            chatMessage.Trim();

            // Convert chat message to lowercase for matching
            chatMessage.ToLower();

            // Check message validity for further processing
            bool isValidMessage = ValidMessage(chatMessage);

            // Don't waste resources decoding a chat message if it's not valid.
            if (!isValidMessage) return;

            // Begin processing message
            Console.WriteLine($"[DEBUG] Message: {chatMessage}");

            var split = SplitString(chatMessage);

            // Prevent outrageous numbers
            int repetitions = (split.Item1 > 0 && split.Item1 <= 20) ? split.Item1 : 1;

            // If keypresses are shorter than 50ms, some games don't pick them up.
            int holdTimeMs = (split.Item3 > 0 && split.Item3 <= 10) ? split.Item3 * 1000 : 50;

            // The actual command itself.
            string keyStroke = split.Item2.ToLower();

            // Do we actually have a matching key?
            if (!Program.keyMap.ContainsKey(keyStroke)) return;

            Console.WriteLine($"[DEBUG] Requested: {repetitions}x {keyMap[keyStroke]} for {holdTimeMs}ms");

            InputSimulator input = new InputSimulator();

            var keyPress = keyMap[keyStroke];

            // Filter any special keys
            var vkKey = SpecialVKeys(keyPress);

            // If there was no match
            if(vkKey == VirtualKeyCode.VOLUME_MUTE)
            {
                vkKey = (VirtualKeyCode)VkKeyScan(keyPress.ToCharArray()[0]);
            }

            // Put it all together
            for (var i = 1; i <= repetitions; i++)
            {
                Console.Write($"[DEBUG] Sending {keyPress} for {holdTimeMs}ms.");

                input.Keyboard.KeyDown(vkKey)
                .Sleep(holdTimeMs)
                .KeyUp(vkKey)
                .Sleep(50); // Delay for next keypress.
                    
                Console.WriteLine($" ... done.");
            }
        }

        /// <summary>
        /// Checks if a message is valid for further processing/attempted decoding by the application.
        /// </summary>
        /// <remarks>
        /// We want to avoid processing every single chat message as most will likely be irrelevant to the bot.
        /// There are certain checks we can make to rule out unprocessable messages from the get-go.
        /// For example, the commands we consume won't ever contain a space, nor will they be particularly long strings.
        /// We can immediately rule out chat messages that meet either of those criteria and not waste resources on them.
        /// </remarks>
        /// <param name="message">A chat message string.</param>
        /// <returns>
        /// An isValid boolean.
        /// </returns>
        static bool ValidMessage (string message)
        {
            // Commands are not longer than 10 characters
            bool tooLong = message.Length >= 10;

            // Commands do not have spaces
            bool hasSpace = message.Contains(" ");

            return (!tooLong && !hasSpace);
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
                    if (letterReached)
                    {
                        three += c;
                    }
                    else
                    {
                        one += c;
                    }
                }
            }

            int intOne = (Int32.TryParse(one, out int valOne)) ? valOne : 0;
            int intThree = (Int32.TryParse(three, out int valThree)) ? valThree : 0;

            return new Tuple<int, string, int>(intOne, two, intThree);
        }
        /// <summary>
        /// Converts certain strings to VK codes.
        /// </summary>
        /// <remarks>
        /// Need to figure out how to handle symbols.
        /// </remarks>
        /// <param name="input">A string.</param>
        /// <returns>
        /// A VirtualKeyCode. Returns VirtualKeyCode.VOLUME_MUTE if no match is found.
        /// </returns>
        static VirtualKeyCode SpecialVKeys (string input)
        {
            switch (input.ToUpper())
            {
                case "UP":
                    return VirtualKeyCode.UP;

                case "DOWN":
                    return VirtualKeyCode.DOWN;

                case "LEFT":
                    return VirtualKeyCode.LEFT;

                case "RIGHT":
                    return VirtualKeyCode.RIGHT;

                case "F1":
                    return VirtualKeyCode.F1;

                case "F2":
                    return VirtualKeyCode.F2;

                case "F3":
                    return VirtualKeyCode.F3;

                case "F4":
                    return VirtualKeyCode.F4;

                case "F5":
                    return VirtualKeyCode.F5;

                case "F6":
                    return VirtualKeyCode.F6;

                case "F7":
                    return VirtualKeyCode.F7;

                case "F8":
                    return VirtualKeyCode.F8;

                case "F9":
                    return VirtualKeyCode.F9;

                case "F10":
                    return VirtualKeyCode.F10;

                case "F11":
                    return VirtualKeyCode.F11;

                case "F12":
                    return VirtualKeyCode.F12;

            }

            // VirtualKeyCode is not nullable, so set the value to something we'll never use as a substitute for null.
            return VirtualKeyCode.VOLUME_MUTE;
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
                doc = XDocument.Load($@".\{filename}.xml");
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
                Console.Write($"[ERROR] {filename}.xml not found in program directory.");
                Console.ReadLine();
                Environment.Exit(1);
            }
            
            var rootNodes = doc.Root.DescendantNodes().OfType<XElement>();
            var allItems = rootNodes.ToDictionary(n => n.Name.ToString().ToLower(), n => n.Value.ToString().ToLower());
            return allItems;
        }
    }
}
