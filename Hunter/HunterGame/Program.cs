namespace HunterGame
{
    class Program
    {
        public static void Main()
        {
            using var game = new MyGame();
            
            game.Run();
        }
    }
}