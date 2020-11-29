using HunterGame.GameObjects.Bases;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame.GameObjects
{
    public class Pit : RectangularObject
    {
        public readonly Geometry.Rectangle CachedRectangle;
        
        public override Geometry.Rectangle Rectangle => CachedRectangle;
        
        public Pit(Vector2 position, Vector2 dimensions, GraphicsDevice graphicsDevice, Color color) : base(position, dimensions, graphicsDevice, color)
        {
            CachedRectangle = base.Rectangle;
        }
    }
}