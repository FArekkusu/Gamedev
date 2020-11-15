using System;
using System.Net;
using System.Threading;

namespace ShooterServer
{
    public class LobbyState : ServerState
    {
        public const int PreparationTime = 5_000;
        public const int TickLength = 1_000;
        
        public int TicksLeft = -1;

        public LobbyState(Server server) : base(server) {}

        public override void HandleConnect(IPEndPoint endpoint)
        {
            Server.CheckLastReceived();
            
            var id = Server.FindFreeSlot(endpoint);
                
            if (id > -1)
            {
                Server.ConnectionAddresses[id] = endpoint;
                Server.ConnectionStatuses[id] = true;
                Server.ConnectionLastReceived[id] = DateTime.Now;
                
                Server.SendConnectionAccepted(endpoint, id);
            }
            else
                Server.SendConnectionDenied(endpoint, id);
        }

        public override void HandleDisconnect(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);
                
            if (id > -1)
                Server.ConnectionStatuses[id] = false;
        }

        public override void Update()
        {
            var connectedCount = Server.CountConnected();

            if (connectedCount < 2)
                TicksLeft = PreparationTime / TickLength;
            else if (TicksLeft > 0)
            {
                TicksLeft--;
                Console.WriteLine($"[Lobby state] Moving to preparation state in {TicksLeft}");
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