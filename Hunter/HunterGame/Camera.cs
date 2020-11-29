using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HunterGame
{
    public class Camera
    {
        public const int MinZoom = 25;
        public const int MaxZoom = 100;
        public const int DefaultZoom = 70;
        public const int ZoomStep = 1;
        
        public int ZoomLevel = DefaultZoom;
        public Matrix Zoom = CreateZoomMatrix(DefaultZoom);
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
            Zoom = CreateZoomMatrix(ZoomLevel);
        }

        public static Matrix CreateZoomMatrix(int zoomLevel)
        {
            return Matrix.CreateScale(zoomLevel / 100f, zoomLevel / 100f, 0);
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