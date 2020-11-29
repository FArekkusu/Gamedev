using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public class MenuState : GameState
    {
        public readonly List<InputField> InputFields = new List<InputField>();
        public readonly List<TextField> TextFields = new List<TextField>();

        public readonly Button StartButton;

        public MenuState(MyGame game) : base(game)
        {
            var font = Game.Content.Load<SpriteFont>("MenuFont");

            AddTextFields(10, 10, 50, new [] {"Hares:", "Doe groups:", "Wolves:"}, font);
            AddInputFields(200, 10, 50, 3, font);
            AddTextFields(10, 320, 0, new [] {"Controls:\n  W/A/S/D - movement\n  Q/E - zoom in/out\n  LMB - shoot"}, font);

            StartButton = new Button(new Vector2(10, 200), font, "Start game", (o, args) =>
            {
                var hareCount = int.Parse(InputFields[0].Content);
                var doeGroupsCount = int.Parse(InputFields[1].Content);
                var wolfCount = int.Parse(InputFields[2].Content);
                
                Game.State = new PlayingState(Game, hareCount, doeGroupsCount, wolfCount);
            });
        }

        public void AddTextFields(int x, int y, int dy, IEnumerable<string> contents, SpriteFont font)
        {
            foreach (var content in contents)
            {
                var textField = new TextField(new Vector2(x, y), font, content);
                
                TextFields.Add(textField);
                
                y += dy;
            }
        }

        public void AddInputFields(int x, int y, int dy, int count, SpriteFont font)
        {
            for (var i = 0; i < count; i++)
            {
                var inputField = new InputField(new Vector2(x, y), font, Game.GraphicsDevice, 1, "0", "1");
                
                InputFields.Add(inputField);
                
                y += dy;
            }
                
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var inputField in InputFields)
                inputField.Update();

            StartButton.Update();
        }

        public override void Draw()
        {
            Game.GraphicsDevice.Clear(Color.WhiteSmoke);
            
            Game.SpriteBatch.Begin();

            foreach (var textField in TextFields)
                textField.Draw(Game.SpriteBatch);
            
            foreach (var inputField in InputFields)
                inputField.Draw(Game.SpriteBatch);

            StartButton.Draw(Game.SpriteBatch);
            
            Game.SpriteBatch.End();
        }
    }
}