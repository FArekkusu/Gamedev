using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Network;
using ShooterClient.UI;
using ShooterCore;
using ShooterCore.Buffs;
using ShooterCore.Objects;

namespace ShooterClient.States
{
    public class PlayingState : GameState
    {
        public WorldState WorldState;
        
        public readonly Renderer Renderer;

        public Texture2D BulletTexture;
        public Texture2D CharacterTexture;
        public Texture2D EnemyTexture;
        public readonly Dictionary<BuffType, Texture2D> PickupTextures = new Dictionary<BuffType, Texture2D>();
        public readonly Dictionary<(double, double), Texture2D> WallTextures = new Dictionary<(double, double), Texture2D>();

        public SpriteFont CharacterStatsFont;
        public readonly TextField CharacterHealth;

        public int NextActionId;
        public readonly FiniteQueue Actions = new FiniteQueue();

        public PlayingState(MyGame game, WorldState worldState) : base(game)
        {
            WorldState = worldState;
            
            Renderer = new Renderer(Game.SpriteBatch);
            
            LoadTextures();
            
            CharacterHealth = new TextField(new Vector2(20, 420), CharacterStatsFont);

            foreach (var wall in WorldState.Walls)
                GenerateWallTexture(wall);
        }

        public void LoadTextures()
        {
            BulletTexture = Game.Content.Load<Texture2D>("Bullet");
            CharacterTexture = Game.Content.Load<Texture2D>("Player");
            EnemyTexture = Game.Content.Load<Texture2D>("EnemyBlank");

            PickupTextures[BuffType.CharacterSpeed] = Game.Content.Load<Texture2D>("BuffCharacterSpeed");
            PickupTextures[BuffType.CharacterDamage] = Game.Content.Load<Texture2D>("BuffCharacterDamage");
            PickupTextures[BuffType.CharacterHealth] = Game.Content.Load<Texture2D>("BuffCharacterHealth");
            PickupTextures[BuffType.BulletSpeed] = Game.Content.Load<Texture2D>("BuffBulletSpeed");

            CharacterStatsFont = Game.Content.Load<SpriteFont>("CharacterStatsFont");
        }

        public override void Update()
        {
            var action = GetInputs();

            var serializedInput = Serializer.SerializeInput(NextActionId++, action);

            Actions.Add(serializedInput);

            Game.Client.SendInput(Actions.Join());
        }

        public Action GetInputs()
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            
            var x = keyboardState.IsKeyDown(Keys.A) ? -1 : keyboardState.IsKeyDown(Keys.D) ? 1 : 0;
            var y = keyboardState.IsKeyDown(Keys.W) ? -1 : keyboardState.IsKeyDown(Keys.S) ? 1 : 0;
            
            var isShooting = mouseState.LeftButton == ButtonState.Pressed && Game.IsActive;

            return new Action(x, y, isShooting, mouseState.X, mouseState.Y);
        }

        public override void Draw()
        {
            foreach (var wall in WorldState.Walls)
                Renderer.Draw(wall, WallTextures);
            
            foreach (var (rectangle, buffType) in WorldState.Pickups)
                Renderer.Draw(rectangle.LeftX, rectangle.UpperY, buffType, PickupTextures);
            
            foreach (var bullet in WorldState.Bullets)
                Renderer.Draw(bullet, BulletTexture);
            
            for (var i = 0; i < WorldState.Characters.Count; i++)
                if (i == Game.Client.ServerAssignedId)
                    Renderer.DrawPlayableCharacter(WorldState.Characters[i], CharacterTexture);
                else
                    Renderer.Draw(WorldState.Characters[i], EnemyTexture);

            var hp = WorldState.Characters[Game.Client.ServerAssignedId].Hp;
            var color = hp > 70 ? Color.Green : hp > 40 ? Color.Orange : Color.Red;
            CharacterHealth.Draw(Renderer.SpriteBatch, "Health: " + hp, color);
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