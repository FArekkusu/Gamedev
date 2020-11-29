using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public abstract class CircularObject : GameObject
    {
        public readonly Texture2D Texture;
        public Vector2 Position;

        public virtual Geometry.Circle Circle
        {
            get
            {
                var radius = Texture.Width / 2.0;
                
                return new Geometry.Circle((Position.X + radius, Position.Y + radius), radius);
            }
        }

        protected CircularObject(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}