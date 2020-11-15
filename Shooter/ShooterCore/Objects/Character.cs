using System.Collections.Generic;
using Geometry;

namespace ShooterCore
{
    public class Character : CircularObject
    {
        public int Hp = 100;
        public int Damage = 10;
        public double LinearVelocity = 100;
        public double BulletLinearVelocity = 200;
        public double Cooldown = 0;
        public List<Buff> Buffs = new List<Buff>();

        public Character((double, double) position, double radius) : base(new Circle(position, radius), 0) {}
    }
}