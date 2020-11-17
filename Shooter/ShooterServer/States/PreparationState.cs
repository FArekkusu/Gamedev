using System;
using System.Linq;
using System.Net;
using System.Threading;
using Network;
using ShooterCore;

namespace ShooterServer
{
    public class PreparationState : ServerState
    {
        public const int PreparationTime = 5_000;
        public const int TickLength = 1_000;
        public const int SleepBetweenSends = 200;
        
        public int TicksLeft = PreparationTime / TickLength;

        public WorldState WorldState;
        public PickupManager PickupManager;
        public bool[] ReceivedWorldData;

        public PreparationState(Server server, WorldState worldState) : base(server)
        {
            WorldState = Serializer.DeserializeWorldState(Serializer.SerializeWorldState(worldState));
            
            PickupManager = new PickupManager(WorldState, Presets.TestBuffs, Presets.TestBuffPositions, Server.Random);
            
            WorldState.RandomizeCharactersPositions(Server.Random);

            ReceivedWorldData = new bool[Server.MaxConnections];

            for (var i = 0; i < Server.MaxConnections; i++)
            {
                var isConnected = Server.Connections[i].IsPresent;
                
                ReceivedWorldData[i] = !isConnected;

                if (!isConnected)
                    WorldState.Characters[i].IsAlive = false;
            }
        }

        public override void HandleDisconnect(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);

            if (id > -1)
            {
                Server.Connections[id].IsPresent = false;
                
                ReceivedWorldData[id] = true;
                
                WorldState.Characters[id].IsAlive = false;
            }
        }

        public override void HandleAcknowledge(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);

            if (id > -1)
            {
                Server.Connections[id].LastReceivedTime = DateTime.Now;
                ReceivedWorldData[id] = true;
            }
        }

        public override void Update()
        {
            var connectedCount = Server.CountConnected();

            if (connectedCount < 2)
            {
                Server.BroadcastGameOver();
                
                Server.State = new LobbyState(Server);
            }
            else if (ReceivedWorldData.All(received => received))
            {
                if (TicksLeft == 0)
                    Server.State = new PlayingState(Server, WorldState, PickupManager);
                else
                {
                    Console.WriteLine($"[Preparation state] Moving to playing state in {TicksLeft}");
                    
                    TicksLeft--;

                    Server.SendPing();
                    
                    Thread.Sleep(TickLength);
                }
            }
            else
            {
                for (var i = 0; i < Server.MaxConnections; i++)
                    if (!Server.Connections[i].IsPresent)
                        ReceivedWorldData[i] = true;
                
                Server.SendWorldState(Serializer.SerializeWorldState(WorldState));

                Thread.Sleep(SleepBetweenSends);
            }
        }
    }
}