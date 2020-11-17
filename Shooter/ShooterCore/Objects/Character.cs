using System.Collections.Generic;
using Geometry;
using ShooterCore.Buffs;

namespace ShooterCore.Objects
{
    public class Character : CircularObject
    {
        public const int MaxHp = 100;
        public const int BaseDamage = 10;
        public const int BaseLinearVelocity = 2;
        public const int BaseBulletLinearVelocity = 250;
        public const double BaseCooldown = 0.5;
        
        public int Hp = MaxHp;
        public int Damage = BaseDamage;
        public double LinearVelocity = BaseLinearVelocity;
        public double BulletLinearVelocity = BaseBulletLinearVelocity;
        public double Cooldown = 0;
        public List<Buff> Buffs = new List<Buff>();

        public Character((double, double) position, double radius) : base(new Circle(position, radius)) {}
    }
}