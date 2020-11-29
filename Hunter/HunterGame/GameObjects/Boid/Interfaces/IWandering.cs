using System;

namespace HunterGame.GameObjects.Boid.Interfaces
{
    public interface IWandering
    {
        public void Wander(double elapsedTime, WorldState worldState);

        public void StartWandering(Random random);
    }
}