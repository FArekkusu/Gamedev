using Geometry;

namespace ShooterCore.Objects
{
    public class Bullet : CircularObject
    {
        public readonly int ParentId;
        public int Damage;
        public double LinearVelocity;
        public double Direction;
        
        public Bullet((double, double) position, double radius, int parentId) : base(new Circle(position, radius))
        {
            ParentId = parentId;
        }
    }
}