using Geometry;

namespace ShooterCore
{
    public class Pickup : RectangularObject
    {
        public Buff Buff;

        public Pickup((double, double) position, double width, double height, Buff buff) : base(new Rectangle((position.Item1, position.Item2 + height), (position.Item1 + width, position.Item2)))
        {
            Buff = buff;
        }
    }
}