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
        private double MoveDistance;

        public Player(Map map) : base(map, 1.2, 1.2)
        {
            Velocity = Vector.Zero;
        }

        public override void Update()
        {
            if (InForeground)
            {
                // gravity, platformer physics
                Velocity.X = 0;
                if (InputHandler.IsPressed(InputHandler.Key.Left))
                {
                    Facing = Direction.Left;
                    Velocity.X -= MoveSpeed;
                    MoveDistance += MoveSpeed;
                }
                else if (InputHandler.IsPressed(InputHandler.Key.Right))
                {
                    Facing = Direction.Right;
                    Velocity.X += MoveSpeed;
                    MoveDistance += MoveSpeed;
                }
                else
                    MoveDistance = 0;

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
                if (InputHandler.IsPressed(InputHandler.Key.Left)) Velocity.X -= MoveSpeed;
                if (InputHandler.IsPressed(InputHandler.Key.Right)) Velocity.X += MoveSpeed;
                if (InputHandler.IsPressed(InputHandler.Key.Up)) Velocity.Y -= MoveSpeed;
                if (InputHandler.IsPressed(InputHandler.Key.Down)) Velocity.Y += MoveSpeed;

                Move(Velocity);
            }

            if (InputHandler.IsTyped(InputHandler.Key.Hide) && (!InForeground || !Map.IntersectsObjects(this)))
                InForeground = !InForeground;

            base.Update();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!InForeground)
            {
                sb.Draw(Resources.Graphics.Player, GetScreenBounds(), new Rectangle(0, 200, 100, 100), Color.White, 
                    0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            else if (!OnGround)
            {
                sb.Draw(Resources.Graphics.Player, GetScreenBounds(), new Rectangle(0, 100, 100, 100), Color.White,
                    0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            else
            {
                int frame = ((int)Math.Round(MoveDistance * 2)) % 4;
                sb.Draw(Resources.Graphics.Player, GetScreenBounds(), new Rectangle(100 * frame, 0, 100, 100), Color.White,
                    0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }
    }
}
