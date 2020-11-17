using Geometry;

namespace ShooterCore.Objects
{
    public abstract class RectangularObject : GameObject
    {
        public readonly Rectangle Rectangle;

        protected RectangularObject(Rectangle rectangle)
        {
            Rectangle = rectangle;
        }
    }
}