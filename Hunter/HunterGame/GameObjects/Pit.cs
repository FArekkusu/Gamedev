using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public class Pit : RectangularObject
    {
        public readonly Geometry.Rectangle StaticRectangle;
        
        public override Geometry.Rectangle Rectangle => StaticRectangle;
        
        public Pit(Vector2 position, Vector2 dimensions, GraphicsDevice graphicsDevice, Color color) : base(position, dimensions, graphicsDevice, color)
        {
            StaticRectangle = base.Rectangle;
        }
    }
}