namespace HunterGame.GameObjects.Boid.Interfaces
{
    public interface IFleeing
    {
        public void Flee(double elapsedTime, WorldState worldState);

        public void StartFleeing();
    }
}