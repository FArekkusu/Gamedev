namespace ShooterServer
{
    class Program
    {
        public static void Main()
        {
            using var server = new Server();
            
            while (true)
                server.State.Update();
        }
    }
}