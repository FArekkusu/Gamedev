using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame.GameObjects.Bases
{
    public abstract class GameObject
    {
        public bool IsAlive = true;
        
        public virtual void Update(GameTime gameTime) {}
        
        public virtual void Draw(SpriteBatch spriteBatch) {}
    }
}