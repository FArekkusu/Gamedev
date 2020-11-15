using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShooterCore;

namespace ShooterClient
{
    public class Renderer
    {
        public SpriteBatch SpriteBatch;

        public Renderer(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch;
        }

        public void DrawPlayableCharacter(Character character, Texture2D texture)
        {
            var mouseState = Mouse.GetState();
                    
            var circle = character.Circle;
            var position = new Vector2((float)circle.X, (float)circle.Y);
            var origin = new Vector2((float)circle.Radius);

            var rotation = (float)(Math.Atan2(mouseState.Y - position.Y, mouseState.X - position.X) + Directions.Down);

            SpriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }

        public void Draw(CircularObject circularObject, Texture2D texture)
        {
            if (!circularObject.IsAlive)
                return;
            
            var circle = circularObject.Circle;
            var position = new Vector2((float)(circle.X - circle.Radius), (float)(circle.Y - circle.Radius));

            SpriteBatch.Draw(texture, position, Color.White);
        }
        
        public void Draw(double x, double y, BuffType buffType, Dictionary<BuffType, Texture2D> pickupTextures)
        {
            var position = new Vector2((float)x, (float)y);
                    
            SpriteBatch.Draw(pickupTextures[buffType], position, Color.White);
        }

        public void Draw(Wall wall, Dictionary<(double, double), Texture2D> wallTextures)
        {
            var rectangle = wall.Rectangle;
            var position = new Vector2((float)rectangle.LeftX, (float)rectangle.UpperY);
                
            var w = rectangle.RightX - rectangle.LeftX;
            var h = rectangle.LowerY - rectangle.UpperY;
                
            SpriteBatch.Draw(wallTextures[(w, h)], position, Color.White);
        }
    }
}