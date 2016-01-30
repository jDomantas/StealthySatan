using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StealthySatan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthySatan
{
    class Particle
    {
        private Rectangle Position;
        private Texture2D[] Animation;
        private int Duration, Timer;
        private bool NeedsFlip;
        public bool Removed { get { return Timer >= Duration; } }

        public Particle(Rectangle position, Texture2D[] animation, int duration, bool needsFlip) {
            Position = position;
            Animation = animation;
            Duration = duration;
            Timer = 0;
            NeedsFlip = needsFlip;
        }

        public void Update()
        {
            Timer++;
        }

        public void Draw(SpriteBatch sb)
        {
            if (Timer >= Duration)
                return;
            int frame = Timer*Animation.Length / Duration;
            sb.Draw(Animation[frame], Position, null, Color.White, 0,
                Vector2.Zero, NeedsFlip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

    }
}
