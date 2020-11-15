using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ShooterCore;

namespace ShooterServer
{
    public class Server : UdpClient
    {
        public const int DefaultPort = 32123;
        public const int MaxConnections = 4;
        public const int ConnectionTimeOutMillis = 5_000;

        public static Random Random = new Random();
        
        public ServerState State;
        
        public IPEndPoint[] ConnectionAddresses = new IPEndPoint[MaxConnections];
        public bool[] ConnectionStatuses = new bool[MaxConnections];
        public DateTime[] ConnectionLastReceived = new DateTime[MaxConnections];

        public Server() : base(DefaultPort)
        {
            State = new LobbyState(this);
            
            Task.Factory.StartNew(Listen);
        }

        public async void Listen()
        {
            while (true)
            {
                var (success, result) = await SafeCommunicator.Receive(this);
                
                if (!success)
                    continue;

                var sender = result.RemoteEndPoint;
                var (protocolId, packetType, packetContent) = Datagram.Parse(result.Buffer);

                if (protocolId != Datagram.ProtocolId)
                    continue;

                if (packetType == PacketType.Connect)
                    State.HandleConnect(sender);
                else if (packetType == PacketType.Disconnect)
                    State.HandleDisconnect(sender);
                else if (packetType == PacketType.Ping)
                    State.HandlePing(sender);
                else if (packetType == PacketType.Inputs)
                    State.HandleInputs(sender, Serializer.DeserializeInput(packetContent));
                else if (packetType == PacketType.Acknowledge)
                    State.HandleAcknowledge(sender);
            }
        }

        public void CheckLastReceived()
        {
            for (var i = 0; i < MaxConnections; i++)
                if (DateTime.Now - ConnectionLastReceived[i] > TimeSpan.FromMilliseconds(ConnectionTimeOutMillis))
                    ConnectionStatuses[i] = false;
        }

        public int FindFreeSlot(IPEndPoint endpoint)
        {
            var id = -1;

            for (var i = 0; i < MaxConnections; i++)
                if (id == -1 && !ConnectionStatuses[i])
                    id = i;
                else if (ConnectionStatuses[i] && ConnectionAddresses[i].Equals(endpoint))
                {
                    id = i;
                    break;
                }

            return id;
        }

        public int FindUserSlot(IPEndPoint endpoint)
        {
            for (var i = 0; i < MaxConnections; i++)
                if (ConnectionStatuses[i] && ConnectionAddresses[i].Equals(endpoint))
                    return i;
            
            return -1;
        }

        public int CountConnected()
        {
            CheckLastReceived();

            return ConnectionStatuses.Count(isConnected => isConnected);
        }

        public async void SendAndCheck(byte[] datagram, IPEndPoint endpoint)
        {
            var success = await SafeCommunicator.Send(this, datagram, endpoint);

            if (!success)
                for (var i = 0; i < MaxConnections; i++)
                    if (ConnectionAddresses[i].Equals(endpoint))
                        ConnectionStatuses[i] = false;
        }

        public void SendConnectionAccepted(IPEndPoint endpoint, int id)
        {
            var datagram = Datagram.Build(PacketType.ConnectionAccepted, new [] {(byte)id});
            
            CheckLastReceived();
            
            SendAndCheck(datagram, endpoint);
        }
        
        public async void SendConnectionDenied(IPEndPoint endpoint)
        {
            var datagram = Datagram.Build(PacketType.ConnectionDenied);

            await SafeCommunicator.Send(this, datagram, endpoint);
        }

        public void SendPing()
        {
            var datagram = Datagram.Build(PacketType.Ping);

            CheckLastReceived();
            
            for (var i = 0; i < MaxConnections; i++)
                if (ConnectionStatuses[i])
                    SendAndCheck(datagram, ConnectionAddresses[i]);
        }

        public void SendAcknowledge(IPEndPoint endpoint)
        {
            var datagram = Datagram.Build(PacketType.Acknowledge);
            
            CheckLastReceived();

            SendAndCheck(datagram, endpoint);
        }

        public void SendWorldState(byte[] bytes)
        {
            var datagram = Datagram.Build(PacketType.WorldState, bytes);

            CheckLastReceived();
            
            for (var i = 0; i < MaxConnections; i++)
                if (ConnectionStatuses[i])
                    SendAndCheck(datagram, ConnectionAddresses[i]);
        }

        public void SendGameOver()
        {
            var datagram = Datagram.Build(PacketType.GameOver);

            CheckLastReceived();
            
            for (var i = 0; i < MaxConnections; i++)
                if (ConnectionStatuses[i])
                    SendAndCheck(datagram, ConnectionAddresses[i]);
        }

        public void BroadcastGameOver()
        {
            for (var i = 0; i < 10; i++)
                SendGameOver();

            for (var i = 0; i < MaxConnections; i++)
                ConnectionStatuses[i] = false;
        }
    }
}