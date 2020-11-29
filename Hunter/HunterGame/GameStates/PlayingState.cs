using System;
using System.Collections.Generic;
using HunterGame.GameObjects;
using HunterGame.GameObjects.Animals;
using HunterGame.GameObjects.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HunterGame.GameStates
{
    public class PlayingState : GameState
    {
        public readonly WorldState WorldState;

        public readonly Vector2 CenterOffset;
        public readonly Camera Camera;

        public PlayingState(MyGame game, int hareCount, int doeGroupsCount, int wolfCount) : base(game)
        {
            WorldState = new WorldState(LoadTextures(), Game.GraphicsDevice, Game.Random);
            
            WorldState.AddHunter();
            WorldState.AddInitialAnimals(hareCount, doeGroupsCount, wolfCount);
            WorldState.AddBorders();
            
            CenterOffset = new Vector2(Game.GraphicsDevice.Viewport.Width / 2f, Game.GraphicsDevice.Viewport.Height / 2f);
            Camera = new Camera();
        }

        public Dictionary<Type, Texture2D> LoadTextures()
        {
            return new Dictionary<Type, Texture2D>
            {
                {typeof(Hunter), Game.Content.Load<Texture2D>("Hunter")},
                {typeof(Hare), Game.Content.Load<Texture2D>("Hare")},
                {typeof(Doe), Game.Content.Load<Texture2D>("Doe")},
                {typeof(Wolf), Game.Content.Load<Texture2D>("Wolf")},
                {typeof(Bullet), Game.Content.Load<Texture2D>("Bullet")},
            };
        }
        
        public override void Update(GameTime gameTime)
        {
            Camera.Update();
            
            WorldState.Update(gameTime, CenterOffset);
            
            if (!WorldState.Hunter.IsAlive || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Game.State = new MenuState(Game);
            
            Camera.Follow(WorldState.Hunter.Position, CenterOffset);
        }

        public override void Draw()
        {
            Game.GraphicsDevice.Clear(Color.Chartreuse);
            
            Game.SpriteBatch.Begin(transformMatrix: Camera.Transform);

            WorldState.Draw(Game.SpriteBatch, CenterOffset);

            Game.SpriteBatch.End();
        }
    }
}