using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShooterClient.UI;

namespace ShooterClient.States
{
    public class ConnectingState : GameState
    {
        public const int NextPingMillis = 1_000;

        public readonly InputField IpInput;
        public readonly TextField ConnectionStatus;
        public readonly Button ConnectButton;
        public readonly Button DisconnectButton;
        public DateTime NextPingTime = DateTime.MaxValue;

        public ConnectingState(MyGame game) : base(game)
        {
            var menuFont = Game.Content.Load<SpriteFont>("MenuFont");
            var connectionFont = Game.Content.Load<SpriteFont>("ConnectionFont");

            IpInput = new InputField(new Vector2(10, 10), menuFont, Game.GraphicsDevice, 40);
            
            ConnectionStatus = new TextField(new Vector2(20, 50), connectionFont);
            
            ConnectButton = new Button(new Vector2(30, 100), menuFont, "Connect", (o, args) =>
            {
                Game.Client.NewConnect(IpInput.Content, 32123);
                
                for (var i = 0; i < 10; i++)
                    Game.Client.ConnectToServer();
            });
            
            DisconnectButton = new Button(new Vector2(30, 150), menuFont, "Disconnect", (o, args) =>
            {
                Game.Client.DisconnectFromServer();
            });
        }
        
        public override void Update()
        {
            if (Game.IsActive)
            {
                IpInput.Update();
                ConnectButton.Update();
                DisconnectButton.Update();
            }

            var now = DateTime.Now;

            if (Game.Client.State == ClientState.InLobby && NextPingTime == DateTime.MaxValue)
                NextPingTime = now;
            
            if (now >= NextPingTime && Game.Client.State == ClientState.InLobby)
            {
                Game.Client.Ping();
                
                NextPingTime = now + TimeSpan.FromMilliseconds(NextPingMillis);
            }
        }

        public override void Draw()
        {
            IpInput.Draw(Game.SpriteBatch);

            var content = "Status: " + (Game.Client.State == ClientState.Disconnected ? "Disconnected" : "Connected");
            var color = Game.Client.State == ClientState.Disconnected ? Color.Red : Color.Green;
            ConnectionStatus.Draw(Game.SpriteBatch, content, color);
            
            ConnectButton.Draw(Game.SpriteBatch);
            DisconnectButton.Draw(Game.SpriteBatch);
        }
    }
}