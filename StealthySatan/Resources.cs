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
            Graphics.PlayerStand = content.Load<Texture2D>("Graphics/imp/imp0");
            Graphics.PlayerWalk = new Texture2D[8];
            for (int i = 0; i < 8; i++)
                Graphics.PlayerWalk[i] = content.Load<Texture2D>("Graphics/imp/imp" + (i + 1));

            Graphics.ManStand = content.Load<Texture2D>("Graphics/man/normal/man0");
            Graphics.ManWalk = new Texture2D[8];
            for (int i = 0; i < 8; i++)
                Graphics.ManWalk[i] = content.Load<Texture2D>("Graphics/man/normal/man" + (i + 1));

            // ======================
            Graphics.ScaredManWalk = new Texture2D[8];
            for (int i = 0; i < 8; i++)
                Graphics.ScaredManWalk[i] = content.Load<Texture2D>("Graphics/man/scared/spookedman" + (i + 1));

            Graphics.PossesedManStand = content.Load<Texture2D>("Graphics/man/possesed/spookyman0");
            Graphics.PossesedManWalk = new Texture2D[8];
            for (int i = 0; i < 8; i++)
                Graphics.PossesedManWalk[i] = content.Load<Texture2D>("Graphics/man/possesed/spookyman" + (i + 1));
            //=======================
            Graphics.Tiles = content.Load<Texture2D>("Graphics/tiles");
        }

        /// <summary>
        /// Textures used by the game
        /// </summary>
        public static class Graphics
        {
            public static Texture2D Pixel;
            public static Texture2D Triangle;
            public static Texture2D[] PlayerWalk;
            public static Texture2D PlayerStand;
            public static Texture2D[] ManWalk;
            public static Texture2D[] ScaredManWalk;
            public static Texture2D ManStand;
            public static Texture2D PossesedManStand;
            public static Texture2D[] PossesedManWalk;
            public static Texture2D Tiles;
            public static Texture2D FadingTriangle;
            public static Texture2D FadingRectangle;
        }

        /// <summary>
        /// Sound effects and music used by the game
        /// </summary>
        public static class Audio
        {

        }
    }
}
