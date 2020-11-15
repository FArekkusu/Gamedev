using Geometry;

namespace ShooterCore
{
    public abstract class CircularObject : GameObject
    {
        public Circle Circle;
        public double Rotation;

        protected CircularObject(Circle circle, float rotation)
        {
            Circle = circle;
            Rotation = rotation;
        }
    }
}