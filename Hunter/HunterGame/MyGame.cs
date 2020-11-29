using System;
using HunterGame.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public class MyGame : Game
    {
        public readonly GraphicsDeviceManager GraphicsDeviceManager;
        public SpriteBatch SpriteBatch;

        public GameState State;
        public readonly Random Random = new Random();
        
        public MyGame()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = @"Content\bin";
        }
        
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            State = new MenuState(this);

            IsMouseVisible = true;
        }
        
        protected override void Update(GameTime gameTime)
        {
            State.Update(gameTime);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            State.Draw();
            
            base.Draw(gameTime);
        }
    }
}