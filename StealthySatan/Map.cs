using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StealthySatan.Entities;
using System;
using System.Collections.Generic;

namespace StealthySatan
{
    class Map
    {
        public const double TileSize = 1;
        public const double ViewScale = 20;

        public int WidthInTiles { get; }
        public int HeightInTiles { get; }

        private Tile[,] Tiles { get; }

        private List<Entity> Entities;

        public Map(int width, int height)
        {
            WidthInTiles = width;
            HeightInTiles = height;

            Tiles = new Tile[width, height];

            string[] tempMap = new string[]
            {
                "................................",
                "................................",
                "###############.................",
                "................................",
                "....................xx..........",
                "........##################......",
                "................................",
                "................................",
                "................................",
                "................................",
                "................xxx.............",
                "................xxx.............",
                "...........########.............",
                "................................",
                "................................",
                "................................",
                "................................",
                "................................",
                "................................",
                "................................",
                "################................",
                "................................",
                "................................",
                "................................",
            };

            for (int x = 0; x < WidthInTiles; x++)
                for (int y = 0; y < HeightInTiles; y++)
                    if (tempMap[y][x] == '.')
                        Tiles[x, y] = new Tile(Tile.TileType.Background, null);
                    else if (tempMap[y][x] == '#')
                        Tiles[x, y] = new Tile(Tile.TileType.Wall, null);
                    else
                        Tiles[x, y] = new Tile(Tile.TileType.Object, null);

            Entities = new List<Entity>();
            Entities.Add(new Player(this));
        }

        /// <summary>
        /// Adds entity to the map
        /// </summary>
        /// <param name="e">Entity to be added</param>
        public void AddEntity(Entity e)
        {
            Entities = new List<Entity>();
        }

        /// <summary>
        /// Checks if entity e can pass tile at (x, y)
        /// </summary>
        /// <param name="e">Entity to test</param>
        /// <param name="x">Tile X position</param>
        /// <param name="y">Tile Y position</param>
        /// <returns>Returns if entity can pass the tile</returns>
        public bool CanPassTile(Entity e, int x, int y)
        {
            if (x < 0 || y < 0 || x >= WidthInTiles || y >= HeightInTiles)
                return false;
            return Tiles[x, y].CanPass(e);
        }

        /// <summary>
        /// Tests if given entity intersects with any objects
        /// </summary>
        /// <param name="e">Entity to be tested</param>
        /// <returns>Returns if entity intersects any objects</returns>
        public bool IntersectsObjects(Entity e)
        {
            int left = (int)Math.Floor(e.Position.X - 0.001);
            int right = (int)Math.Ceiling(e.Position.X + e.Width + 0.001);
            int top = (int)Math.Floor(e.Position.Y - 0.001);
            int bottom = (int)Math.Ceiling(e.Position.Y + e.Height + 0.001);

            if (left < 0) left = 0;
            if (top < 0) top = 0;
            if (right > WidthInTiles) right = WidthInTiles;
            if (bottom > HeightInTiles) bottom = HeightInTiles;

            for (int x = left; x < right; x++)
                for (int y = top; y < bottom; y++)
                    if (Tiles[x, y].Type == Tile.TileType.Object)
                        return true;

            return false;   
        }

        public void Update()
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
                Entities[i].Update();

            Entities.RemoveAll(e => e.Removed);
        }
        
        public void Draw(SpriteBatch sb)
        {
            for (int x = 0; x < WidthInTiles; x++)
                for (int y = 0; y < HeightInTiles; y++)
                {
                    Color tileColor = Color.LightGray;
                    if (Tiles[x, y].Type == Tile.TileType.Object) tileColor = Color.Black;
                    else if (Tiles[x, y].Type == Tile.TileType.Wall) tileColor = Color.Red;

                    sb.Draw(Resources.Graphics.Pixel, new Rectangle(
                        (int)Math.Round(x * ViewScale),
                        (int)Math.Round(y * ViewScale),
                        (int)Math.Round(ViewScale),
                        (int)Math.Round(ViewScale)), tileColor);
                }

            for (int i = 0; i < Entities.Count; i++)
                Entities[i].Draw(sb);
        }
    }
}
