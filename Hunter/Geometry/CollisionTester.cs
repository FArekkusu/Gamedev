namespace Geometry
{
    public static class CollisionTester
    {
        public static bool TestCircleCircle(Circle a, Circle b)
        {
            var dist = Utils.Hypot(a.X - b.X, a.Y - b.Y);
            var margin = a.Radius + b.Radius;
            return dist < margin;
        }

        public static bool TestCircleRectangle(Circle circle, Rectangle rectangle)
        {
            var insideHorizontal = Utils.Between(circle.X, rectangle.LeftX, rectangle.RightX);
            var insideVertical = Utils.Between(circle.Y, rectangle.UpperY, rectangle.LowerY);
            
            if (insideHorizontal && insideVertical)
                return true;
            
            var closestX = Utils.Clamp(circle.X, rectangle.LeftX, rectangle.RightX);
            var closestY = Utils.Clamp(circle.Y, rectangle.UpperY, rectangle.LowerY);
            
            return Utils.Hypot(circle.X - closestX, circle.Y - closestY) < circle.Radius;
        }
    }
}