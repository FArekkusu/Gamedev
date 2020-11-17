using System;
using System.Net;
using System.Threading;

namespace ShooterServer.States
{
    public class LobbyState : ServerState
    {
        public const int PreparationTime = 5_000;
        public const int TickLength = 1_000;
        
        public int TicksLeft = PreparationTime / TickLength;

        public LobbyState(Server server) : base(server) {}

        public override void HandleConnect(IPEndPoint endpoint)
        {
            Server.CheckLastReceived();
            
            var id = Server.FindFreeSlot(endpoint);
                
            if (id > -1)
            {
                if (!Server.Connections[id].IsPresent)
                    Server.Connections[id].LastReceivedId = 0;
                
                Server.Connections[id].Address = endpoint;
                Server.Connections[id].IsPresent = true;
                Server.Connections[id].LastReceivedTime = DateTime.Now;

                Server.SendConnectionAccepted(endpoint, id);
            }
            else
                Server.SendConnectionDenied(endpoint);
        }

        public override void HandleDisconnect(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);
                
            if (id > -1)
                Server.Connections[id].IsPresent = false;
        }

        public override void HandlePing(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);

            if (id > -1)
            {
                Server.Connections[id].LastReceivedTime = DateTime.Now;
                
                Server.SendAcknowledge(endpoint);
            }
        }

        public override void Update()
        {
            var connectedCount = Server.CountConnected();

            if (connectedCount < Server.MinimumRequiredPlayers)
            {
                Console.WriteLine($"[Lobby state] {connectedCount} user(s) connected");
                
                TicksLeft = PreparationTime / TickLength;
            }
            else if (TicksLeft > 0)
            {
                Console.WriteLine($"[Lobby state] Moving to preparation state in {TicksLeft}");
                
                TicksLeft--;
            }
            else
            {
                Server.State = new PreparationState(Server, Presets.TestWorldState);
                return;
            }
            
            Thread.Sleep(TickLength);
        }
    }
}
