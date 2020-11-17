using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShooterClient.UI
{
    public class TextField
    {
        public readonly Vector2 Position;
        public readonly SpriteFont Font;

        public TextField(Vector2 position, SpriteFont font)
        {
            Position = position;
            Font = font;
        }

        public void Draw(SpriteBatch spriteBatch, string content, Color color)
        {
            spriteBatch.DrawString(Font, content, Position, color);
        }
    }
}