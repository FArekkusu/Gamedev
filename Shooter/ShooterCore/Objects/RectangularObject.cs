using Geometry;

namespace ShooterCore
{
    public abstract class RectangularObject : GameObject
    {
        public Rectangle Rectangle;

        protected RectangularObject(Rectangle rectangle)
        {
            Rectangle = rectangle;
        }
    }
}