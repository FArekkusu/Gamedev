using System;
using HunterGame.GameObjects.Bases;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame.GameObjects.Player
{
    public class Bullet : CircularObject
    {
        public const double LinearVelocity = 300;

        public double Timer = 2;
        public readonly Vector2 Direction;

        public Bullet(Texture2D texture, Vector2 position, double direction) : base(texture, position)
        {
            Direction = new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction));
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedTime = Math.Min(Timer, gameTime.ElapsedGameTime.TotalSeconds);

            Position += Direction * (float)(LinearVelocity * elapsedTime);

            Timer -= elapsedTime;

            if (Timer <= 0)
                IsAlive = false;
        }
    }
}