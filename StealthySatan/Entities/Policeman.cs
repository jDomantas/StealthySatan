﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace StealthySatan.Entities
{
    class Policeman : Entity
    {
        private enum Strategy { Check, Patrol, LookAround }

        private const double MoveSpeed = 0.1;
        private const double RunSpeed = 0.12;

        private Strategy CurrentStrategy, OriginalStrategy;
        private double PatrolXLeft, PatrolXRight;
        private double StartX;
        private double CheckX;
        private int LookTime, TimeSpentChecking, ShootingTimer, ForceGun;
        private Staircase GoalStaircase;
        private bool GunAlwaysUp, Locked;
        
        
        /// <summary>
        /// Create a static policeman that looks around
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        public Policeman(Map map, Vector position, bool gunAlwaysUp, bool lockLeft, bool lockRight) : base(map, 1.6, 2.3) // Look around
        {
            Position = position;
            StartX = position.X;
            CurrentStrategy = OriginalStrategy = Strategy.LookAround;
            Facing = Map.Random.Next(2) == 1 ? Direction.Left : Direction.Right;
            if (lockLeft)
            {
                Facing = Direction.Left;
                Locked = true;
            }
            else if (lockRight)
            {
                Facing = Direction.Right;
                Locked = true;
            }
            LookTime = Map.Random.Next(300);
            GoalStaircase = null;
            GunAlwaysUp = gunAlwaysUp;
        }

        /// <summary>
        /// Create a policeman that patrols between x1 and x2
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        public Policeman(Map map, Vector position, double x1, double x2, bool gunAlwaysUp, bool lockLeft, bool lockRight) : this(map, position, gunAlwaysUp, lockLeft, lockRight)
        {
            CurrentStrategy = OriginalStrategy = Strategy.Patrol;
            if (x2 < x1)
            {
                double tmp = x2;
                x2 = x1;
                x1 = tmp;                    
            }
            PatrolXLeft = x1;
            PatrolXRight = x2;
        }

        public override void Kill()
        {
            var rect = new Rectangle(
                   (int)Math.Round((Position.X - Width) * Map.ViewScale),
                   (int)Math.Round((Position.Y - Height * 0.59) * Map.ViewScale),
                   (int)Math.Round(Width * 2.7 * Map.ViewScale),
                   (int)Math.Round(Width * 2.7 / 600 * 512 * Map.ViewScale));

            Map.AddParticle(new Particle(rect, Resources.Graphics.CopDeath, 25, Facing == Direction.Left));

            base.Kill();
        }

        public override void Update()
        {
            if (ForceGun > 0) ForceGun--;

            if (CheckPlayerVisibility())
            {
                if ((GetCenter() - Map.PlayerEntity.GetCenter()).Length < 20)
                {
                    // in shooting position
                    if (GunAlwaysUp || ShootingTimer++ >= 10)
                    {
                        Resources.Audio.GunShot.CreateInstance().Play();
                        Map.PlayerEntity.Kill();
                        ForceGun = 45;
                    }
                }
                else
                {
                    CurrentStrategy = Strategy.Check;
                    CheckX = Map.PlayerEntity.GetCenter().X;
                    ShootingTimer = 0;
                    TimeSpentChecking = 0; 
                }
            }
            else
            {
                ShootingTimer = 0;
            }

            if (CurrentStrategy == Strategy.Check)
            {
                TimeSpentChecking++;
                if (TimeSpentChecking >= 500)
                    CurrentStrategy = OriginalStrategy;
                
                if (WalkTo(CheckX, RunSpeed))
                {
                    LookTime--;
                    if (LookTime <= 0)
                    {
                        LookTime = 200 + Map.Random.Next(100);
                        Facing = (Direction)(1 - (int)Facing);
                    }
                    DistanceWalked = 0;
                }
            }
            else if (GoalStaircase != null)
            {
                if (WalkTo(GoalStaircase.GetCenter().X, RunSpeed))
                {
                    Position = new Vector(
                        GoalStaircase.Other.Location.X + Staircase.Width / 2 - Width / 2,
                        GoalStaircase.Other.Location.Y + Staircase.Height - Height);

                    CurrentStrategy = OriginalStrategy = Strategy.LookAround;
                    StartX = GetCenter().X + Map.Random.NextDouble() * 4 - 2;
                    LookTime = 40 + Map.Random.Next(20);
                    GoalStaircase = null;
                }
            }
            else if (CurrentStrategy == Strategy.Patrol)
            {
                if (Facing == Direction.Left && WalkTo(PatrolXLeft, MoveSpeed))
                    Facing = Direction.Right;
                else if (Facing == Direction.Right && WalkTo(PatrolXRight, MoveSpeed))
                    Facing = Direction.Left;
            }
            else if (CurrentStrategy == Strategy.LookAround)
            {
                if (WalkTo(StartX, MoveSpeed))
                {
                    DistanceWalked = 0;
                    LookTime--;
                    if (LookTime <= 0)
                    {
                        LookTime = 200 + Map.Random.Next(100);
                        if(!Locked) Facing = (Direction)(1 - (int)Facing);
                    }
                }
            }

            MoveVertical(0.5);
        }

        private bool WalkTo(double x, double speed)
        {
            if (GetCenter().X < x - 0.2)
            {
                Facing = Direction.Right;
                MoveHorizontal(speed);
                DistanceWalked += speed;
                return false;
            }
            else if (GetCenter().X > x + 0.2)
            {
                Facing = Direction.Left;
                MoveHorizontal(-speed);
                DistanceWalked += speed;
                return false;
            }
            else
                return true;
        }

        public override void Draw(SpriteBatch sb)
        {
            var rect = new Rectangle(
                   (int)Math.Round((Position.X - Width) * Map.ViewScale),
                   (int)Math.Round((Position.Y - Height * 0.59) * Map.ViewScale),
                   (int)Math.Round(Width * 2.7 * Map.ViewScale),
                   (int)Math.Round(Width * 2.7 / 600 * 512 * Map.ViewScale));

            Texture2D[] frames;

            if (GunAlwaysUp) frames = Resources.Graphics.CopLight;
            else if (ForceGun > 0 || ShootingTimer > 7) frames = Resources.Graphics.CopGun;
            else if (ShootingTimer > 0) frames = Resources.Graphics.CopPrep;
            else frames = Resources.Graphics.Cop;
            
            Texture2D texture;
            if (DistanceWalked == 0) texture = frames[0];
            else texture = frames[((int)Math.Floor(DistanceWalked * 2)) % 6 + 1];

            sb.Draw(texture, rect, null, Color.White, 0, Vector2.Zero,
                Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            if (ForceGun == 45)
            {
                int index = 0;
                if (DistanceWalked != 0) index = ((int)Math.Floor(DistanceWalked * 2)) % 6 + 1;
                sb.Draw(Resources.Graphics.Muzzle1[index], rect, null, Color.White, 0, Vector2.Zero,
                Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            else if (ForceGun == 44)
            {
                int index = 0;
                if (DistanceWalked != 0) index = ((int)Math.Floor(DistanceWalked * 2)) % 6 + 1;
                sb.Draw(Resources.Graphics.Muzzle2[index], rect, null, Color.White, 0, Vector2.Zero,
                Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }

        public override void AllarmTriggered(Vector location)
        {
            if (CurrentStrategy != Strategy.Check && CanSeeLocation(location))
            {
                CurrentStrategy = Strategy.Check;
                TimeSpentChecking = 0;
                CheckX = location.X + Map.Random.NextDouble() * 2 - 1;
            }
        }

        public override void CallFromStaircase(Staircase s)
        {
            if(CurrentStrategy != Strategy.Check && CanSeeLocation(s.GetCenter()))
                GoalStaircase = s;
        }
    }
}
