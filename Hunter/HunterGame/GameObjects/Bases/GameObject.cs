using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public abstract class GameObject
    {
        public bool IsAlive = true;
        
        public virtual void Update(GameTime gameTime) {}
        
        public virtual void Draw(SpriteBatch spriteBatch) {}
    }
}