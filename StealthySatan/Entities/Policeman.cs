using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthySatan.Entities
{
    class Policeman : Entity
    {
        private enum Strategy { Check, Patrol, LookAround }
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
        public Policeman(Map map, Vector position, int x1, int x2) : this(map, position)
        {
            CurrentStrategy = Strategy.Patrol;
            if (x2 < x1)
            {
                int tmp = x2;
                x2 = x1;
                x1 = tmp;                    
            }
            PatrolXLeft = x1;
            PatrolXRight = x2;
        }

        public override void Update()
        {
           
        }
    }
}
