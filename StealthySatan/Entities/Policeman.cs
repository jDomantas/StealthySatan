using System;
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

        private Strategy CurrentStrategy, OriginalStrategy;
        private double PatrolXLeft, PatrolXRight;
        private double StartX;
        private double CheckX;
        private int LookTime, TimeSpentChecking;
        private Staircase GoalStaircase;



        /// <summary>
        /// Create a static policeman that looks around
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        public Policeman(Map map, Vector position) : base(map, 1.6, 2.3) // Look around
        {
            Position = position;
            StartX = position.X;
            CurrentStrategy = OriginalStrategy = Strategy.LookAround;
            Facing = Map.Random.Next(2) == 1 ? Direction.Left : Direction.Right;
            LookTime = Map.Random.Next(300);
            GoalStaircase = null;
        }

        /// <summary>
        /// Create a policeman that patrols between x1 and x2
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        public Policeman(Map map, Vector position, double x1, double x2) : this(map, position)
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

        public override void Update()
        {
            if (CheckPlayerVisibility())
            {
                // shoot player instead
                CurrentStrategy = Strategy.Check;
                TimeSpentChecking = 0;
                CheckX = Map.PlayerEntity.Position.X + Map.Random.NextDouble() * 2 - 1;
            }

            if (CurrentStrategy != Strategy.Check && GoalStaircase != null)
            {
                if (GetCenter().X < GoalStaircase.GetCenter().X - 0.5)
                {
                    MoveHorizontal(MoveSpeed * 1.2);
                    DistanceWalked += MoveSpeed * 1.2;
                    Facing = Direction.Right;
                }
                else if (GetCenter().X > GoalStaircase.GetCenter().X + 0.5)
                {
                    MoveHorizontal(-MoveSpeed * 1.2);
                    DistanceWalked += MoveSpeed * 1.2;
                    Facing = Direction.Left;
                }
                else
                {
                    Position = new Vector(
                        GoalStaircase.Other.Location.X + Staircase.Width / 2 - Width / 2,
                        GoalStaircase.Other.Location.Y + Staircase.Height - Height);

                    CurrentStrategy = OriginalStrategy = Strategy.LookAround;
                    StartX = GetCenter().X + Map.Random.NextDouble() * 8 - 4;
                    LookTime = 40 + Map.Random.Next(20);
                    GoalStaircase = null;
                }
            }
            else if (CurrentStrategy == Strategy.Patrol)
            {
                if (Facing == Direction.Left)
                {
                    DistanceWalked += MoveSpeed * 0.7;
                    if (Position.X <= PatrolXLeft || MoveHorizontal(-MoveSpeed * 0.7))
                        Facing = Direction.Right;
                }
                else
                {
                    DistanceWalked += MoveSpeed * 0.7;
                    if (Position.X >= PatrolXRight || MoveHorizontal(MoveSpeed * 0.7))
                        Facing = Direction.Left;
                }
            }
            else if (CurrentStrategy == Strategy.LookAround)
            {
                if (Position.X < StartX - 0.5)
                {
                    Facing = Direction.Right;
                    MoveHorizontal(MoveSpeed);
                    DistanceWalked += MoveSpeed;
                }
                else if (Position.X > StartX + 0.5)
                {
                    Facing = Direction.Left;
                    MoveHorizontal(-MoveSpeed);
                    DistanceWalked += MoveSpeed;
                }
                else
                {
                    DistanceWalked = 0;
                    LookTime--;
                    if (LookTime <= 0)
                    {
                        LookTime = 200 + Map.Random.Next(100);
                        Facing = Facing == Direction.Left ? Direction.Right : Direction.Left;
                    }
                }
            }
            else
            {
                if(Math.Abs(CheckX - Position.X) <= 1.5)
                    TimeSpentChecking++;

                if (TimeSpentChecking >= 400 + Map.Random.Next(400))
                {
                    CurrentStrategy = OriginalStrategy;
                    DistanceWalked = 0;
                }
                else
                {
                    if (Math.Abs(CheckX - Position.X) <= 0.5)
                    {
                        LookTime--;
                        if (LookTime <= 0)
                        {
                            LookTime = 200 + Map.Random.Next(100);
                            Facing = Facing == Direction.Left ? Direction.Right : Direction.Left;
                        }
                        DistanceWalked = 0;
                    }
                    else
                    {
                        int s = Math.Sign(CheckX - Position.X);
                        if (s == -1)
                            Facing = Direction.Left;
                        else
                            Facing = Direction.Right;
                        MoveHorizontal(s * MoveSpeed * 1.5);
                        DistanceWalked += MoveSpeed * 1.5;
                        LookTime = 40 + Map.Random.Next(20);
                    }
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
            
            sb.Draw(
                DistanceWalked < 0.001 ? Resources.Graphics.ManStand : Resources.Graphics.ManWalk[(int)Math.Floor(DistanceWalked * 2) % 8],
                rect, null, Color.White, 0, Vector2.Zero, Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
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
