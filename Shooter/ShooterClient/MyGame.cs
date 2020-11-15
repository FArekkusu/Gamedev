using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShooterClient
{
    public class MyGame : Game
    {
        public GraphicsDeviceManager GraphicsDeviceManager;
        public SpriteBatch SpriteBatch;

        public GameState State;
        public Client Client;

        public MyGame()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = @"Content\bin";
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            State = new ConnectingState(this);
            
            Client = new Client(this);

            IsMouseVisible = true;
        }

        protected override void Update(GameTime gameTime)
        {
            State.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            SpriteBatch.Begin();
            State.Draw();
            SpriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}