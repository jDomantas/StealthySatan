using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StealthySatan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthySatan
{
    class LitArea
    {
        private const int LightAlpha = 10;
        private const int LightValue = 255;
        private readonly Color LightColor = new Color(LightValue, LightValue, LightValue, LightAlpha);

        public double X { get; }
        public double Y { get; }
        public double Height { get; }
        public double TopWidth { get; }
        public double BottomWidth { get; }

        public LitArea(double x, double y, double height, double topWidth, double bottomWidth)
        {
            X = x;
            Y = y;
            Height = height;
            TopWidth = topWidth;
            BottomWidth = bottomWidth;
        }

        /// <summary>
        /// Checks if entity intersects this lit area
        /// </summary>
        /// <param name="e">Entity to be tested</param>
        /// <returns>Returns if entity intersects this lit area</returns>
        public bool DoesIntersect(Entity e)
        {
            // do simple check against entity's center
            double x = e.Position.X + e.Width / 2;
            double y = e.Position.Y + e.Height / 2;
            if (y < Y || y >= Y + Height) return false;

            double halfWidth = (TopWidth + (BottomWidth - TopWidth) * (y - Y) / Height) / 2;
            if (x < X - halfWidth || x > X + halfWidth)
                return false;

            return true;
        }

        public void Draw(SpriteBatch sb)
        {
            int leftX = (int)Math.Round((X - BottomWidth / 2) * Map.ViewScale);
            int topLeftX = (int)Math.Round((X - TopWidth / 2) * Map.ViewScale);
            int topRightX = (int)Math.Round((X + TopWidth / 2) * Map.ViewScale);
            int sideWidth = topLeftX - leftX;
            int innerWidth = topRightX - topLeftX;
            int topY = (int)Math.Round(Y * Map.ViewScale);
            int height = (int)Math.Round(Height * Map.ViewScale);

            sb.Draw(Resources.Graphics.FadingTriangle, new Rectangle(leftX, topY, sideWidth, height), null, LightColor,
                0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            sb.Draw(Resources.Graphics.FadingRectangle, new Rectangle(topLeftX, topY, innerWidth, height), LightColor);
            sb.Draw(Resources.Graphics.FadingTriangle, new Rectangle(topRightX, topY, sideWidth, height), LightColor);

            leftX = (int)Math.Round((X - (BottomWidth * 1.2) / 2) * Map.ViewScale);
            topLeftX = (int)Math.Round((X - (TopWidth * 1.3) / 2) * Map.ViewScale);
            topRightX = (int)Math.Round((X + (TopWidth * 1.3) / 2) * Map.ViewScale);
            sideWidth = topLeftX - leftX;
            innerWidth = topRightX - topLeftX;

            sb.Draw(Resources.Graphics.FadingTriangle, new Rectangle(leftX, topY, sideWidth, height), null, LightColor,
                0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            sb.Draw(Resources.Graphics.FadingRectangle, new Rectangle(topLeftX, topY, innerWidth, height), LightColor);
            sb.Draw(Resources.Graphics.FadingTriangle, new Rectangle(topRightX, topY, sideWidth, height), LightColor);
        }
    }
}
