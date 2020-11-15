using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using ShooterCore;

namespace ShooterClient
{
    public class Client : UdpClient
    {
        public MyGame Game;
        public ClientState State = ClientState.Disconnected;
        public int ServerAssignedId;
        
        public bool ConnectionSet;
        
        public Client(MyGame game)
        {
            Game = game;
            
            Task.Factory.StartNew(Listen);
        }

        public void NewConnect(string hostName, int port)
        {
            Connect(hostName, port);

            if (State == ClientState.Denied)
                State = ClientState.Disconnected;
            
            ConnectionSet = true;
        }

        public async void Listen()
        {
            while (true)
            {
                if (!ConnectionSet)
                {
                    await Task.Delay(1000);
                    
                    continue;
                }
                
                var datagram = await ReceiveAsync();
                
                var (protocolId, packetType, packetContent) = Datagram.Parse(datagram.Buffer);

                if (protocolId != Datagram.ProtocolId)
                    continue;

                if (packetType == PacketType.ConnectionAccepted)
                    HandleConnectionAccepted(packetContent[0]);
                else if (packetType == PacketType.ConnectionDenied)
                    HandleConnectionDenied();
                else if (packetType == PacketType.GameOver)
                    HandleGameOver();
                else if (packetType == PacketType.WorldState)
                    HandleWorldState(packetContent);
            }
        }

        public void HandleConnectionAccepted(int id)
        {
            State = ClientState.InLobby;
            
            ServerAssignedId = id;
        }

        public void HandleConnectionDenied()
        {
            State = ClientState.Denied;
            
            ConnectionSet = false;

            Game.State = new ConnectingState(Game);
        }

        public void HandleGameOver()
        {
            State = ClientState.Disconnected;
                    
            Game.State = new ConnectingState(Game);
        }

        public void HandleWorldState(byte[] bytes)
        {
            State = ClientState.Playing;

            SendAcknowledge();

            var worldState = Serializer.DeserializeWorldState(bytes);

            if (Game.State is ConnectingState)
                Game.State = new PlayingState(Game, worldState);
            else if (Game.State is PlayingState playingState)
                playingState.WorldState = worldState;
        }

        public async void ConnectToServer()
        {
            var datagram = Datagram.Build(PacketType.Connect);
            
            await SendAsync(datagram, datagram.Length);
        }

        public async void DisconnectFromServer()
        {
            State = ClientState.Disconnected;
            
            var datagram = Datagram.Build(PacketType.Disconnect);

            for (var i = 0; i < 10; i++)
                await SendAsync(datagram, datagram.Length);
        }

        public async void Ping()
        {
            var datagram = Datagram.Build(PacketType.Ping);
            
            await SendAsync(datagram, datagram.Length);
        }

        public async void SendInput(byte[] serializedInputs)
        {
            var datagram = Datagram.Build(PacketType.Inputs, serializedInputs);

            await SendAsync(datagram, datagram.Length);
        }

        public async void SendAcknowledge()
        {
            var datagram = Datagram.Build(PacketType.Acknowledge);
            
            await SendAsync(datagram, datagram.Length);
        }
    }
}