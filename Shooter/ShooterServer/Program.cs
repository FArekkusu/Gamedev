namespace ShooterServer
{
    class Program
    {
        static void Main()
        {
            using var server = new Server();
            
            while (true)
                server.State.Update();
        }
    }
}