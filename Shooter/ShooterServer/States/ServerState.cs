using System;
using System.Net;

namespace ShooterServer.States
{
    public abstract class ServerState
    {
        public readonly Server Server;

        protected ServerState(Server server)
        {
            Server = server;
        }

        public virtual void HandleConnect(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);

            if (id > -1)
            {
                Server.Connections[id].LastReceivedTime = DateTime.Now;
                
                Server.SendConnectionAccepted(endpoint, id);
            }
            else
                Server.SendConnectionDenied(endpoint);
        }
        
        public virtual void HandleDisconnect(IPEndPoint endpoint) {}
        
        public virtual void HandlePing(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);
                    
            if (id > -1)
                Server.Connections[id].LastReceivedTime = DateTime.Now;
        }
        
        public virtual void HandleInputs(IPEndPoint endpoint, byte[] data)
        {
            var id = Server.FindUserSlot(endpoint);
                    
            if (id > -1)
                Server.Connections[id].LastReceivedTime = DateTime.Now;
        }

        public virtual void HandleAcknowledge(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);
                    
            if (id > -1)
                Server.Connections[id].LastReceivedTime = DateTime.Now;
        }
        
        public virtual void Update() {}
    }
}