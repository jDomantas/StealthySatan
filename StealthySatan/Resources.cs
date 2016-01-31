using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
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

            Graphics.ScaredManWalk = new Texture2D[8];
            for (int i = 0; i < 8; i++)
                Graphics.ScaredManWalk[i] = content.Load<Texture2D>("Graphics/man/scared/spookedman" + (i + 1));

            Graphics.PossesedManStand = content.Load<Texture2D>("Graphics/man/possesed/spookyman0");
            Graphics.PossesedManWalk = new Texture2D[8];
            for (int i = 0; i < 8; i++)
                Graphics.PossesedManWalk[i] = content.Load<Texture2D>("Graphics/man/possesed/spookyman" + (i + 1));

            Graphics.PlayerFade = new Texture2D[3];
            for (int i = 0; i < 3; i++)
                Graphics.PlayerFade[i] = content.Load<Texture2D>("Graphics/imp/dissapear" + (i + 1));

            Graphics.PlayerDeath = new Texture2D[8];
            for (int i = 0; i < 8; i++)
                Graphics.PlayerDeath[i] = content.Load<Texture2D>("Graphics/imp/deth" + Math.Min((i + 1), 4));

            Graphics.PossessAnimation = new Texture2D[6];
            for (int i = 0; i < 6; i++)
                Graphics.PossessAnimation[i] = content.Load<Texture2D>("Graphics/possess/possess" + (i + 1));

            Graphics.ManDeath = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                Graphics.ManDeath[i] = content.Load<Texture2D>("Graphics/man/death/businessdeth" + (i + 1));

            Graphics.Tiles = content.Load<Texture2D>("Graphics/tiles");



            Graphics.CopEvilGun = new Texture2D[7];
            for (int i = 0; i < 7; i++)
                Graphics.CopEvilGun[i] = content.Load<Texture2D>("Graphics/cop/evilcopgun" + (i));
            
            Graphics.CopEvilPrep = new Texture2D[7];
            for (int i = 0; i < 7; i++)
                Graphics.CopEvilPrep[i] = content.Load<Texture2D>("Graphics/cop/evilcopprep" + (i));
            
            Graphics.CopEvil = new Texture2D[7];
            for (int i = 0; i < 7; i++)
                Graphics.CopEvil[i] = content.Load<Texture2D>("Graphics/cop/evilcop" + (i));
            
            Graphics.CopGun = new Texture2D[7];
            for (int i = 0; i < 7; i++)
                Graphics.CopGun[i] = content.Load<Texture2D>("Graphics/cop/copgun" + (i));
            
            Graphics.CopLight = new Texture2D[7];
            for (int i = 0; i < 7; i++)
                Graphics.CopLight[i] = content.Load<Texture2D>("Graphics/cop/megacop" + (i));
            
            Graphics.CopPrep = new Texture2D[7];
            for (int i = 0; i < 7; i++)
                Graphics.CopPrep[i] = content.Load<Texture2D>("Graphics/cop/copprep" + (i));
            
            Graphics.Cop = new Texture2D[7];
            for (int i = 0; i < 7; i++)
                Graphics.Cop[i] = content.Load<Texture2D>("Graphics/cop/cop" + (i));

            //======================================================================

            Audio.BackgroundMusic = content.Load<Song>("Audio/music/bgmusic");
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
            public static Texture2D[] PlayerFade;
            public static Texture2D[] PlayerDeath;
            public static Texture2D[] ManWalk;
            public static Texture2D[] ScaredManWalk;
            public static Texture2D ManStand;
            public static Texture2D PossesedManStand;
            public static Texture2D[] PossesedManWalk;
            public static Texture2D[] PossessAnimation;
            public static Texture2D[] ManDeath;

            public static Texture2D[] Cop;
            public static Texture2D[] CopPrep;
            public static Texture2D[] CopGun;
            public static Texture2D[] CopLight;
            public static Texture2D[] CopEvil;
            public static Texture2D[] CopEvilPrep;
            public static Texture2D[] CopEvilGun;

            public static Texture2D Tiles;
            public static Texture2D FadingTriangle;
            public static Texture2D FadingRectangle;
        }

        /// <summary>
        /// Sound effects and music used by the game
        /// </summary>
        public static class Audio
        {
            public static Song BackgroundMusic;
        }
    }
}
