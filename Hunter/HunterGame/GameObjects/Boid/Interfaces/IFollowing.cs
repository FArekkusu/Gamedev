namespace HunterGame.GameObjects.Boid.Interfaces
{
    public interface IFollowing
    {
        public void Follow(double elapsedTime, WorldState worldState);
        
        public void StartFollowing();
    }
}