using System.Collections.Generic;
using ShooterCore;

namespace ShooterServer
{
    public static class Presets
    {
        public static WorldState TestWorldState = new WorldState
        {
            Characters = new List<Character>
            {
                new Character((100, 100), 20),
                new Character((100, 200), 20),
                new Character((200, 100), 20),
                new Character((200, 200), 20),
            },
            Walls = new List<Wall>
            {
                new Wall((10, 10), 310, 5),
                new Wall((10, 10), 5, 310),
                new Wall((10, 315), 310, 5),
                new Wall((315, 10), 5, 310),
            }
        };

        public static List<Buff> TestBuffs = new List<Buff>
        {
            new Buff(BuffType.CharacterSpeed, Modifier.IncreaseCharacterSpeed, Modifier.DecreaseCharacterSpeed, ModifierReplacementStrategy.Replace, 5, (32, 32)),
            new Buff(BuffType.CharacterDamage, Modifier.IncreaseCharacterDamage, Modifier.DecreaseCharacterDamage, ModifierReplacementStrategy.Replace, 5, (32, 32)),
            new Buff(BuffType.CharacterHealth, Modifier.IncreaseCharacterHealth, Modifier.DoNothing, ModifierReplacementStrategy.Append, 0, (32, 32)),
            new Buff(BuffType.BulletSpeed, Modifier.IncreaseBulletSpeed, Modifier.DecreaseBulletSpeed, ModifierReplacementStrategy.Replace, 5, (32, 32)),
        };
        
        public static List<(double, double)> TestBuffPositions = new List<(double, double)>
        {
            (20, 20),
            (70, 20),
            (120, 20),
            (170, 20),
        };
    }
}