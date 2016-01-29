using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthySatan.Entities
{
    abstract class Entity
    {
        public Vector Position { get; protected set; }
        public double Width { get; }
        public double Height { get; }
        public Map Map { get; }

        public bool InForeground { get; protected set; }
        public bool OnGround { get; private set; }
        public bool Removed { get; protected set; }
        
        public Entity(Map map, double width, double height)
        {
            Map = map;

            Width = width;
            Height = height;

            Position = Vector.Zero;

            InForeground = true;

            bool val = MoveUp(1);
        }

        public virtual void Update() { }

        /// <summary>
        /// Move entity horizontally by dx units and vertically by dy units
        /// </summary>
        /// <param name="dx">Distance to move horizontally</param>
        /// <param name="dy">Distance to move vertically</param>
        protected void Move(double dx, double dy)
        {
            MoveHorizontal(dx);
            MoveVertical(dy);
        }

        /// <summary>
        /// Move entity by given vector
        /// </summary>
        /// <param name="delta">Distance and direction to move by</param>
        protected void Move(Vector delta)
        {
            Move(delta.X, delta.Y);
        }

        /// <summary>
        /// Move horizontally by dx units (left if negative, right if positive)
        /// </summary>
        /// <param name="dx">Distance and direction to move</param>
        /// <returns>Returns if collided with a tile</returns>
        protected bool MoveHorizontal(double dx)
        {
            if (dx < 0) return MoveLeft(-dx);
            else if (dx > 0) return MoveRight(dx);
            else return false;
        }

        /// <summary>
        /// Move vertically by dy units (up if negative, down if positive)
        /// </summary>
        /// <param name="dy">Distance and direction to move</param>
        /// <returns>Returns if collided with a tile</returns>
        protected bool MoveVertical(double dy)
        {
            OnGround = false;

            if (dy < 0) return MoveUp(-dy);
            else if (dy > 0) return MoveDown(dy);
            else return false;
        }
        
        private bool MoveUp(double dist)
        {
            int left = (int)Math.Floor(Position.X / Map.TileSize);
            int right = (int)Math.Ceiling((Position.X + Width) / Map.TileSize);
            int top = (int)Math.Floor(Position.Y / Map.TileSize) - 1;
            int newTop = (int)Math.Floor((Position.Y - dist) / Map.TileSize);

            for (int y = top; y >= newTop; y--)
                for (int x = left; x < right; x++)
                    if (!Map.CanPassTile(this, x, y))
                    {
                        Position.Y = y + Map.TileSize;
                        return true;
                    }

            Position.Y -= dist;
            return false;
        }
        
        private bool MoveDown(double dist)
        {
            int left = (int)Math.Floor(Position.X / Map.TileSize);
            int right = (int)Math.Ceiling((Position.X + Width) / Map.TileSize);
            int bottom = (int)Math.Ceiling((Position.Y + Height) / Map.TileSize);
            int newBottom = (int)Math.Floor((Position.Y + Height + dist) / Map.TileSize);

            for (int y = bottom; y <= newBottom; y++)
                for (int x = left; x < right; x++)
                    if (!Map.CanPassTile(this, x, y))
                    {
                        Position.Y = y - Height;
                        OnGround = true;
                        return true;
                    }

            Position.Y += dist;
            OnGround = false;
            return false;
        }
        
        private bool MoveLeft(double dist)
        {
            int top = (int)Math.Floor(Position.Y / Map.TileSize);
            int bottom = (int)Math.Ceiling((Position.Y + Height) / Map.TileSize);
            int left = (int)Math.Floor(Position.X / Map.TileSize) - 1;
            int newLeft = (int)Math.Floor((Position.X - dist) / Map.TileSize);

            for (int x = left; x >= newLeft; x--)
                for (int y = top; y < bottom; y++)
                    if (!Map.CanPassTile(this, x, y))
                    {
                        Position.X = x + Map.TileSize;
                        return true;
                    }

            Position.X -= dist;
            return false;
        }
        
        private bool MoveRight(double dist)
        {
            int top = (int)Math.Floor(Position.Y / Map.TileSize);
            int bottom = (int)Math.Ceiling((Position.Y + Height) / Map.TileSize);
            int right = (int)Math.Ceiling((Position.X + Width) / Map.TileSize);
            int newRight = (int)Math.Floor((Position.X + Width + dist) / Map.TileSize);

            for (int x = right; x <= newRight; x++)
                for (int y = top; y < bottom; y++)
                    if (!Map.CanPassTile(this, x, y))
                    {
                        Position.X = x - Width;
                        return true;
                    }

            Position.X += dist;
            return false;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            // generic renderer to show hitbox
            sb.Draw(Resources.Graphics.Pixel, GetScreenBounds(), Color.Gray);
        }

        /// <summary>
        /// Returns position and size of the entity's hitbox on screen
        /// </summary>
        /// <returns>Returns position and size of the entity's hitbox on screen</returns>
        public Rectangle GetScreenBounds()
        {
            return new Rectangle(
                (int)Math.Round(Position.X * Map.ViewScale),
                (int)Math.Round(Position.Y * Map.ViewScale),
                (int)Math.Round(Width * Map.ViewScale),
                (int)Math.Round(Height * Map.ViewScale));
        }
    }
}
