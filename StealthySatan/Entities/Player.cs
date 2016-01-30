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
        private int InvisibilityProgress;
        public Disguise CurrentDisguise { get; private set; }
        
        public Player(Map map) : base(map, 0.9, 0.9)
        {
            Velocity = Vector.Zero;
            Position = new Vector(5, 5);
            CurrentDisguise = Disguise.Player;
        }

        public override void Kill()
        {
            base.Kill();

            int direction = Facing == Direction.Left ? 1 : -1;
            Rectangle rect = new Rectangle(
                (int)Math.Round((Position.X - Width + Width * direction * InvisibilityProgress / 20) * Map.ViewScale),
                (int)Math.Round((Position.Y - Height * Math.Sqrt(2) + InvisibilityProgress * Height / 15) * Map.ViewScale),
                (int)Math.Round(Width * 3 * Map.ViewScale),
                (int)Math.Round(Height * 3 * Map.ViewScale));
            Map.AddParticle(new Particle(rect, Resources.Graphics.PlayerDeath, 80, Facing == Direction.Left));
        }

        public void Respawn()
        {
            Position = new Vector(1.05,1.05);
            Removed = false;
            InForeground = false;
            Map.AddEntity(this);
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
            else if (col is Civilian)
            {
                CurrentDisguise = Disguise.Civilian;
                col.Kill();
            }
            
        }

        public override void Update()
        {

            if (InForeground && InvisibilityProgress > 0)
                InvisibilityProgress--;
            else if (!InForeground && InvisibilityProgress < 10)
                InvisibilityProgress++;

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

        public void DrawAsCivilian(SpriteBatch sb)
        {
            var rect = new Rectangle(
                   (int)Math.Round((Position.X - 1.75) * Map.ViewScale),
                   (int)Math.Round((Position.Y - 2.757) * Map.ViewScale),
                   (int)Math.Round(1.6 * 2.7 * Map.ViewScale),
                   (int)Math.Round(1.6 * 2.7 / 600 * 512 * Map.ViewScale));

            sb.Draw(
                DistanceWalked < 0.001 ? Resources.Graphics.PossesedManStand : Resources.Graphics.PossesedManWalk[(int)Math.Floor(DistanceWalked * 2) % 8],
                rect, null, Color.White, 0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

        }

        public override void Draw(SpriteBatch sb)
        {
            switch(CurrentDisguise)
            {
                case Disguise.Policeman:
                    DrawAsPoliceman(sb);
                    return;
                case Disguise.Civilian:
                    DrawAsCivilian(sb);
                    return;
                case Disguise.Invisible:
                    return;
                
            }

            int direction = Facing == Direction.Left ? 1 : -1;

            Rectangle rect = new Rectangle(
                (int)Math.Round((Position.X - Width + Width*direction*InvisibilityProgress/20) * Map.ViewScale),
                (int)Math.Round((Position.Y - Height * Math.Sqrt(2) + InvisibilityProgress * Height / 15) * Map.ViewScale),
                (int)Math.Round(Width * 3 * Map.ViewScale),
                (int)Math.Round(Height * 3 * Map.ViewScale));

            int ow = rect.Width;
            int oh = rect.Height;
            double multiplier = (1.0 + InvisibilityProgress / 40.0);
            rect.Width = (int)Math.Round(ow * multiplier);
            rect.Height = (int)Math.Round(oh * multiplier);
            rect.X -= (rect.Width - ow) / 2;
            rect.Y -= (rect.Height - oh) / 2;

            Texture2D texture;
            if (InvisibilityProgress == 0)
            {
                if (!OnGround)
                {
                    int frame = 0;
                    if (Velocity.Y > -0.2) frame = 1;
                    if (Velocity.Y > 0) frame = 2;
                    if (Velocity.Y > 0.2) frame = 3;
                    texture = Resources.Graphics.PlayerWalk[frame];
                }
                else if (DistanceWalked < 0.001)
                {
                    texture = Resources.Graphics.PlayerStand;
                }
                else
                {
                    int frame = ((int)Math.Round(DistanceWalked * 2)) % 8;
                    texture = Resources.Graphics.PlayerWalk[frame];
                }

            }
            else if (InvisibilityProgress < 5)
                texture = Resources.Graphics.PlayerFade[0];
            else if (InvisibilityProgress < 10)
                texture = Resources.Graphics.PlayerFade[1];
            else
                texture = Resources.Graphics.PlayerFade[2];

            sb.Draw(texture, rect, null, Color.LightGray, 0, Vector2.Zero,
                Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}
