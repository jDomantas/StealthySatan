using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthySatan
{
    class InputHandler
    {
        public enum Key { Up = 0, Down = 1, Left = 2, Right = 3, Hide = 4, Enter = 5 }

        private static Keys[][] KeyBindings = new Keys[][]
        {
            new Keys[] { Keys.W, Keys.Up, Keys.NumPad8 },
            new Keys[] { Keys.S, Keys.Down, Keys.NumPad2 },
            new Keys[] { Keys.A, Keys.Left, Keys.NumPad4 },
            new Keys[] { Keys.D, Keys.Right, Keys.NumPad6 },
            new Keys[] { Keys.F, Keys.Space },
            new Keys[] { Keys.E, Keys.Enter }
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
            for (int i = 0; i < KeyBindings[(int)key].Length; i++)
                if (CurrentKeyboard.IsKeyDown(KeyBindings[(int)key][i]))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns if given key became pressed in current frame
        /// </summary>
        /// <param name="key">Key to be tested</param>
        /// <returns>Returns if given key became pressed in current frame</returns>
        public static bool IsTyped(Key key)
        {
            for (int i = 0; i < KeyBindings[(int)key].Length; i++)
                if (CurrentKeyboard.IsKeyDown(KeyBindings[(int)key][i]) &&
                    OldKeyboard.IsKeyUp(KeyBindings[(int)key][i]))
                    return true;
            return false;
        }
    }
}
