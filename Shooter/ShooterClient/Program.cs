using System;

namespace ShooterClient
{
    class Program
    {
        [STAThread]
        public static void Main()
        {
            using var game = new MyGame();
            
            try
            {
                game.Run();
            }
            finally
            {
                if (game.Client.ConnectionSet)
                    game.Client.DisconnectFromServer();
            }
        }
    }
}