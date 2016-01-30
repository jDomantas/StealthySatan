using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthySatan.Entities
{
    class Civilian : Entity
    {
        private int WalkTimer, ScareTimer, StairCooldown, PreserveDirection;
        private double MoveSpeed = 0.1;

        public Civilian(Map map, Vector position) : base(map, 1.6, 2.3)
        {
            Position = position;
        }

        public override void Update()
        {
            if (CheckPlayerVisibility())
            {
                ScareTimer = 120 + Map.Random.Next(60);
                if (PreserveDirection <= 0 ||
                    Math.Abs(Map.PlayerEntity.Position.X + Map.PlayerEntity.Width/2 - Position.X - Width/2) <= 4)
                {
                    if (Map.PlayerEntity.Position.X + Map.PlayerEntity.Width/2 < Position.X + Width/2)
                        Facing = Direction.Right;
                    else
                        Facing = Direction.Left;
                }
                else
                {
                    PreserveDirection--;
                }
                
            }
            if (ScareTimer > 0)
            {
                if (StairCooldown <= 0)
                {
                    Staircase toEnter = Map.GetIntersectingStaircase(this);
                    if (toEnter != null)
                    {
                        toEnter = toEnter.Other;
                        Position = new Vector(
                            toEnter.Location.X + Staircase.Width / 2 - Width / 2,
                            toEnter.Location.Y + Staircase.Height - Height);
                        StairCooldown = 60;
                    }
                }
                else
                {
                    StairCooldown--;
                }
                ScareTimer--;
                if (MoveHorizontal(Facing == Direction.Left ? -MoveSpeed * 1.2 : MoveSpeed * 1.2))
                {
                    Facing = Facing == Direction.Left ? Direction.Right : Direction.Left;
                    PreserveDirection = 60 + Map.Random.Next(30);
                }
                DistanceWalked += MoveSpeed * 1.2;
            }
            else
            {
                if (WalkTimer > 0)
                {
                    if (MoveHorizontal(Facing == Direction.Left ? -MoveSpeed : MoveSpeed))
                        Facing = Facing == Direction.Left ? Direction.Right : Direction.Left;
                    DistanceWalked += MoveSpeed;
                    WalkTimer--;
                }
                else
                {
                    DistanceWalked = 0;
                    int choice = Map.Random.Next(300);
                    if (choice < 2)
                        Facing = Facing == Direction.Left ? Direction.Right : Direction.Left;
                    else if (choice < 3)
                        WalkTimer = Map.Random.Next(60) + 60;
                }
            }
            MoveVertical(0.5);
        }

        public override void Draw(SpriteBatch sb)
        {
            var rect = new Rectangle(
                   (int)Math.Round((Position.X - Width) * Map.ViewScale),
                   (int)Math.Round((Position.Y - Height * 0.59) * Map.ViewScale),
                   (int)Math.Round(Width * 2.7 * Map.ViewScale),
                   (int)Math.Round(Width * 2.7 / 600 * 512 * Map.ViewScale));

            Texture2D texture;
            if (ScareTimer > 0)
                texture = Resources.Graphics.ScaredManWalk[(int)Math.Floor(DistanceWalked * 2) % 8];
            else if (DistanceWalked > 0)
                texture = Resources.Graphics.ManWalk[(int)Math.Floor(DistanceWalked * 2) % 8];
            else
                texture = Resources.Graphics.ManStand;
            sb.Draw(
                texture, rect, null, Color.White, 0, Vector2.Zero,
                Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}
