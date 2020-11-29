using Microsoft.Xna.Framework;

namespace HunterGame.GameStates
{
    public abstract class GameState
    {
        public readonly MyGame Game;

        protected GameState(MyGame game)
        {
            Game = game;
        }
        
        public virtual void Update(GameTime gameTime) {}
        
        public virtual void Draw() {}
    }
}