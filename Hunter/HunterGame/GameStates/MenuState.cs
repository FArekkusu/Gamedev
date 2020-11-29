using System.Collections.Generic;
using HunterGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame.GameStates
{
    public class MenuState : GameState
    {
        public readonly Dictionary<string, InputField> InputFields = new Dictionary<string, InputField>();
        public readonly List<TextField> TextFields = new List<TextField>();

        public readonly Button StartButton;

        public MenuState(MyGame game) : base(game)
        {
            var font = Game.Content.Load<SpriteFont>("MenuFont");

            AddTextFields(10, 10, 50, new [] {"Hares:", "Doe groups:", "Wolves:"}, font);
            AddInputFields(200, 10, 50, new [] {"hares", "doe groups", "wolves"}, font);
            AddTextFields(10, 320, 0, new [] {"Controls:\n  LMB - shoot\n  W/A/S/D - movement\n  Q/E - zoom in/out\n  Esc - return to menu"}, font);

            StartButton = new Button(new Vector2(10, 200), font, "Start game", (o, args) =>
            {
                var hareCount = int.Parse(InputFields["hares"].Content);
                var doeGroupsCount = int.Parse(InputFields["doe groups"].Content);
                var wolfCount = int.Parse(InputFields["wolves"].Content);
                
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

        public void AddInputFields(int x, int y, int dy, IEnumerable<string> names, SpriteFont font)
        {
            foreach (var name in names)
            {
                var inputField = new InputField(new Vector2(x, y), font, Game.GraphicsDevice, 1, "0", "1");
                
                InputFields[name] = inputField;
                
                y += dy;
            }
                
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var inputField in InputFields.Values)
                inputField.Update();

            StartButton.Update();
        }

        public override void Draw()
        {
            Game.GraphicsDevice.Clear(Color.WhiteSmoke);
            
            Game.SpriteBatch.Begin();

            foreach (var textField in TextFields)
                textField.Draw(Game.SpriteBatch);
            
            foreach (var inputField in InputFields.Values)
                inputField.Draw(Game.SpriteBatch);

            StartButton.Draw(Game.SpriteBatch);
            
            Game.SpriteBatch.End();
        }
    }
}