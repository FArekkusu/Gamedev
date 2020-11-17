using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Network;

namespace ShooterClient
{
    public class Client : UdpClient
    {
        public const int ConnectionTimeOutMillis = 5_000;
        
        public static (string, int) EmptyConnection = ("", 0);
        
        public MyGame Game;
        public ClientState State = ClientState.Disconnected;
        public int ServerAssignedId;

        public (string, int) CurrentConnection = EmptyConnection;
        
        public DateTime LastReceivedTime = DateTime.MinValue;
        public int LastReceivedId;
        public int NextSentId;

        public Client(MyGame game)
        {
            Game = game;

            Task.Factory.StartNew(Listen);
        }

        public void NewConnect(string hostName, int port)
        {
            if (CurrentConnection == (hostName, port) && State != ClientState.Disconnected)
                return;
            
            if (CurrentConnection != EmptyConnection)
                DisconnectFromServer();
            
            Connect(hostName, port);

            CurrentConnection = (hostName, port);
            LastReceivedId = 0;
            NextSentId = 1;
        }

        public async void Listen()
        {
            while (true)
            {
                var (success, result) = await SafeCommunicator.Receive(this);

                if (!success)
                {
                    HandleConnectionDenied();
                    
                    continue;
                }

                var (protocolId, packetId, packetType, packetContent) = Datagram.Parse(result.Buffer);

                if (protocolId != Datagram.ProtocolId || packetId < LastReceivedId)
                    continue;

                LastReceivedId = packetId;
                
                if (packetType == PacketType.ConnectionAccepted)
                    HandleConnectionAccepted(packetContent[0]);
                else if (packetType == PacketType.ConnectionDenied)
                    HandleConnectionDenied();
                else if (packetType == PacketType.Ping)
                    HandlePing();
                else if (packetType == PacketType.Acknowledge)
                    HandleAcknowledge();
                else if (packetType == PacketType.GameOver)
                    HandleGameOver();
                else if (packetType == PacketType.WorldState)
                    HandleWorldState(packetContent);
            }
        }

        public void HandleConnectionAccepted(int id)
        {
            if (State == ClientState.Disconnected)
                State = ClientState.InLobby;
            
            LastReceivedTime = DateTime.Now;

            ServerAssignedId = id;
        }

        public void HandleConnectionDenied()
        {
            State = ClientState.Disconnected;
            LastReceivedTime = DateTime.MinValue;

            if (Game.State is PlayingState)
                Game.State = new ConnectingState(Game);
        }

        public void HandlePing()
        {
            LastReceivedTime = DateTime.Now;

            SendAcknowledge();
        }

        public void HandleAcknowledge()
        {
            LastReceivedTime = DateTime.Now;
        }

        public void HandleGameOver()
        {
            State = ClientState.Disconnected;
            LastReceivedTime = DateTime.MinValue;
                    
            Game.State = new ConnectingState(Game);
        }

        public void HandleWorldState(byte[] bytes)
        {
            State = ClientState.Playing;
            LastReceivedTime = DateTime.Now;

            var worldState = Serializer.DeserializeWorldState(bytes);

            if (Game.State is ConnectingState)
            {
                SendAcknowledge();
                
                Game.State = new PlayingState(Game, worldState);
            }
            else if (Game.State is PlayingState playingState)
                playingState.WorldState = worldState;
        }

        public async void SendAndCheck(byte[] datagram)
        {
            var success = await SafeCommunicator.Send(this, datagram);
            
            if (!success)
                HandleConnectionDenied();
        }

        public bool CheckLastReceived()
        {
            var timedOut = DateTime.Now - LastReceivedTime > TimeSpan.FromMilliseconds(ConnectionTimeOutMillis);

            if (timedOut)
            {
                DisconnectFromServer();
                
                HandleConnectionDenied();
            }
            
            return timedOut;
        }

        public void ConnectToServer()
        {
            var datagram = Datagram.Build(NextSentId++, PacketType.Connect);
            
            SendAndCheck(datagram);
        }

        public void DisconnectFromServer()
        {
            for (var i = 0; i < 10; i++)
            {
                var datagram = Datagram.Build(NextSentId++, PacketType.Disconnect);
                SendAndCheck(datagram);
            }

            HandleConnectionDenied();
        }

        public void Ping()
        {
            if (CheckLastReceived())
                return;

            var datagram = Datagram.Build(NextSentId++, PacketType.Ping);
            
            SendAndCheck(datagram);
        }

        public void SendInput(byte[] serializedInputs)
        {
            if (CheckLastReceived())
                return;
            
            var datagram = Datagram.Build(NextSentId++, PacketType.Inputs, serializedInputs);

            SendAndCheck(datagram);
        }

        public void SendAcknowledge()
        {
            var datagram = Datagram.Build(NextSentId++, PacketType.Acknowledge);
            
            SendAndCheck(datagram);
        }
    }
}