namespace Geometry
{
    public class Circle
    {
        public readonly double X;
        public readonly double Y;

        public readonly double Radius;

        public Circle((double, double) position, double radius)
        {
            (X, Y) = position;
            Radius = radius;
        }
    }
}