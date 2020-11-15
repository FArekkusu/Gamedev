using System;

namespace Geometry
{
    public static class Utils
    {
        public static double Hypot(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static bool Between(double value, double lower, double upper)
        {
            return lower <= value && value <= upper;
        }

        public static double Clamp(double value, double lower, double upper)
        {
            return value < lower ? lower : value > upper ? upper : value;
        }

        public static (double, double) Normalize((double, double) vector)
        {
            var (x, y) = vector;

            if (x == 0 && y == 0)
                return vector;

            var length = Hypot(x, y);

            return (x / length, y / length);
        }
    }
}