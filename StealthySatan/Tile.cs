using Microsoft.Xna.Framework.Graphics;
using StealthySatan.Entities;

namespace StealthySatan
{
    class Tile
    {
        public enum TileType { Background, Object, Wall }

        public TileType Type { get; }
        public Texture2D Texture { get; }

        public Tile(TileType type, Texture2D texture)
        {
            Type = type;
            Texture = texture;
        }

        public bool CanPass(Entity e)
        {
            if (e.InForeground) return Type == TileType.Background || Type == TileType.Object;
            else return Type == TileType.Background;
        }
    }
}
