using System;
using ShooterCore.Objects;

namespace ShooterCore.Buffs
{
    public static class Modifier
    {
        public const double CharacterLinearVelocityMultiplier = 1.5;
        public const int CharacterDamageMultiplier = 2;
        public const double BulletLinearVelocityMultiplier = 1.5;
        public const int RestoredHp = 30;
        
        public static readonly Action<GameObject> IncreaseCharacterSpeed = o =>
        {
            if (o is Character character)
                character.LinearVelocity = Character.BaseLinearVelocity * CharacterLinearVelocityMultiplier;
        };
        
        public static readonly Action<GameObject> ResetCharacterSpeed = o =>
        {
            if (o is Character character)
                character.LinearVelocity = Character.BaseLinearVelocity;
        };
        
        public static readonly Action<GameObject> IncreaseCharacterDamage = o =>
        {
            if (o is Character character)
                character.Damage = Character.BaseDamage * CharacterDamageMultiplier;
        };
        
        public static readonly Action<GameObject> ResetCharacterDamage = o =>
        {
            if (o is Character character)
                character.Damage = Character.BaseDamage;
        };
        
        public static readonly Action<GameObject> IncreaseCharacterHealth = o =>
        {
            if (o is Character character)
                character.Hp = Math.Min(Character.MaxHp, character.Hp + RestoredHp);
        };
        
        public static readonly Action<GameObject> IncreaseBulletSpeed = o =>
        {
            if (o is Character character)
                character.BulletLinearVelocity = Character.BaseBulletLinearVelocity * BulletLinearVelocityMultiplier;
        };
        
        public static readonly Action<GameObject> ResetBulletSpeed = o =>
        {
            if (o is Character character)
                character.BulletLinearVelocity = Character.BaseBulletLinearVelocity;
        };

        public static readonly Action<GameObject> DoNothing = o => {};
    }
}