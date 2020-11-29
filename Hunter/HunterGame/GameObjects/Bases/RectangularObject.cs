using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame.GameObjects.Bases
{
    public abstract class RectangularObject : GameObject
    {
        public readonly Texture2D Texture;
        public Vector2 Position;
        
        public virtual Geometry.Rectangle Rectangle => new Geometry.Rectangle((Position.X, Position.Y + Texture.Height), (Position.X + Texture.Width, Position.Y));
        
        protected RectangularObject(Vector2 position, Vector2 dimensions, GraphicsDevice graphicsDevice, Color color)
        {
            Texture = GenerateTexture(graphicsDevice, (int)dimensions.X, (int)dimensions.Y, color);
            Position = position;
        }

        public static Texture2D GenerateTexture(GraphicsDevice graphicsDevice, int w, int h, Color color)
        {
            var texture = new Texture2D(graphicsDevice, w, h);

            var data = new Color[w * h];
            for (var i = 0; i < data.Length; i++)
                data[i] = color;
            
            texture.SetData(data);
            
            return texture;
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}