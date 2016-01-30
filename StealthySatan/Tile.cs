using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StealthySatan.Entities;

namespace StealthySatan
{
    class Tile
    {
        public enum BackType { None, Ventilation, Wall }
        public enum BlockType { None, Lamp, Wall, Floor, Ceiling, Crate }

        public BackType Back { get; }
        public BlockType Background { get; protected set; }
        public BlockType Foreground { get; protected set; }

        public Tile(BackType back, BlockType background, BlockType foreground)
        {
            if (background == BlockType.None && foreground != BlockType.None && foreground != BlockType.Lamp)
                throw new System.Exception("no");

            Back = back;
            Background = background;
            Foreground = foreground;
        }

        public virtual bool CanPass(Entity e)
        {
            if (e.InForeground) return Foreground == BlockType.None || Foreground == BlockType.Lamp;
            else return Background == BlockType.None;
        }

        public virtual void DrawBackground(SpriteBatch sb, int x, int y)
        {
            int index = -1;
            switch (Back)
            {
                case BackType.None: return;
                case BackType.Ventilation: index = 0; break;
                case BackType.Wall: index = 1; break;
                default: throw new System.Exception("no");
            }

            if (index != 0 && index != 1)
            {
                throw new System.Exception("also no");
            }

            sb.Draw(Resources.Graphics.Tiles, new Rectangle(
                        (int)((x - 0.2) * Map.ViewScale),
                        (int)((y - 0.2) * Map.ViewScale),
                        (int)Map.ViewScale,
                        (int)Map.ViewScale),
                        new Rectangle(index * 30, 0, 25, 25), Color.White);
        }

        public virtual void DrawBackgroundLayer(SpriteBatch sb, int x, int y)
        {
            DrawBlockAt(sb, Background,
                (int)((x - 0.2) * Map.ViewScale),
                (int)((y - 0.2) * Map.ViewScale));
        }

        public virtual void DrawForegroundLayer(SpriteBatch sb, int x, int y)
        {
            DrawBlockAt(sb, Foreground,
                (int)(x * Map.ViewScale),
                (int)(y * Map.ViewScale));
        }

        private void DrawBlockAt(SpriteBatch sb, BlockType block, int x, int y)
        {
            int index = 0;
            switch (block)
            {
                case BlockType.Ceiling: index = 2; break;
                case BlockType.Crate: index = 5; break;
                case BlockType.Floor: index = 3; break;
                case BlockType.Wall: index = 4; break;
                case BlockType.Lamp: index = 6; break;
                case BlockType.None: return;
            }

            sb.Draw(Resources.Graphics.Tiles, new Rectangle(x, y, (int)(Map.ViewScale * 1.2), (int)(Map.ViewScale * 1.2)), 
                new Rectangle(index * 30, 0, 30, 30), Color.White);
        }
    }
}
