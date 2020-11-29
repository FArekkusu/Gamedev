using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HunterGame
{
    public class Camera
    {
        public const int MinZoom = 25;
        public const int MaxZoom = 100;
        public const int DefaultZoom = 75;
        public const int ZoomStep = 1;
        
        public int ZoomLevel = DefaultZoom;
        public Matrix Zoom = Matrix.CreateScale(DefaultZoom / 100f, DefaultZoom / 100f, 0);
        public Matrix Transform;

        public void Update()
        {
            var keyboardState = Keyboard.GetState();
            
            if (keyboardState.IsKeyDown(Keys.Q))
                UpdateZoom(ZoomStep);
            else if (keyboardState.IsKeyDown(Keys.E))
                UpdateZoom(-ZoomStep);
        }

        public void UpdateZoom(int change)
        {
            ZoomLevel = Math.Max(MinZoom, Math.Min(MaxZoom, ZoomLevel + change));
            Zoom = Matrix.CreateScale(ZoomLevel / 100f, ZoomLevel / 100f, 0);
        }

        public void Follow(Vector2 target, Vector2 centerOffset)
        {
            var (x, y) = target;
            var position = Matrix.CreateTranslation(-x, -y, 0);

            (x, y) = centerOffset;
            var offset = Matrix.CreateTranslation(x, y, 0);

            Transform = position * Zoom * offset;
        }
    }
}