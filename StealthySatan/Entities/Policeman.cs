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

        private Strategy CurrentStrategy;
        private double PatrolXLeft, PatrolXRight;
        private double CheckX;
        private int LookTime;



        /// <summary>
        /// Create a static policeman that looks around
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        public Policeman(Map map, Vector position) : base(map, 1.6, 2.3) // Look around
        {
            Position = position;
            CurrentStrategy = Strategy.LookAround;
            Facing = Map.Random.Next(2) == 1 ? Direction.Left : Direction.Right;
            LookTime = Map.Random.Next(300);
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
            CurrentStrategy = Strategy.Patrol;
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
            if (CurrentStrategy == Strategy.Patrol)
            {
                if (Facing == Direction.Left)
                {
                    if (Position.X <= PatrolXLeft || MoveHorizontal(-MoveSpeed*0.7))
                    {
                        Facing = Direction.Right;
                    }
                }
                else
                {
                    if (Position.X >= PatrolXRight || MoveHorizontal(MoveSpeed*0.7))
                    {
                        Facing = Direction.Left;
                    }
                }
            }
            else if (CurrentStrategy == Strategy.LookAround)
            {
                LookTime--;
                if (LookTime <= 0)
                {
                    LookTime = 200 + Map.Random.Next(100);
                    Facing = Facing == Direction.Left ? Direction.Right : Direction.Left;
                }

            }
            else
            {
                int s = Math.Sign(CheckX - Position.X);
                if (s == -1)
                    Facing = Direction.Left;
                else
                    Facing = Direction.Right;
                MoveHorizontal(s * MoveSpeed * 1.5);
                if (Math.Abs(CheckX - Position.X) <= 0.5)
                {
                    CurrentStrategy = Strategy.LookAround;
                    LookTime = 40 + Map.Random.Next(20);
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Resources.Graphics.Pixel, GetScreenBounds(), Facing == Direction.Left ? Color.Blue : Color.Green);
        }
    }
}
