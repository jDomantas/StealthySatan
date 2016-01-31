using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StealthySatan.Entities;

namespace StealthySatan
{
    class Tile
    {
        public enum BackType { None, Ventilation, Wall }
        public enum BlockType { None, Lamp, Wall, Floor, Ceiling, Crate, Transparent, Filing1, Filing2, Photocopy, Node }

        public BackType Back { get; }
        public BlockType Background { get; protected set; }
        public BlockType Foreground { get; protected set; }

        public Tile(BackType back, BlockType background, BlockType foreground)
        {
            if (IsSolid(Foreground) && ! IsSolid(Background))
                throw new System.Exception("no");

            Back = back;
            Background = background;
            Foreground = foreground;
        }

        static bool IsSolid(BlockType b)
        {
            return b != BlockType.None && b != BlockType.Lamp;
        }

        public virtual bool CanPass(Entity e)
        {
            if (e.InForeground) return !IsSolid(Foreground);
            else return !IsSolid(Background);
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
                        (int)((x - 0.2) * Map.ViewScale + 2),
                        (int)((y - 0.2) * Map.ViewScale + 2),
                        (int)Map.ViewScale,
                        (int)Map.ViewScale),
                        new Rectangle(index * 30 + 1, 1, 25, 25), Color.White);
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
                case BlockType.Filing1: sb.Draw(Resources.Graphics.Filing1, new Rectangle(x, y, (int)(Map.ViewScale * 1.2), (int)(Map.ViewScale * 3.2)), new Rectangle(77, 60, 210, 627), Color.White); return;
                case BlockType.Filing2: sb.Draw(Resources.Graphics.Filing2, new Rectangle(x, y, (int)(Map.ViewScale * 1.2), (int)(Map.ViewScale * 3.2)), new Rectangle(77, 60, 210, 627), Color.White); return;
                case BlockType.Transparent: return;
                case BlockType.Photocopy: sb.Draw(Resources.Graphics.Photocopy, new Rectangle((int)(x-120/190.0*Map.ViewScale),(int)(y-120/190.0*Map.ViewScale),(int)(Map.ViewScale*630/190.0), (int)(Map.ViewScale*539/190.0)), Color.White); return;
                case BlockType.Node: sb.Draw(Resources.Graphics.Nodes[0], new Rectangle(x,y,(int)(Map.ViewScale*2.2),(int)(Map.ViewScale*2.2)),Color.White); return;
                case BlockType.None: return;
            }

            sb.Draw(Resources.Graphics.Tiles, new Rectangle(x, y, (int)(Map.ViewScale * 1.2), (int)(Map.ViewScale * 1.2)), 
                new Rectangle(index * 30, 0, 30, 30), Color.White);
        }
    }
}
