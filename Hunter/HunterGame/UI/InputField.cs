using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HunterGame.UI
{
    public class InputField
    {
        public const int VPadding = 5;
        public const int BorderWidth = 2;
        
        public readonly Vector2 Position;
        public readonly Texture2D Texture;
        public bool IsFocused;
        
        public readonly Vector2 TextPosition;
        public readonly SpriteFont Font;
        public readonly int MaxLength;
        public readonly string DefaultValue;
        public string Content;

        public KeyboardState PreviousKeyboardState;
        public MouseState PreviousMouseState;
        
        public readonly Dictionary<Keys, string> Symbols = new Dictionary<Keys, string>
        {
            {Keys.D0, "0"}, {Keys.D1, "1"}, {Keys.D2, "2"}, {Keys.D3, "3"}, {Keys.D4, "4"},
            {Keys.D5, "5"}, {Keys.D6, "6"}, {Keys.D7, "7"}, {Keys.D8, "8"}, {Keys.D9, "9"},
            {Keys.NumPad0, "0"}, {Keys.NumPad1, "1"}, {Keys.NumPad2, "2"}, {Keys.NumPad3, "3"}, {Keys.NumPad4, "4"},
            {Keys.NumPad5, "5"}, {Keys.NumPad6, "6"}, {Keys.NumPad7, "7"}, {Keys.NumPad8, "8"}, {Keys.NumPad9, "9"}
        };

        public InputField(Vector2 position, SpriteFont font, GraphicsDevice graphicsDevice, int maxLength, string defaultValue = "", string content = null)
        {
            var (x, y) = position;
            
            TextPosition = new Vector2(x + VPadding, y);
            Font = font;
            MaxLength = maxLength;
            DefaultValue = defaultValue.Substring(0, Math.Min(defaultValue.Length, maxLength));
            Content = content ?? DefaultValue;

            Position = position;
            Texture = GenerateBox(graphicsDevice);
            UpdateBoxColor();
        }

        public void UpdateBoxColor()
        {
            var w = Texture.Width;
            var h = Texture.Height;

            var data = new Color[w * h];
            for (var i = 0; i < data.Length; i++)
                data[i] = i < w * BorderWidth || i >= w * (h - BorderWidth) || i % w < BorderWidth || i % w >= w - BorderWidth ? Color.Black : IsFocused ? Color.White : Color.LightGray;

            Texture.SetData(data);
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
            
            var textRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

            if (PreviousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                IsFocused = mouseRectangle.Intersects(textRectangle);

                if (!IsFocused && Content == "")
                    Content = DefaultValue;
                
                UpdateBoxColor();
            }
            else if (IsFocused)
            {
                var keyboardState = Keyboard.GetState();

                UpdateContent(keyboardState);

                PreviousKeyboardState = keyboardState;
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
            spriteBatch.Draw(Texture, Position, Color.White);
            spriteBatch.DrawString(Font, Content, TextPosition, Color.Black);
        }
    }
}