using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame.GameObjects.Bases
{
    public abstract class Creature : CircularObject
    {
        public (Vector2, Vector2) CachedCenterPosition;

        public Vector2 CenterPosition
        {
            get
            {
                if (Position != CachedCenterPosition.Item1)
                {
                    var circle = Circle;
                    var newCenterPosition = new Vector2((float)circle.X, (float)circle.Y);
                    
                    CachedCenterPosition = (Position, newCenterPosition);
                }
        
                return CachedCenterPosition.Item2;
            }
        }

        protected Creature(Texture2D texture, Vector2 position) : base(texture, position) {}
    }
}