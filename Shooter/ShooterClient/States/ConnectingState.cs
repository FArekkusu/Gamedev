using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShooterClient
{
    public class ConnectingState : GameState
    {
        public const int NextPingMillis = 1_000;
        
        public Button StartGameButton;
        public InputField IpInput;
        public DateTime NextPingTime = DateTime.MaxValue;

        public ConnectingState(MyGame game) : base(game)
        {
            var font = Game.Content.Load<SpriteFont>("MenuFont");

            IpInput = new InputField(new Vector2(10, 10), font, Game.GraphicsDevice, 15);
            
            StartGameButton = new Button(new Vector2(100, 100), font, "Start game", (o, args) =>
            {
                Game.Client.NewConnect(IpInput.Content, 32123);
                
                for (var i = 0; i < 10; i++)
                    Game.Client.ConnectToServer();
            });
        }
        
        public override void Update()
        {
            IpInput.Update();
            StartGameButton.Update();

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
            StartGameButton.Draw(Game.SpriteBatch);
        }
    }
}