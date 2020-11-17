using System.Collections.Generic;
using ShooterCore;
using ShooterCore.Buffs;
using ShooterCore.Objects;

namespace ShooterServer
{
    public static class Presets
    {
        public const int CharacterRadius = 20; // hard-coded for existing character texture
        public static readonly (int, int) PickupDimensions = (32, 32); // hard-coded for existing pickup textures
        
        public static readonly WorldState TestWorldState = new WorldState
        {
            Characters = new List<Character>
            {
                new Character((75, 75), CharacterRadius),
                new Character((75, 330), CharacterRadius),
                new Character((430, 75), CharacterRadius),
                new Character((430, 330), CharacterRadius),
            },
            Walls = new List<Wall>
            {
                new Wall((0, 0), 510, 5),
                new Wall((0, 0), 5, 410),
                new Wall((0, 405), 510, 5),
                new Wall((505, 0), 5, 410),
            }
        };

        public static readonly List<Buff> TestBuffs = new List<Buff>
        {
            new Buff(BuffType.CharacterSpeed, Modifier.IncreaseCharacterSpeed, Modifier.ResetCharacterSpeed, ModifierReplacementStrategy.Replace, 5, PickupDimensions),
            new Buff(BuffType.CharacterDamage, Modifier.IncreaseCharacterDamage, Modifier.ResetCharacterDamage, ModifierReplacementStrategy.Replace, 5, PickupDimensions),
            new Buff(BuffType.CharacterHealth, Modifier.IncreaseCharacterHealth, Modifier.DoNothing, ModifierReplacementStrategy.Append, 0, PickupDimensions),
            new Buff(BuffType.BulletSpeed, Modifier.IncreaseBulletSpeed, Modifier.ResetBulletSpeed, ModifierReplacementStrategy.Replace, 5, PickupDimensions),
        };
        
        public static readonly List<(double, double)> TestBuffPositions = new List<(double, double)>
        {
            (239, 15),
            (239, 363),
            (15, 189),
            (463, 189),
        };
    }
}