using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Network;

namespace ShooterServer
{
    public class Server : UdpClient
    {
        public const int Port = 32123;
        public const int MaxConnections = 4;
        public const int ConnectionTimeOutMillis = 5_000;
        public const int LobbyTimeOutMinutes = 10;

        public static Random Random = new Random();
        
        public ServerState State;

        public DateTime LastZeroConnected;
        
        public Connection[] Connections = new Connection[MaxConnections];

        public int NextSentId = 1;

        public Server() : base(Port)
        {
            State = new LobbyState(this);
            
            Task.Factory.StartNew(Listen);
        }

        public async void Listen()
        {
            while (true)
            {
                CheckRestartLobby();

                var (success, result) = await SafeCommunicator.Receive(this);
                
                if (!success)
                    continue;

                var sender = result.RemoteEndPoint;
                var (protocolId, packetId, packetType, packetContent) = Datagram.Parse(result.Buffer);

                if (protocolId != Datagram.ProtocolId)
                    continue;

                var userId = FindUserSlot(sender);

                if (userId > -1)
                {
                    if (packetId < Connections[userId].LastReceivedId && packetType != PacketType.Inputs)
                        continue;
                    
                    Connections[userId].LastReceivedId = Math.Max(packetId, Connections[userId].LastReceivedId);
                }

                if (packetType == PacketType.Connect)
                    State.HandleConnect(sender);
                else if (packetType == PacketType.Disconnect)
                    State.HandleDisconnect(sender);
                else if (packetType == PacketType.Ping)
                    State.HandlePing(sender);
                else if (packetType == PacketType.Inputs)
                    State.HandleInputs(sender, packetContent);
                else if (packetType == PacketType.Acknowledge)
                    State.HandleAcknowledge(sender);
            }
        }

        public void CheckRestartLobby()
        {
            var connectedCount = CountConnected();

            if (connectedCount == 0)
            {
                NextSentId = 1;
                LastZeroConnected = DateTime.Now;
            }
            else if (connectedCount == 1 && DateTime.Now - LastZeroConnected >= TimeSpan.FromMinutes(LobbyTimeOutMinutes))
            {
                BroadcastGameOver();
                    
                LastZeroConnected = DateTime.Now;
            }
        }

        public void CheckLastReceived()
        {
            for (var i = 0; i < MaxConnections; i++)
                if (DateTime.Now - Connections[i].LastReceivedTime > TimeSpan.FromMilliseconds(ConnectionTimeOutMillis))
                    Connections[i].IsPresent = false;
        }

        public int FindFreeSlot(IPEndPoint endpoint)
        {
            var id = -1;

            for (var i = 0; i < MaxConnections; i++)
                if (id == -1 && !Connections[i].IsPresent)
                    id = i;
                else if (Connections[i].IsPresent && Connections[i].Address.Equals(endpoint))
                {
                    id = i;
                    break;
                }

            return id;
        }

        public int FindUserSlot(IPEndPoint endpoint)
        {
            for (var i = 0; i < MaxConnections; i++)
                if (Connections[i].IsPresent && Connections[i].Address.Equals(endpoint))
                    return i;
            
            return -1;
        }

        public int CountConnected()
        {
            CheckLastReceived();

            return Connections.Count(connection => connection.IsPresent);
        }

        public async void SendAndCheck(byte[] datagram, IPEndPoint endpoint)
        {
            var success = await SafeCommunicator.Send(this, datagram, endpoint);

            if (!success)
                for (var i = 0; i < MaxConnections; i++)
                    if (Connections[i].Address.Equals(endpoint))
                        Connections[i].IsPresent = false;
        }

        public void SendConnectionAccepted(IPEndPoint endpoint, int id)
        {
            var datagram = Datagram.Build(NextSentId++, PacketType.ConnectionAccepted, new [] {(byte)id});
            
            CheckLastReceived();
            
            SendAndCheck(datagram, endpoint);
        }
        
        public async void SendConnectionDenied(IPEndPoint endpoint)
        {
            var datagram = Datagram.Build(NextSentId++, PacketType.ConnectionDenied);

            await SafeCommunicator.Send(this, datagram, endpoint);
        }

        public void SendPing()
        {
            var datagram = Datagram.Build(NextSentId++, PacketType.Ping);
            
            CheckLastReceived();

            for (var i = 0; i < MaxConnections; i++)
                if (Connections[i].IsPresent)
                    SendAndCheck(datagram, Connections[i].Address);
        }

        public void SendAcknowledge(IPEndPoint endpoint)
        {
            var datagram = Datagram.Build(NextSentId++, PacketType.Acknowledge);
            
            CheckLastReceived();

            SendAndCheck(datagram, endpoint);
        }

        public void SendWorldState(byte[] bytes)
        {
            var datagram = Datagram.Build(NextSentId++, PacketType.WorldState, bytes);

            CheckLastReceived();
            
            for (var i = 0; i < MaxConnections; i++)
                if (Connections[i].IsPresent)
                    SendAndCheck(datagram, Connections[i].Address);
        }

        public void SendGameOver()
        {
            var datagram = Datagram.Build(NextSentId++, PacketType.GameOver);
            
            CheckLastReceived();

            for (var i = 0; i < MaxConnections; i++)
                if (Connections[i].IsPresent)
                    SendAndCheck(datagram, Connections[i].Address);
        }

        public void BroadcastGameOver()
        {
            for (var i = 0; i < 10; i++)
                SendGameOver();

            for (var i = 0; i < MaxConnections; i++)
                Connections[i].IsPresent = false;
        }
    }
}