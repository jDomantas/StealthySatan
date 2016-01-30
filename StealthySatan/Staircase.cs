using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StealthySatan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthySatan
{
    class Staircase
    {
        public const int Width = 2;
        public const int Height = 3;

        public Vector Location { get; }
        public Staircase Other { get; set; }

        public Staircase(Vector location, Staircase other)
        {
            Location = location;
            Other = other;
        }

        public bool DoesIntersect(Entity e)
        {
            double x = e.Position.X + e.Width / 2;
            double y = e.Position.Y + e.Height / 2;
            return x > Location.X && x < Location.X + Width && y > Location.Y && y < Location.Y + Height;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Resources.Graphics.Pixel, new Rectangle(
                (int)((Location.X - 0.2) * Map.ViewScale), 
                (int)((Location.Y - 0.2) * Map.ViewScale), 
                (int)(Width * Map.ViewScale), 
                (int)(Height * Map.ViewScale)), 
                Color.DarkGray);
        }

        public Vector GetCenter()
        {
            return new Vector(Location.X + Width/2, Location.Y + Height/2);
        }
    }
}
