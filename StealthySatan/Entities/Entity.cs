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
        public enum Direction { Left = 0, Right }

        public Vector Position { get; protected set; }
        public double Width { get; }
        public double Height { get; }
        public Map Map { get; }

        public bool InForeground { get; protected set; }
        public bool OnGround { get; private set; }
        public bool Removed { get; protected set; }
        public Direction Facing { get; protected set; }
        private int AlarmTimer;

        protected double DistanceWalked;

        public Entity(Map map, double width, double height)
        {
            Map = map;

            Width = width;
            Height = height;

            Position = Vector.Zero;

            InForeground = true;

            Facing = Direction.Left;
        }

        public virtual void Kill() {
            Removed = true;
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

        public bool CanSeeOther(Entity other) {
            return CanSeeLocation(other.Position + new Vector(other.Width, other.Height) / 2);
        }

        public bool CanSeeLocation(Vector location)
        {
            if ((Position - location).Length / Map.TileSize > 18)
                return false;
            return Map.IsRectangleEmpty((int)Math.Floor((Position.X + Width / 2) / Map.TileSize),
                                        (int)Math.Floor((Position.Y + Height / 2) / Map.TileSize),
                                        (int)Math.Floor(location.X / Map.TileSize),
                                        (int)Math.Floor(location.Y / Map.TileSize));

        }

        public bool CheckPlayerVisibility()
        {
            if (Map.PlayerEntity.Removed)
            { AlarmTimer = 30; return false; }
            if (Map.PlayerEntity.CurrentDisguise != Player.Disguise.Player && !Map.PlayerEntity.CanSeeThroughDisguise)
            { AlarmTimer = 30; return false; }
            if (!Map.PlayerEntity.InForeground)
            { AlarmTimer = 30; return false; }
            if (Map.PlayerEntity.Position.X + Map.PlayerEntity.Width / 2 > Position.X + Width / 2 && Facing == Direction.Left)
            { AlarmTimer = 30; return false; }
            if (Map.PlayerEntity.Position.X + Map.PlayerEntity.Width / 2 < Position.X + Width / 2 && Facing == Direction.Right)
            { AlarmTimer = 30; return false; }

            if (CanSeeOther(Map.PlayerEntity))
            {
                if (AlarmTimer-- < 0)
                    Map.TriggerAlarm();
                return true;
            }

            AlarmTimer = 30;
            return false;
        }

        public Vector GetCenter()
        {
            return new Vector(Position.X + Width/2, Position.Y + Height/2);
        }

        public virtual void AllarmTriggered(Vector location)
        {

        }

        public bool DoesCollide(Entity other)
        {
            return Position.X <= other.Position.X + other.Width &&
                   Position.Y <= other.Position.Y + other.Height &&
                   other.Position.X <= Position.X + Width &&
                   other.Position.Y <= Position.Y + Height;
        }

        public virtual void CallFromStaircase(Staircase s)
        {

        }
    }
}
