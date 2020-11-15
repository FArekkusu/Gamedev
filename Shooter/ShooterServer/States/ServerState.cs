using System;
using System.Net;

namespace ShooterServer
{
    public abstract class ServerState
    {
        public Server Server;

        protected ServerState(Server server)
        {
            Server = server;
        }

        public virtual void HandleConnect(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);

            if (id > -1)
            {
                Server.ConnectionLastReceived[id] = DateTime.Now;
                
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
                Server.ConnectionLastReceived[id] = DateTime.Now;
        }

        public virtual void HandleInputs(IPEndPoint endpoint, ((int, int), bool, double) data)
        {
            var id = Server.FindUserSlot(endpoint);
                    
            if (id > -1)
                Server.ConnectionLastReceived[id] = DateTime.Now;
        }

        public virtual void HandleAcknowledge(IPEndPoint endpoint)
        {
            var id = Server.FindUserSlot(endpoint);
                    
            if (id > -1)
                Server.ConnectionLastReceived[id] = DateTime.Now;
        }
        
        public virtual void Update() {}
    }
}