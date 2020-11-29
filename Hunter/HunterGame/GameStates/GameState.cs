using Microsoft.Xna.Framework;

namespace HunterGame
{
    public abstract class GameState
    {
        public MyGame Game;

        protected GameState(MyGame game)
        {
            Game = game;
        }
        
        public virtual void Update(GameTime gameTime) {}
        
        public virtual void Draw() {}
    }
}