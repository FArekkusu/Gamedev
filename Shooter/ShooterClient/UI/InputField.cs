using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ShooterClient
{
    public class InputField
    {
        public Vector2 TextPosition;
        public SpriteFont Font;
        public int MaxLength;
        public string Content;

        public const int VPadding = 5;
        private const int BorderWidth = 2;
        
        public Vector2 BoxPosition;
        public Texture2D BoxTexture;
        public bool IsFocused;
        
        public KeyboardState PreviousKeyboardState;
        public MouseState PreviousMouseState;
        
        public Dictionary<Keys, string> Symbols = new Dictionary<Keys, string>
        {
            {Keys.D0, "0"}, {Keys.D1, "1"}, {Keys.D2, "2"}, {Keys.D3, "3"}, {Keys.D4, "4"},
            {Keys.D5, "5"}, {Keys.D6, "6"}, {Keys.D7, "7"}, {Keys.D8, "8"}, {Keys.D9, "9"},
            {Keys.OemPeriod, "."}
        };

        public InputField(Vector2 boxPosition, SpriteFont font, GraphicsDevice graphicsDevice, int maxLength, string content = "127.0.0.1")
        {
            var (x, y) = boxPosition;
            
            TextPosition = new Vector2(x + VPadding, y);
            Font = font;
            MaxLength = maxLength;
            Content = content;

            BoxPosition = boxPosition;
            BoxTexture = GenerateBox(graphicsDevice);
            UpdateBoxColor();
        }

        public void UpdateBoxColor()
        {
            var w = BoxTexture.Width;
            var h = BoxTexture.Height;

            var data = new Color[w * h];
            for (var i = 0; i < data.Length; i++)
                data[i] = i < w * BorderWidth || i >= w * (h - BorderWidth) || i % w < BorderWidth || i % w >= w - BorderWidth ? Color.Black : IsFocused ? Color.White : Color.LightGray;

            BoxTexture.SetData(data);
        }
        
        public Texture2D GenerateBox(GraphicsDevice graphicsDevice)
        {
            var (w, h) = Font.MeasureString(new string('0', MaxLength));
            return new Texture2D(graphicsDevice, (int)w + VPadding * 2, (int)h);
        }
        
        public void Update()
        {
            var mouseState = Mouse.GetState();
            var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            
            var textRectangle = new Rectangle((int)BoxPosition.X, (int)BoxPosition.Y, BoxTexture.Width, BoxTexture.Height);

            if (PreviousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                IsFocused = mouseRectangle.Intersects(textRectangle);
                
                UpdateBoxColor();
            }
            else
            {
                if (IsFocused)
                {
                    var keyboardState = Keyboard.GetState();

                    UpdateContent(keyboardState);

                    PreviousKeyboardState = keyboardState;
                }
            }

            PreviousMouseState = mouseState;
        }
        
        public void UpdateContent(KeyboardState keyboardState)
        {
            if (KeyReleased(keyboardState, Keys.Back) && Content.Length > 0)
                Content = Content.Substring(0, Content.Length - 1);
            else
                foreach (var (k, v) in Symbols)
                    if (KeyReleased(keyboardState, k) && Content.Length < MaxLength)
                    {
                        Content += v;
                        break;
                    }
        }
        
        public bool KeyReleased(KeyboardState keyboardState, Keys key)
        {
            return PreviousKeyboardState.IsKeyDown(key) && keyboardState.IsKeyUp(key);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BoxTexture, BoxPosition, Color.White);
            spriteBatch.DrawString(Font, Content, TextPosition, Color.Black);
        }
    }
}