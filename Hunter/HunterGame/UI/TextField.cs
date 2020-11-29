using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public class TextField
    {
        public readonly Vector2 Position;
        public readonly SpriteFont Font;
        public readonly string Content;

        public TextField(Vector2 position, SpriteFont font, string content)
        {
            Position = position;
            Font = font;
            Content = content;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Content, Position, Color.Black);
        }
    }
}