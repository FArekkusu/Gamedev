﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Network;
using ShooterCore;

namespace ShooterServer.States
{
    public class PlayingState : ServerState
    {
        public const int SimulationTimeStep = 20;
        public const double TimeDelta = SimulationTimeStep / 1000.0;
        
        public readonly WorldState WorldState;
        public readonly PickupManager PickupManager;
        public readonly Dictionary<int, int> LastPerformedActions = new Dictionary<int, int>();
        
        public DateTime NextUpdate;

        public readonly List<(int, int, ShooterCore.Action)> Actions = new List<(int, int, ShooterCore.Action)>();
        
        public readonly object Synchronizer = new object();

        public PlayingState(Server server, WorldState worldState, PickupManager pickupManager) : base(server)
        {
            WorldState = worldState;
            PickupManager = pickupManager;
            
            for (var i = 0; i < Server.MaxConnections; i++)
                if (Server.Connections[i].IsPresent)
                    LastPerformedActions[i] = -1;

            NextUpdate = DateTime.Now + TimeSpan.FromMilliseconds(SimulationTimeStep);
        }

        public override void HandleDisconnect(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);

            if (id > -1)
                WorldState.Characters[id].IsAlive = false;
        }

        public override void HandleInputs(IPEndPoint endpoint, byte[] data)
        {
            var id = Server.FindUserSlot(endpoint);

            if (id > -1)
            {
                var now = DateTime.Now;
                
                Server.Connections[id].LastReceivedTime = now;

                var actions = new List<(int, ShooterCore.Action)>();
                var current = 0;

                while (current < data.Length)
                {
                    var slice = data.Skip(current).ToArray();
                    var (actionId, action) = Serializer.DeserializeInput(slice);
                    
                    actions.Add((actionId, action));

                    current += sizeof(int) + 1 + (action.IsShooting ? sizeof(double) : 0);
                }
                
                lock (Synchronizer)
                {
                    foreach (var (actionId, action) in actions)
                        Actions.Add((id, actionId, action));
                }
            }
        }

        public override void Update()
        {
            if (DateTime.Now < NextUpdate)
                return;
            
            lock (Synchronizer)
            {
                PickupManager.Update(WorldState.Pickups, TimeDelta);

                ObjectsUpdater.UpdateCharactersStatuses(WorldState.Characters, TimeDelta);

                var actions = Actions.OrderBy(action => (action.Item2, action.Item1));
                
                ObjectsUpdater.PerformActions(actions, WorldState.Characters, WorldState.Bullets, WorldState.Pickups, WorldState.Walls, PickupManager, LastPerformedActions);

                Actions.Clear();
                
                ObjectsUpdater.UpdateBullets(WorldState.Bullets, WorldState.Characters, WorldState.Walls, TimeDelta);
                
                WorldState.Bullets = WorldState.Bullets.Where(bullet => bullet.IsAlive).ToList();
            }
            
            NextUpdate = DateTime.Now + TimeSpan.FromMilliseconds(SimulationTimeStep);

            if (WorldState.Characters.Count(character => character.IsAlive) >= Server.MinimumRequiredPlayers)
            {
                Server.SendWorldState(Serializer.SerializeWorldState(WorldState));

                for (var i = 0; i < Server.MaxConnections; i++)
                    WorldState.Characters[i].IsAlive = WorldState.Characters[i].IsAlive && Server.Connections[i].IsPresent;
            }
            else
            {
                Server.BroadcastGameOver();
                
                Server.State = new LobbyState(Server);
            }
        }
    }
}