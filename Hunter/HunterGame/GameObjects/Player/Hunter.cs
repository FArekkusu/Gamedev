using System;
using System.Collections.Generic;
using HunterGame.GameObjects.Bases;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HunterGame.GameObjects.Player
{
    public class Hunter : Creature
    {
        public const double RotationCorrection = Math.PI / 2;
        public const double BaseCooldown = 0.5;
        public const double LinearVelocity = 150;
        
        public double Cooldown;

        public readonly Texture2D BulletTexture;
        
        public override Geometry.Circle Circle => new Geometry.Circle((Position.X, Position.Y), Texture.Width / 2.0);

        public Hunter(Texture2D texture, Vector2 position, Texture2D bulletTexture) : base(texture, position)
        {
            BulletTexture = bulletTexture;
        }
        
        public static double GetRotation(Vector2 centerOffset)
        {
            var mouseState = Mouse.GetState();
            
            var (x, y) = new Vector2(mouseState.X - centerOffset.X, mouseState.Y - centerOffset.Y);
            
            return Math.Atan2(y, x);
        }
        
        public void Update(GameTime gameTime, Vector2 centerOffset, List<Bullet> bullets)
        {
            var elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;
            
            if (Cooldown > 0)
                Cooldown -= elapsedTime;

            Move(elapsedTime);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                TryShoot(centerOffset, bullets);
        }

        public void Move(double elapsedTime)
        {
            var keyboardState = Keyboard.GetState();

            var x = keyboardState.IsKeyDown(Keys.A) ? -1 : keyboardState.IsKeyDown(Keys.D) ? 1 : 0;
            var y = keyboardState.IsKeyDown(Keys.W) ? -1 : keyboardState.IsKeyDown(Keys.S) ? 1 : 0;

            var (dx, dy) = Geometry.Utils.Normalize((x, y));
            
            Position.X += (float)(dx * LinearVelocity * elapsedTime);
            Position.Y += (float)(dy * LinearVelocity * elapsedTime);
        }

        public void TryShoot(Vector2 centerOffset, List<Bullet> bullets)
        {
            if (Cooldown > 0)
                return;

            Cooldown = BaseCooldown;

            bullets.Add(CreateBulletOnHunter(centerOffset));
        }

        public Bullet CreateBulletOnHunter(Vector2 centerOffset)
        {
            var rotation = GetRotation(centerOffset);

            var bulletOffset = new Vector2(BulletTexture.Width / 2f);
            
            return new Bullet(BulletTexture, Position - bulletOffset, rotation);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 centerOffset)
        {
            var origin = new Vector2(Texture.Width / 2f);
            
            var rotation = (float)(GetRotation(centerOffset) + RotationCorrection);

            spriteBatch.Draw(Texture, Position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }
    }
}