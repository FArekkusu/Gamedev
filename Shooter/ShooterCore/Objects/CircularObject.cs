using Geometry;

namespace ShooterCore.Objects
{
    public abstract class CircularObject : GameObject
    {
        public readonly Circle Circle;

        protected CircularObject(Circle circle)
        {
            Circle = circle;
        }
    }
}