using System;
using System.Collections.Generic;
using WindowsInput;
using WindowsInput.Native;

namespace TwitchChatControl
{
    class VirtualKeyboard
    {
        InputSimulator input = new InputSimulator();

        private Dictionary<string, VirtualKeyCode> keyboardMapping = new Dictionary<string, VirtualKeyCode> (StringComparer.OrdinalIgnoreCase)
        {
            { "0", VirtualKeyCode.VK_0 },
            { "1", VirtualKeyCode.VK_1 },
            { "2", VirtualKeyCode.VK_2 },
            { "3", VirtualKeyCode.VK_3 },
            { "4", VirtualKeyCode.VK_4 },
            { "5", VirtualKeyCode.VK_5 },
            { "6", VirtualKeyCode.VK_6 },
            { "7", VirtualKeyCode.VK_7 },
            { "8", VirtualKeyCode.VK_8 },
            { "9", VirtualKeyCode.VK_9 },
            { "A", VirtualKeyCode.VK_A },
            { "B", VirtualKeyCode.VK_B },
            { "C", VirtualKeyCode.VK_C },
            { "D", VirtualKeyCode.VK_D },
            { "E", VirtualKeyCode.VK_E },
            { "F", VirtualKeyCode.VK_F },
            { "G", VirtualKeyCode.VK_G },
            { "H", VirtualKeyCode.VK_H },
            { "I", VirtualKeyCode.VK_I },
            { "J", VirtualKeyCode.VK_J },
            { "K", VirtualKeyCode.VK_K },
            { "L", VirtualKeyCode.VK_L },
            { "M", VirtualKeyCode.VK_M },
            { "N", VirtualKeyCode.VK_N },
            { "O", VirtualKeyCode.VK_O },
            { "P", VirtualKeyCode.VK_P },
            { "Q", VirtualKeyCode.VK_Q },
            { "R", VirtualKeyCode.VK_R },
            { "S", VirtualKeyCode.VK_S },
            { "T", VirtualKeyCode.VK_T },
            { "U", VirtualKeyCode.VK_U },
            { "V", VirtualKeyCode.VK_V },
            { "W", VirtualKeyCode.VK_W },
            { "X", VirtualKeyCode.VK_X },
            { "Y", VirtualKeyCode.VK_Y },
            { "Z", VirtualKeyCode.VK_Z },
            { "+", VirtualKeyCode.ADD },
            { "BACKSPACE", VirtualKeyCode.BACK },
            { ".", VirtualKeyCode.DECIMAL },
            { "/", VirtualKeyCode.DIVIDE },
            { "ESC", VirtualKeyCode.ESCAPE },
            { "NUMLOCK", VirtualKeyCode.NUMLOCK },
            { "NUMPAD*", VirtualKeyCode.MULTIPLY },
            { "NUMPAD0", VirtualKeyCode.NUMPAD0 },
            { "NUMPAD1", VirtualKeyCode.NUMPAD1 },
            { "NUMPAD2", VirtualKeyCode.NUMPAD2 },
            { "NUMPAD3", VirtualKeyCode.NUMPAD3 },
            { "NUMPAD4", VirtualKeyCode.NUMPAD4 },
            { "NUMPAD5", VirtualKeyCode.NUMPAD5 },
            { "NUMPAD6", VirtualKeyCode.NUMPAD6 },
            { "NUMPAD7", VirtualKeyCode.NUMPAD7 },
            { "NUMPAD8", VirtualKeyCode.NUMPAD8 },
            { "NUMPAD9", VirtualKeyCode.NUMPAD9 },
            { "ENTER", VirtualKeyCode.RETURN },
            { "SPACE", VirtualKeyCode.SPACE },
            { "NUM-", VirtualKeyCode.SUBTRACT },
            { "TAB", VirtualKeyCode.TAB },
            { "DELETE", VirtualKeyCode.DELETE },
            { "END", VirtualKeyCode.END },
            { "PAGEDOWN", VirtualKeyCode.NEXT },
            { "PAGEUP", VirtualKeyCode.PRIOR },
            { "F1", VirtualKeyCode.F1 },
            { "F2", VirtualKeyCode.F2 },
            { "F3", VirtualKeyCode.F3 },
            { "F4", VirtualKeyCode.F4 },
            { "F5", VirtualKeyCode.F5 },
            { "F6", VirtualKeyCode.F6 },
            { "F7", VirtualKeyCode.F7 },
            { "F8", VirtualKeyCode.F8 },
            { "F9", VirtualKeyCode.F9 },
            { "F10", VirtualKeyCode.F10 },
            { "F11", VirtualKeyCode.F11 },
            { "F12", VirtualKeyCode.F12 },
            { "F13", VirtualKeyCode.F13 },
            { "F14", VirtualKeyCode.F14 },
            { "F15", VirtualKeyCode.F15 },
            { "F16", VirtualKeyCode.F16 },
            { "F17", VirtualKeyCode.F17 },
            { "F18", VirtualKeyCode.F18 },
            { "F19", VirtualKeyCode.F19 },
            { "F20", VirtualKeyCode.F20 },
            { "F21", VirtualKeyCode.F21 },
            { "F22", VirtualKeyCode.F22 },
            { "F23", VirtualKeyCode.F23 },
            { "F24", VirtualKeyCode.F24 },
            { "LCTRL", VirtualKeyCode.LCONTROL },
            { "LALT", VirtualKeyCode.LMENU },
            { "LSHIFT", VirtualKeyCode.LSHIFT },
            { "LWIN", VirtualKeyCode.LWIN },
            { "RCTRL", VirtualKeyCode.RCONTROL },
            { "RALT", VirtualKeyCode.RMENU },
            { "RSHIFT", VirtualKeyCode.RSHIFT },
            { "RWIN", VirtualKeyCode.RWIN },
            { "DOWN", VirtualKeyCode.DOWN },
            { "LEFT", VirtualKeyCode.LEFT },
            { "RIGHT", VirtualKeyCode.RIGHT },
            { "UP", VirtualKeyCode.UP }
        };

        /// <summary>
        /// Takes the macro syntax and translates it into keystrokes, determining repetition and sequences.
        /// </summary>
        /// <param name="key">The macro to send.</param>
        /// <param name="repetitions">How many times to repeat the keypress.</param>
        /// <param name="holdTimeMs">How long to hold each keypress.</param>
        /// <param name="postKeyDelayMs">How long to wait between keypresses.</param>
        public void ProcessKeystroke(string key, int repetitions, int holdTimeMs, int postKeyDelayMs)
        {
            // Process sequences, then combinations (modifiers)
            // If it contains a -, it's a sequence.
            var keyStrokes = key.Split('-');

            // Remove any repeats/hold time if it's a sequence
            if (keyStrokes.Length > 1)
            {
                repetitions = 1;
                holdTimeMs = 75;
            }

            foreach (var keyStroke in keyStrokes)
            {
                SendRepeatKey(keyStroke, repetitions, holdTimeMs, postKeyDelayMs);
            }
        }

        /// <summary>
        /// Loops a virtual keypress for a number of repetitions, hold times, and pauses between presses.
        /// </summary>
        /// <param name="key">Which key to press.</param>
        /// <param name="repetitions">How many times to repeat the keypress.</param>
        /// <param name="holdTimeMs">How long to hold each keypress.</param>
        /// <param name="postKeyDelayMs">How long to wait between keypresses.</param>
        public void SendRepeatKey(string key, int repetitions, int holdTimeMs, int postKeyDelayMs)
        {
            Console.WriteLine($"[DEBUG] Requested: {repetitions}x {key} for {holdTimeMs}ms");

            // If it contains +, it's a modifier.
            var keyStrokes = key.Split('+');
            var modifiedKeys = (keyStrokes.Length > 1);
            var modifiers = new List<VirtualKeyCode>();

            foreach (var keyStroke in keyStrokes)
            {
                // Do we have a matching VirtualKeyCode?
                if (!keyboardMapping.ContainsKey(keyStroke)) return;
            }

            // The primary key to press is the last element in the array.
            var vkKey = keyboardMapping[keyStrokes[keyStrokes.Length - 1]];

            if (modifiedKeys)
            {
                Array.Resize(ref keyStrokes, keyStrokes.Length - 1);

                foreach (var modifierKey in keyStrokes)
                {
                    modifiers.Add(keyboardMapping[modifierKey]);
                }
                    
            }

            for (var i = 1; i <= repetitions; i++)
            {
                Console.Write($"[DEBUG] Sending {key} for {holdTimeMs}ms.");

                if(modifiedKeys)
                {
                    SendModifiedKey(modifiers, vkKey, holdTimeMs, postKeyDelayMs);
                }
                else
                {
                    SendKey(vkKey, holdTimeMs, postKeyDelayMs);
                }

                Console.WriteLine($" ... done.");
            }
        }

        /// <summary>
        /// Presses and holds a virtual key for a specified amount of time.
        /// </summary>
        /// <param name="vkKey">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <param name="holdTimeMs">How long to hold each keypress.</param>
        /// <param name="postKeyDelayMs">How long to wait between keypresses.</param>
        private void SendKey(VirtualKeyCode vkKey, int holdTimeMs, int postKeyDelayMs)
        {
            // Add delay between keypresses- some games need keys 
            // to be pressed for a bit before they pick them up.
            input.Keyboard
                .KeyDown(vkKey)
                .Sleep(holdTimeMs)
                .KeyUp(vkKey)
                .Sleep(postKeyDelayMs); // Delay for next keypress.
        }

        /// <summary>
        /// Simulates a simple modified keystroke like CTRL-C where CTRL is the modifierKey and C is the key.
        /// The flow is Modifier KeyDown, KeyDown, Pause, KeyUp, Modifier KeyUp.
        /// </summary>
        /// <param name="modifierKeys">List of <see cref="VirtualKeyCode"/> for the modifier keys.</param>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <param name="holdTimeMs">How long to hold each keypress.</param>
        /// <param name="postKeyDelayMs">How long to wait between keypresses.</param>
        private void SendModifiedKey(List<VirtualKeyCode> modifierKeys, VirtualKeyCode keyCode, int holdTimeMs, int postKeyDelayMs)
        {
            // Press all the modifier keys
            foreach (var modKey in modifierKeys)
            {
                input.Keyboard.KeyDown(modKey);
            }

            // Press they key itself
            input.Keyboard
                .KeyDown(keyCode)
                .Sleep(holdTimeMs)
                .KeyUp(keyCode);

            // Release all the modifier keys
            foreach (var modKey in modifierKeys)
            {
                input.Keyboard.KeyUp(modKey);
            }

            input.Keyboard.Sleep(postKeyDelayMs); // Delay for next keypress.
        }
    }
}
