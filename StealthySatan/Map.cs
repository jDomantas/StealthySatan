using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StealthySatan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StealthySatan
{
    class Map
    {
        public const double TileSize = 1; // never ever change this 
        public const double ViewScale = 40;

        public int WidthInTiles { get; }
        public int HeightInTiles { get; }

        private Tile[,] Tiles { get; }

        private int GuardsToSpawn;
        private int NextGuardTimer;
        private Vector GuardTarget;

        private List<Entity> Entities;
        private List<LitArea> LitAreas;
        private List<Staircase> Staircases;
        private List<Particle> Particles;

        public Player PlayerEntity { get; private set; }

        public Random Random { get; }

        private int TicksWithoutPlayer;

        public Map(int width, int height, Texture2D createFrom)
        {
            WidthInTiles = width;
            HeightInTiles = height;

            Tiles = new Tile[width, height];
            Entities = new List<Entity>();
            LitAreas = new List<LitArea>();
            Staircases = new List<Staircase>();
            Particles = new List<Particle>();

            Random = new Random((int)DateTime.Now.Ticks);

            GuardsToSpawn = 0;
            

            Entities.Add(PlayerEntity = new Player(this));

            InitMap(createFrom);
            AddMapObjects();
        }

        private void InitMap(Texture2D texture)
        {
            Color[] textureData = new Color[WidthInTiles * HeightInTiles];
            texture.GetData(textureData);
            for (int x = 0; x < WidthInTiles; x++)
                for (int y = 0; y < HeightInTiles; y++)
                    if (textureData[x + y * WidthInTiles] == new Color(127, 127, 127))
                        Tiles[x, y] = new Tile(Tile.BackType.Ventilation, Tile.BlockType.None, Tile.BlockType.None);
                    else if (textureData[x + y * WidthInTiles] == Color.Black)
                    {
                        if (y > 0 && textureData[x + (y - 1) * WidthInTiles] == new Color(127, 127, 127))
                            Tiles[x, y] = new Tile(Tile.BackType.Wall, Tile.BlockType.Ceiling, Tile.BlockType.Ceiling);
                        else if (y < HeightInTiles - 1 && textureData[x + (y + 1) * WidthInTiles] == new Color(127, 127, 127))
                            Tiles[x, y] = new Tile(Tile.BackType.Wall, Tile.BlockType.Floor, Tile.BlockType.Floor);
                        else
                            Tiles[x, y] = new Tile(Tile.BackType.Wall, Tile.BlockType.Wall, Tile.BlockType.Wall);
                    }
                    else if (textureData[x + y * WidthInTiles] == new Color(63, 72, 204))
                    {
                        Tiles[x, y] = new Tile(Tile.BackType.Wall, Tile.BlockType.Crate, Tile.BlockType.None);
                    }
                    else if (textureData[x + y * WidthInTiles] == Color.White)
                    {
                        Tiles[x, y] = new Tile(Tile.BackType.Wall, Tile.BlockType.None, Tile.BlockType.None);
                    }
                    else if (textureData[x + y * WidthInTiles] == new Color(237, 28, 36))
                    {

                        Tiles[x, y] = new Tile(Tile.BackType.Wall, Tile.BlockType.None, Tile.BlockType.Lamp);
                        if (x > 0 && x < WidthInTiles - 1 &&
                            textureData[x + y * WidthInTiles + 1] == new Color(237, 28, 36) &&
                            textureData[x + y * WidthInTiles - 1] == new Color(237, 28, 36))
                            LitAreas.Add(new LitArea(x + 0.6, y + 0.4, 5.5, 2.3, 8));
                    }
        }

        private void AddMapObjects()
        {
            Entities.Add(new Policeman(this, new Vector(10, 15.4), 4, 13, false));
            Entities.Add(new Policeman(this, new Vector(45, 15.4), false));
            Entities.Add(new Policeman(this, new Vector(30, 24.4), true));

            Entities.Add(new Civilian(this, new Vector(45, 6.4)));
            Entities.Add(new Civilian(this, new Vector(45, 15.4)));
            Entities.Add(new Civilian(this, new Vector(30, 24.4)));
            
            AddPairOfStairaces(new Vector(30, 6), new Vector(30, 15));
            AddPairOfStairaces(new Vector(2, 15), new Vector(2, 24));
        }

        private void AddPairOfStairaces(Vector pos1, Vector pos2)
        {
            Staircase first = new Staircase(pos1, null);
            Staircase second = new Staircase(pos2, first);
            first.Other = second;

            Staircases.Add(first);
            Staircases.Add(second);
        }

        public void AddEntity(Entity e)
        {
            // lol
            //Entities = new List<Entity>();
            Entities.Add(e);
        }

        public void AddParticle(Particle p)
        {
            Particles.Add(p);
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
                    if (Tiles[x, y].Background != Tile.BlockType.None && Tiles[x, y].Foreground == Tile.BlockType.None)
                        return true;

            return false;   
        }

        /// <summary>
        /// Checks if given entity is in lit area
        /// </summary>
        /// <param name="e">Entity to be tested</param>
        /// <returns>Returns if entity is lit</returns>
        public bool IsLit(Entity e)
        {
            for (int i = 0; i < LitAreas.Count; i++)
                if (LitAreas[i].DoesIntersect(e))
                    return true;
            return false;
        }

        public Staircase GetIntersectingStaircase(Entity e)
        {
            for (int i = 0; i < Staircases.Count; i++)
                if (Staircases[i].DoesIntersect(e))
                    return Staircases[i];
            return null;
        }

        public Entity GetIntersectingEntity(Entity e)
        {
            for (int i = 0; i < Entities.Count; i++)
                if (Entities[i].DoesCollide(e) && Entities[i] != e)
                    return Entities[i];
            return null;
        }

        public void Update()
        {
            if (PlayerEntity.Removed)
            {
                TicksWithoutPlayer++;
                if (TicksWithoutPlayer > 120)
                    PlayerEntity.Respawn();
            }
            else
                TicksWithoutPlayer = 0;

            for (int i = Entities.Count - 1; i >= 0; i--)
                Entities[i].Update();

            Entities.RemoveAll(e => e.Removed);

            for (int i = Particles.Count - 1; i >= 0; i--)
                Particles[i].Update();

            Particles.RemoveAll(e => e.Removed);
        }

        public void Draw(SpriteBatch sb)
        {
            for (int y = HeightInTiles - 1; y >= 0; y--)
                for (int x = WidthInTiles - 1; x >= 0; x--)
                    Tiles[x, y].DrawBackground(sb, x, y);

            for (int i = 0; i < Staircases.Count; i++)
                Staircases[i].Draw(sb);

            for (int y = HeightInTiles - 1; y >= 0; y--)
                for (int x = WidthInTiles - 1; x >= 0; x--)
                    Tiles[x, y].DrawBackgroundLayer(sb, x, y);

            for (int i = 0; i < Entities.Count; i++)
                Entities[i].Draw(sb);

            for (int i = 0; i < Particles.Count; i++)
                Particles[i].Draw(sb);

            for (int y = HeightInTiles - 1; y >= 0; y--)
                for (int x = WidthInTiles - 1; x >= 0; x--)
                    Tiles[x, y].DrawForegroundLayer(sb, x, y);
        }

        public void DrawBlend(SpriteBatch sb)
        {
            for (int i = 0; i < LitAreas.Count; i++)
                LitAreas[i].Draw(sb);
        }

        public bool IsRectangleEmpty(int x1, int y1, int x2, int y2)
        {
            if (x1 < 0) x1 = 0;
            if (y1 < 0) y1 = 0;
            if (x2 < 0) x2 = 0;
            if (y2 < 0) y2 = 0;

            if (x1 >= WidthInTiles) x1 = WidthInTiles - 1;
            if (y1 >= HeightInTiles) y1 = HeightInTiles - 1;
            if (x2 >= WidthInTiles) x2 = WidthInTiles - 1;
            if (y2 >= HeightInTiles) y2 = HeightInTiles - 1;

            if (x2 < x1)
            {
                int tmp = x2;
                x2 = x1;
                x1 = tmp;
            }

            if (y2 < y1)
            {
                int tmp = y2;
                y2 = y1;
                y1 = tmp;
            }

            for (int x = x1; x <= x2; x++)
                for (int y = y1; y <= y2; y++)
                    if (Tiles[x, y].Foreground != Tile.BlockType.None && Tiles[x, y].Foreground != Tile.BlockType.Lamp)
                        return false;

            return true;
        }

        /// <summary>
        /// Triggers alarm at the player's current location
        /// </summary>
        public void TriggerAlarm()
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
                Entities[i].AllarmTriggered(PlayerEntity.Position + new Vector(PlayerEntity.Width, PlayerEntity.Height) / 2);

            // also start spawning policemen

            foreach (var s in Staircases
                    .Where(st => PlayerEntity.CanSeeLocation(st.Location + new Vector(Staircase.Width, Staircase.Height) / 2))
                    .Select(st => st.Other))
                for (int i = Entities.Count - 1; i >= 0; i--)
                    Entities[i].CallFromStaircase(s); 
        }

        public Entity GetTarget(Entity e)
        {
            return Entities.Where(en => e.CanSeeOther(en) && en != e).OrderBy(en => (e.GetCenter()-en.GetCenter()).LengthSquared).FirstOrDefault();
        }
    }
}
