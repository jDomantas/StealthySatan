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
        public enum Disguise { Player, Invisible, Civilian, Policeman }
        private const double Gravity = 0.01;
        private const double MoveSpeed = 0.1;
        private const double JumpPower = 0.255;
        private Vector Velocity;
        private bool LightImmune;
        public Disguise CurrentDisguise { get; private set; }

        public Player(Map map) : base(map, 0.9, 0.9)
        {
            Velocity = Vector.Zero;
            Position = new Vector(5, 5);
            CurrentDisguise = Disguise.Player;
        }

        private void CheckForPossesions()
        {
            Entity col = Map.GetIntersectingEntity(this);
            if (col == null)
                return;
            if (col is Policeman)
            {
                CurrentDisguise = Disguise.Policeman;
                //to do: kill col
                col.Kill();
            }
            
        }

        public override void Update()
        {
            if (!InForeground && !LightImmune && Map.IsLit(this))
                InForeground = true;

            if (InForeground)
            {
                if (CurrentDisguise == Disguise.Player)
                    CheckForPossesions();
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

                if (OnGround && InputHandler.IsTyped(InputHandler.Key.Up) && CurrentDisguise == Disguise.Player)
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
                        toEnter.Location.Y + Staircase.Height - Height);

                    InForeground = (CurrentDisguise != Disguise.Player);
                    LightImmune = true;
                }
            }


            if (InputHandler.IsTyped(InputHandler.Key.Hide))
            {
                if ((!InForeground || !Map.IntersectsObjects(this)) && CurrentDisguise == Disguise.Player)
                    InForeground = !InForeground;
                else if (CurrentDisguise != Disguise.Player && CurrentDisguise != Disguise.Invisible)
                {
                    CurrentDisguise = Disguise.Player;
                    // to do: corpses n animations n shit
                }
            }
                

            base.Update();
        }

        public void DrawAsPoliceman(SpriteBatch sb)
        {
            var rect = new Rectangle(
                (int)Math.Round((Position.X - 0.35) * Map.ViewScale),
                (int)Math.Round((Position.Y - 1.4) * Map.ViewScale),
                (int)Math.Round(1.6 * Map.ViewScale),
                (int)Math.Round(2.3 * Map.ViewScale));
            sb.Draw(Resources.Graphics.Pixel, rect, Facing == Direction.Left ? Color.Blue : Color.Green);
        }

        public override void Draw(SpriteBatch sb)
        {
            switch(CurrentDisguise)
            {
                case Disguise.Policeman:
                    DrawAsPoliceman(sb);
                    return;
                case Disguise.Invisible:
                    return;
                
            }

            Rectangle rect = new Rectangle(
                (int)Math.Round((Position.X - Width) * Map.ViewScale),
                (int)Math.Round((Position.Y - Height * Math.Sqrt(2)) * Map.ViewScale),
                (int)Math.Round(Width * 3 * Map.ViewScale),
                (int)Math.Round(Height * 3 * Map.ViewScale));

            if (!InForeground)
            {
                sb.Draw(Resources.Graphics.PlayerStand, rect, null, Color.LightGray, 
                    0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            else if (!OnGround)
            {
                int frame = 0;
                if (Velocity.Y > -0.2) frame = 1;
                if (Velocity.Y > 0) frame = 2;
                if (Velocity.Y > 0.2) frame = 3;
                sb.Draw(Resources.Graphics.PlayerWalk[frame], rect, null, Color.White,
                    0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            else if (DistanceWalked < 0.001)
            {
                sb.Draw(Resources.Graphics.PlayerStand, rect, null, Color.White,
                    0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            else
            {
                int frame = ((int)Math.Round(DistanceWalked * 2)) % 8;
                sb.Draw(Resources.Graphics.PlayerWalk[frame], rect, null, Color.White,
                    0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }
    }
}
