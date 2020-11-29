using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HunterGame.UI
{
    public class Button
    {
        public readonly Vector2 Position;
        public readonly SpriteFont Font;
        public readonly string Content;
        public event EventHandler Click;
        
        public bool IsHovering;
        public MouseState PreviousMouseState;

        public Button(Vector2 position, SpriteFont font, string content, EventHandler click)
        {
            Position = position;
            Font = font;
            Content = content;
            Click = click;
        }
        
        public void Update()
        {
            IsHovering = false;
            
            var mouseState = Mouse.GetState();
            var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            var (w, h) = Font.MeasureString(Content);
            var textRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)w, (int)h);

            if (mouseRectangle.Intersects(textRectangle))
            {
                IsHovering = true;
                if (PreviousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                    Click?.Invoke(this, EventArgs.Empty);
            }

            PreviousMouseState = mouseState;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            var color = IsHovering ? Color.DarkViolet : Color.IndianRed;
            spriteBatch.DrawString(Font, Content, Position, color);
        }
    }
}