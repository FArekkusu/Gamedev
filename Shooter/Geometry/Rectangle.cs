namespace Geometry
{
    public class Rectangle
    {
        public readonly double LeftX;
        public readonly double LowerY;

        public readonly double RightX;
        public readonly double UpperY;

        public Rectangle((double, double) lowerLeft, (double, double) upperRight)
        {
            (LeftX, LowerY) = lowerLeft;
            (RightX, UpperY) = upperRight;
        }
    }
}