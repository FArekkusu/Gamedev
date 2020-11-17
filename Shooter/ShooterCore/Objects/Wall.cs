using Geometry;

namespace ShooterCore.Objects
{
    public class Wall : RectangularObject
    {
        public Wall((double, double) position, double width, double height) : base(new Rectangle((position.Item1, position.Item2 + height), (position.Item1 + width, position.Item2))) {}
    }
}