namespace ShooterClient.States
{
    public abstract class GameState
    {
        public readonly MyGame Game;

        protected GameState(MyGame game)
        {
            Game = game;
        }
        
        public virtual void Update() {}
        
        public virtual void Draw() {}
    }
}