using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StealthySatan
{
    /// <summary>
    /// Stores all game's resources
    /// </summary>
    static class Resources
    {
        public static void Load(ContentManager content)
        {
            Graphics.Player = content.Load<Texture2D>("Graphics/player");
        }

        /// <summary>
        /// Textures used by the game
        /// </summary>
        public static class Graphics
        {
            public static Texture2D Pixel;
            public static Texture2D Player;
        }

        /// <summary>
        /// Sound effects and music used by the game
        /// </summary>
        public static class Audio
        {

        }
    }
}
