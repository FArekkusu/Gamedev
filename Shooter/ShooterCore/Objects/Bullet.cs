using Geometry;

namespace ShooterCore
{
    public class Bullet : CircularObject
    {
        public int ParentId;
        public int Damage;
        public double LinearVelocity;
        public double Direction;
        
        public Bullet((double, double) position, double radius, int parentId) : base(new Circle(position, radius), 0)
        {
            ParentId = parentId;
        }
    }
}