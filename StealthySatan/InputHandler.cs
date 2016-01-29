using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthySatan
{
    class InputHandler
    {
        public enum Key { Up = 0, Down = 1, Left = 2, Right = 3, Hide = 4 }

        private static Keys[] KeyBindings = new Keys[5]
        {
            Keys.W,
            Keys.S,
            Keys.A,
            Keys.D,
            Keys.Space
        };

        private static KeyboardState CurrentKeyboard, OldKeyboard;
        private static MouseState CurrentMouse, OldMouse;

        public static void UpdateInternalState()
        {
            OldKeyboard = CurrentKeyboard;
            OldMouse = CurrentMouse;

            CurrentKeyboard = Keyboard.GetState();
            CurrentMouse = Mouse.GetState();
        }

        /// <summary>
        /// Returns if given key is currently pressed
        /// </summary>
        /// <param name="key">Key to be tested</param>
        /// <returns>Returns if given key is currently pressed</returns>
        public static bool IsPressed(Key key)
        {
            return CurrentKeyboard.IsKeyDown(KeyBindings[(int)key]);
        }

        /// <summary>
        /// Returns if given key became pressed in current frame
        /// </summary>
        /// <param name="key">Key to be tested</param>
        /// <returns>Returns if given key became pressed in current frame</returns>
        public static bool IsTyped(Key key)
        {
            return CurrentKeyboard.IsKeyDown(KeyBindings[(int)key]) && OldKeyboard.IsKeyUp(KeyBindings[(int)key]);
        }
    }
}
