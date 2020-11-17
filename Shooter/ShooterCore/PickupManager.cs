using System;
using System.Collections.Generic;
using Geometry;

namespace ShooterCore
{
    public class PickupManager
    {
        public WorldState WorldState;
        public List<Buff> Buffs;
        public Random Random;
        
        public List<((double, double), BuffType, double)> Queue = new List<((double, double), BuffType, double)>();
        
        public PickupManager(WorldState worldState, List<Buff> buffs, List<(double, double)> positions, Random random)
        {
            WorldState = worldState;
            Buffs = buffs;
            Random = random;
            
            positions = new List<(double, double)>(positions);
            
            while (positions.Count > 0)
            {
                var i = random.Next(positions.Count);
                
                Notify(positions[i]);

                positions.RemoveAt(i);
            }
        }

        public void Notify((double, double) position)
        {
            var time = Random.Next(2, 6);
            var buffType = Buffs[Random.Next(Buffs.Count)].Type;
            
            Queue.Add((position, buffType, time));
        }

        public Buff GetBuff(BuffType buffType)
        {
            foreach (var buff in Buffs)
                if (buff.Type == buffType)
                    return buff;
            
            throw new ArgumentException($"Buff of type {buffType} does not exist");
        }

        public void Update(double timeDelta)
        {
            var newQueue = new List<((double, double), BuffType, double)>();

            foreach (var (position, buffType, timeLeft) in Queue)
            {
                var newTimeLeft = timeLeft - timeDelta;
                
                if (newTimeLeft <= 0)
                {
                    var (x, y) = position;
                    var (w, h) = GetBuff(buffType).Dimensions;
                    
                    WorldState.Pickups.Add((new Rectangle((x, y + h), (x + w, y)), buffType));
                }
                else
                    newQueue.Add((position, buffType, newTimeLeft));
            }

            Queue = newQueue;
        }
    }
}