namespace ShooterCore
{
    public enum PacketType
    {
        Acknowledge,
        
        Connect,
        ConnectionAccepted,
        ConnectionDenied,
        
        Disconnect,
        
        GameOver,
        
        Inputs,
        
        Ping,

        WorldState,
    }
}