using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace StealthySatan.Entities
{
    class Player : Entity
    {
        private const double Gravity = 0.01;
        private const double MoveSpeed = 0.1;
        private const double JumpPower = 0.255;
        private Vector Velocity;
        private bool LightImmune;

        public Player(Map map) : base(map, 0.9, 0.9)
        {
            Velocity = Vector.Zero;
            Position = new Vector(5, 5);
        }

        public override void Update()
        {
            if (!InForeground && !LightImmune && Map.IsLit(this))
                InForeground = true;

            if (InForeground)
            {
                LightImmune = false;

                // gravity, platformer physics
                Velocity.X = 0;
                if (InputHandler.IsPressed(InputHandler.Key.Left))
                {
                    Facing = Direction.Left;
                    Velocity.X -= MoveSpeed;
                    DistanceWalked += MoveSpeed;
                }
                else if (InputHandler.IsPressed(InputHandler.Key.Right))
                {
                    Facing = Direction.Right;
                    Velocity.X += MoveSpeed;
                    DistanceWalked += MoveSpeed;
                }
                else
                    DistanceWalked = 0;

                if (OnGround && InputHandler.IsTyped(InputHandler.Key.Up))
                    Velocity.Y = -JumpPower;

                Velocity.Y += Gravity;
                if (MoveHorizontal(Velocity.X)) Velocity.X = 0;
                if (MoveVertical(Velocity.Y)) Velocity.Y = 0;
            }
            else
            {
                // no gravity, wall climbing
                Velocity = Vector.Zero;
                if (InputHandler.IsPressed(InputHandler.Key.Left)) { Facing = Direction.Left; Velocity.X -= MoveSpeed; LightImmune = false; }
                if (InputHandler.IsPressed(InputHandler.Key.Right)) { Facing = Direction.Right; Velocity.X += MoveSpeed; LightImmune = false; }
                if (InputHandler.IsPressed(InputHandler.Key.Up)) { Velocity.Y -= MoveSpeed; LightImmune = false; }
                if (InputHandler.IsPressed(InputHandler.Key.Down)) { Velocity.Y += MoveSpeed; LightImmune = false; }

                Move(Velocity);
            }

            if (InputHandler.IsTyped(InputHandler.Key.Enter))
            {
                Staircase toEnter = Map.GetIntersectingStaircase(this);
                if (toEnter != null)
                {
                    toEnter = toEnter.Other;
                    Position = new Vector(
                        toEnter.Location.X + Staircase.Width / 2 - Width / 2, 
                        toEnter.Location.Y + Staircase.Height / 2 - Height / 2);

                    InForeground = false;
                    LightImmune = true;
                }
            }

            if (InputHandler.IsTyped(InputHandler.Key.Hide) && (!InForeground || !Map.IntersectsObjects(this)))
                InForeground = !InForeground;

            base.Update();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!InForeground)
            {
                sb.Draw(Resources.Graphics.Player, GetScreenBounds(), new Rectangle(0, 200, 100, 100), Color.LightGray, 
                    0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            else if (!OnGround)
            {
                sb.Draw(Resources.Graphics.Player, GetScreenBounds(), new Rectangle(0, 100, 100, 100), Color.White,
                    0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            else
            {
                int frame = ((int)Math.Round(DistanceWalked * 2)) % 4;
                sb.Draw(Resources.Graphics.Player, GetScreenBounds(), new Rectangle(100 * frame, 0, 100, 100), Color.White,
                    0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }
    }
}
