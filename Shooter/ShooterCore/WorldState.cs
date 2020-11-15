using System;
using System.Collections.Generic;
using Geometry;

namespace ShooterCore
{
    public class WorldState
    {
        public List<Character> Characters = new List<Character>();
        public List<Bullet> Bullets = new List<Bullet>();
        public List<(Rectangle, BuffType)> Pickups = new List<(Rectangle, BuffType)>();
        public List<Wall> Walls = new List<Wall>();

        public void RandomizeCharactersPositions(Random random)
        {
            for (var i = Characters.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                
                var temp = Characters[i];
                Characters[i] = Characters[j];
                Characters[j] = temp;
            }
        }
    }
}