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
                var datagram = await ReceiveAsync();
                
                var sender = datagram.RemoteEndPoint;
                var (protocolId, packetType, packetContent) = Datagram.Parse(datagram.Buffer);

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

        public async void SendConnectionAccepted(IPEndPoint endpoint, int id)
        {
            var datagram = Datagram.Build(PacketType.ConnectionAccepted, new [] {(byte)id});

            await SendAsync(datagram, datagram.Length, endpoint);
        }
        
        public async void SendConnectionDenied(IPEndPoint endpoint, int id)
        {
            var datagram = Datagram.Build(PacketType.ConnectionDenied, new [] {(byte)id});

            await SendAsync(datagram, datagram.Length, endpoint);
        }

        public async void SendWorldState(byte[] bytes)
        {
            var datagram = Datagram.Build(PacketType.WorldState, bytes);

            CheckLastReceived();
            
            for (var i = 0; i < MaxConnections; i++)
                if (ConnectionStatuses[i])
                    await SendAsync(datagram, datagram.Length, ConnectionAddresses[i]);
        }

        public async void SendGameOver()
        {
            var datagram = Datagram.Build(PacketType.GameOver);

            CheckLastReceived();
            
            for (var i = 0; i < MaxConnections; i++)
                if (ConnectionStatuses[i])
                    await SendAsync(datagram, datagram.Length, ConnectionAddresses[i]);
        }

        public async void BroadcastGameOver()
        {
            for (var i = 0; i < 10; i++)
            {
                SendGameOver();
                    
                await Task.Delay(100);
            }
                
            for (var i = 0; i < MaxConnections; i++)
                ConnectionStatuses[i] = false;
                
            State = new LobbyState(this);
        }
    }
}