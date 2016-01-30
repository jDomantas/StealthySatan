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
        private static GamePadState CurrentGamePad, OldGamePad;

        public static void UpdateInternalState()
        {
            OldKeyboard = CurrentKeyboard;
            OldMouse = CurrentMouse;
            OldGamePad = CurrentGamePad;
            
            CurrentKeyboard = Keyboard.GetState();
            CurrentMouse = Mouse.GetState();
            CurrentGamePad = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            if (!CurrentGamePad.IsConnected) CurrentGamePad = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.Two);
            if (!CurrentGamePad.IsConnected) CurrentGamePad = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.Three);
            if (!CurrentGamePad.IsConnected) CurrentGamePad = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.Four);
        }

        /// <summary>
        /// Returns if given key is currently pressed
        /// </summary>
        /// <param name="key">Key to be tested</param>
        /// <returns>Returns if given key is currently pressed</returns>
        public static bool IsPressed(Key key)
        {
            switch (key)
            {
                case Key.Up: if (CurrentGamePad.DPad.Up == ButtonState.Pressed) return true; break;
                case Key.Down: if (CurrentGamePad.DPad.Down == ButtonState.Pressed) return true; break;
                case Key.Left: if (CurrentGamePad.DPad.Left == ButtonState.Pressed) return true; break;
                case Key.Right: if (CurrentGamePad.DPad.Right == ButtonState.Pressed) return true; break;
                case Key.Hide: if (CurrentGamePad.Buttons.A == ButtonState.Pressed) return true; break;
                case Key.Enter: if (CurrentGamePad.Buttons.B == ButtonState.Pressed) return true; break;
            }

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
            switch (key)
            {
                case Key.Up: if (OldGamePad.DPad.Up == ButtonState.Released && CurrentGamePad.DPad.Up == ButtonState.Pressed) return true; break;
                case Key.Down: if (OldGamePad.DPad.Down == ButtonState.Released && CurrentGamePad.DPad.Down == ButtonState.Pressed) return true; break;
                case Key.Left: if (OldGamePad.DPad.Left == ButtonState.Released && CurrentGamePad.DPad.Left == ButtonState.Pressed) return true; break;
                case Key.Right: if (OldGamePad.DPad.Right == ButtonState.Released && CurrentGamePad.DPad.Right == ButtonState.Pressed) return true; break;
                case Key.Hide: if (OldGamePad.Buttons.A == ButtonState.Released && CurrentGamePad.Buttons.A == ButtonState.Pressed) return true; break;
                case Key.Enter: if (OldGamePad.Buttons.B == ButtonState.Released && CurrentGamePad.Buttons.B == ButtonState.Pressed) return true; break;
            }

            for (int i = 0; i < KeyBindings[(int)key].Length; i++)
                if (CurrentKeyboard.IsKeyDown(KeyBindings[(int)key][i]) &&
                    OldKeyboard.IsKeyUp(KeyBindings[(int)key][i]))
                    return true;
            return false;
        }
    }
}
