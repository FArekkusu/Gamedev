using System;

namespace ShooterCore
{
    public class Buff : IEquatable<Buff>
    {
        public readonly BuffType Type;
        public readonly Action<GameObject> Apply;
        public readonly Action<GameObject> Revert;
        public readonly ModifierReplacementStrategy ReplacementStrategy;
        public double Timer;
        public readonly (double, double) Dimensions;

        public Buff(BuffType buffType, Action<GameObject> apply, Action<GameObject> revert, ModifierReplacementStrategy replacementStrategy, double timer, (double, double) dimensions)
        {
            Type = buffType;
            Apply = apply;
            Revert = revert;
            ReplacementStrategy = replacementStrategy;
            Timer = timer;
            Dimensions = dimensions;
        }

        public Buff Clone()
        {
            return new Buff(Type, Apply, Revert, ReplacementStrategy, Timer, Dimensions);
        }
        
        public bool Equals(Buff other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (ReferenceEquals(other, null))
                return false;
            return ReplacementStrategy == other.ReplacementStrategy && Apply == other.Apply && Revert == other.Revert;
        }

        public override bool Equals(object o)
        {
            return Equals(o as Buff);
        }

        public override int GetHashCode()
        {
            return (Type, Apply, Revert, ReplacementStrategy, Dimensions).GetHashCode();
        }

        public static bool operator ==(Buff a, Buff b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null))
                return false;
            if (ReferenceEquals(b, null))
                return false;
            return a.Equals(b);
        }

        public static bool operator !=(Buff a, Buff b)
        {
            return !(a == b);
        }
    }
}