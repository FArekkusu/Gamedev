using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ShooterCore;

namespace ShooterServer
{
    public class PlayingState : ServerState
    {
        public const int NextUpdateStep = 20;
        public const int SimulationTimeStep = NextUpdateStep;
        
        public WorldState WorldState;
        public PickupManager PickupManager;
        public ObjectsUpdater ObjectsUpdater;
        
        public DateTime LastUpdate;
        public DateTime NextUpdate;

        public List<(int, ((int, int), bool, double))> Actions = new List<(int, ((int, int), bool, double))>();
        
        public object Synchronizer = new object();

        public PlayingState(Server server, WorldState worldState, PickupManager pickupManager) : base(server)
        {
            WorldState = worldState;
            PickupManager = pickupManager;
            ObjectsUpdater = new ObjectsUpdater(WorldState, PickupManager);

            LastUpdate = DateTime.Now;
            NextUpdate = LastUpdate + TimeSpan.FromMilliseconds(NextUpdateStep);
        }

        public override void HandleDisconnect(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);

            if (id > -1)
            {
                Server.ConnectionStatuses[id] = false;
                
                WorldState.Characters[id].Hp = 0;
                WorldState.Characters[id].IsAlive = false;
            }
        }

        public override void HandleInputs(IPEndPoint endpoint, ((int, int), bool, double) data)
        {
            var id = Server.FindUserSlot(endpoint);

            if (id > -1)
            {
                var now = DateTime.Now;
                
                Server.ConnectionLastReceived[id] = now;

                lock (Synchronizer)
                {
                    Actions.Add((id, data));
                }
            }
        }

        public override void Update()
        {
            var now = DateTime.Now;

            if (now >= NextUpdate)
            {
                lock (Synchronizer)
                {
                    var timeDelta = SimulationTimeStep / 1000.0;
                    
                    PickupManager.Update(timeDelta);

                    ObjectsUpdater.UpdateCharactersStatuses(timeDelta);
                    
                    ObjectsUpdater.PerformActions(Actions, timeDelta);

                    Actions.Clear();
                    
                    ObjectsUpdater.UpdateBullets(timeDelta);
                }
                
                LastUpdate = now;
                NextUpdate += TimeSpan.FromMilliseconds(NextUpdateStep);
            }
            
            if (WorldState.Characters.Count(character => character.IsAlive) > 1)
                Server.SendWorldState(Serializer.SerializeWorldState(WorldState));
            else
                Server.BroadcastGameOver();
        }
    }
}