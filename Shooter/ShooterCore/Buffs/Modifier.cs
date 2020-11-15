using System;

namespace ShooterCore
{
    public class Modifier
    {
        public static readonly Action<GameObject> IncreaseCharacterSpeed = o =>
        {
            if (o is Character character)
                character.LinearVelocity *= 1.5;
        };
        
        public static readonly Action<GameObject> DecreaseCharacterSpeed = o =>
        {
            if (o is Character character)
                character.LinearVelocity /= 1.5;
        };
        
        public static readonly Action<GameObject> IncreaseCharacterDamage = o =>
        {
            if (o is Character character)
                character.Damage *= 2;
        };
        
        public static readonly Action<GameObject> DecreaseCharacterDamage = o =>
        {
            if (o is Character character)
                character.Damage /= 2;
        };
        
        public static readonly Action<GameObject> IncreaseCharacterHealth = o =>
        {
            if (o is Character character)
                character.Hp = Math.Min(100, character.Hp + 30);
        };
        
        public static readonly Action<GameObject> IncreaseBulletSpeed = o =>
        {
            if (o is Character character)
                character.BulletLinearVelocity *= 1.5;
        };
        
        public static readonly Action<GameObject> DecreaseBulletSpeed = o =>
        {
            if (o is Character character)
                character.BulletLinearVelocity /= 1.5;
        };

        public static readonly Action<GameObject> DoNothing = o => {};
    }
}