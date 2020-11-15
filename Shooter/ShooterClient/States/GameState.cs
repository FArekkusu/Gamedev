namespace ShooterClient
{
    public abstract class GameState
    {
        public MyGame Game;

        public GameState(MyGame game)
        {
            Game = game;
        }
        
        public virtual void Update() {}
        
        public virtual void Draw() {}
    }
}