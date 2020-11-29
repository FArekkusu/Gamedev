using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public class Bullet : CircularObject
    {
        public const double LinearVelocity = 300;

        public double Timer = 1;
        public readonly double Direction;

        public Bullet(Texture2D texture, Vector2 position, double direction) : base(texture, position)
        {
            Direction = direction;
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedTime = Math.Min(Timer, gameTime.ElapsedGameTime.TotalSeconds);
            
            var dx = Math.Cos(Direction);
            var dy = Math.Sin(Direction);

            Position.X += (float)(dx * LinearVelocity * elapsedTime);
            Position.Y += (float)(dy * LinearVelocity * elapsedTime);
            
            Timer -= elapsedTime;

            if (Timer <= 0)
                IsAlive = false;
        }
    }
}