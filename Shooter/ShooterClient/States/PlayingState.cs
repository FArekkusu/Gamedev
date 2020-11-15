using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShooterCore;

namespace ShooterClient
{
    public class PlayingState : GameState
    {
        public WorldState WorldState;
        
        public Renderer Renderer;

        public Texture2D BulletTexture;
        public Texture2D CharacterTexture;
        public Texture2D EnemyTexture;
        public Dictionary<BuffType, Texture2D> PickupTextures = new Dictionary<BuffType, Texture2D>();
        public Dictionary<(double, double), Texture2D> WallTextures = new Dictionary<(double, double), Texture2D>();

        public PlayingState(MyGame game, WorldState worldState) : base(game)
        {
            WorldState = worldState;
            
            Renderer = new Renderer(Game.SpriteBatch);
            
            LoadTextures();

            foreach (var wall in WorldState.Walls)
                GenerateWallTexture(wall);
        }

        public void LoadTextures()
        {
            BulletTexture = Game.Content.Load<Texture2D>("Bullet");
            CharacterTexture = Game.Content.Load<Texture2D>("Player");
            EnemyTexture = Game.Content.Load<Texture2D>("Enemy");

            PickupTextures[BuffType.CharacterSpeed] = Game.Content.Load<Texture2D>("BuffCharacterSpeed");
            PickupTextures[BuffType.CharacterDamage] = Game.Content.Load<Texture2D>("BuffCharacterDamage");
            PickupTextures[BuffType.CharacterHealth] = Game.Content.Load<Texture2D>("BuffCharacterHealth");
            PickupTextures[BuffType.BulletSpeed] = Game.Content.Load<Texture2D>("BuffBulletSpeed");
        }

        public override void Update()
        {
            if (!Game.IsActive)
            {
                Game.Client.Ping();

                return;
            }

            var ((x, y), isShooting, shotDirection) = GetInputs();

            var serializedInput = Serializer.SerializeInput((x, y), isShooting, shotDirection);

            Game.Client.SendInput(serializedInput);
        }

        public ((int, int), bool, double) GetInputs()
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            
            var x = keyboardState.IsKeyDown(Keys.A) ? -1 : keyboardState.IsKeyDown(Keys.D) ? 1 : 0;
            var y = keyboardState.IsKeyDown(Keys.W) ? -1 : keyboardState.IsKeyDown(Keys.S) ? 1 : 0;
            
            var isShooting = mouseState.LeftButton == ButtonState.Pressed;
            
            var circle = WorldState.Characters[Game.Client.ServerAssignedId].Circle;
            var shotDirection = Math.Atan2(mouseState.Y - circle.Y, mouseState.X - circle.X);

            return ((x, y), isShooting, shotDirection);
        }

        public override void Draw()
        {
            foreach (var bullet in WorldState.Bullets)
                Renderer.Draw(bullet, BulletTexture);
            
            for (var i = 0; i < WorldState.Characters.Count; i++)
                if (i == Game.Client.ServerAssignedId)
                    Renderer.DrawPlayableCharacter(WorldState.Characters[i], CharacterTexture);
                else
                    Renderer.Draw(WorldState.Characters[i], EnemyTexture);
            
            foreach (var (rectangle, buffType) in WorldState.Pickups)
                Renderer.Draw(rectangle.LeftX, rectangle.UpperY, buffType, PickupTextures);
            
            foreach (var wall in WorldState.Walls)
                Renderer.Draw(wall, WallTextures);
        }
        
        public void GenerateWallTexture(Wall wall)
        {
            var rectangle = wall.Rectangle;
            var w = (int)(rectangle.RightX - rectangle.LeftX);
            var h = (int)(rectangle.LowerY - rectangle.UpperY);
            var texture = new Texture2D(Game.GraphicsDevice, w, h);

            var data = new Color[w * h];
            for (var i = 0; i < data.Length; i++)
                data[i] = Color.Black;            
            
            texture.SetData(data);

            WallTextures[(w, h)] = texture;
        }
    }
}