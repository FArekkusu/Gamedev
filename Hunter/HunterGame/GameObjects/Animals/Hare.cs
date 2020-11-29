using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public class Hare : Boid
    {
        
        public const double FleeingSpeed = 375;
        public const double WanderingSpeed = 37.5;
        public const double MaxForce = 5;
        public const double TextureIndependentFearDistance = 50;
        public const double TextureIndependentCalmingDistance = 100;
        
        public readonly double FearDistance;
        public readonly double CalmingDistance;
        
        public Vector2 WanderTarget;

        public Hare(Texture2D texture, Vector2 position) : base(texture, position)
        {
            FearDistance = TextureIndependentFearDistance + Texture.Width / 2.0;
            CalmingDistance = TextureIndependentCalmingDistance + Texture.Width / 2.0;

            State = BoidState.Wandering;
            WanderTarget = CenterPosition;
        }

        public override void Update(GameTime gameTime, WorldState worldState)
        {
            var elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;
            
            if (State == BoidState.Wandering)
                Wander(elapsedTime, worldState);
            else if (State == BoidState.Fleeing)
                Flee(elapsedTime, worldState);
        }

        public void Wander(double elapsedTime, WorldState worldState)
        {
            var close = FindInRadius(worldState.Creatures, FearDistance);

            if (close.Any())
            {
                StartFleeing();
                return;
            }
            
            var borderAvoidingForce = GetBordersAvoidingForce(FleeingSpeed, MaxForce);
            
            CheckArrival(ref WanderTarget, worldState.Random, borderAvoidingForce);

            Acceleration += GetArrivalForce(WanderTarget, WanderingSpeed, MaxForce);
            Acceleration += borderAvoidingForce;
            
            ApplyForces(WanderingSpeed * elapsedTime);
        }

        public void StartWandering(Random random)
        {
            State = BoidState.Wandering;
            
            WanderTarget = ChooseNewTarget(random);
        }

        public void Flee(double elapsedTime, WorldState worldState)
        {
            var close = FindInRadius(worldState.Creatures, CalmingDistance).ToList();

            if (close.Count == 0)
            {
                StartWandering(worldState.Random);
                return;
            }

            Acceleration += GetFleeingForce(close, FleeingSpeed, MaxForce);
            Acceleration += GetBordersAvoidingForce(FleeingSpeed, MaxForce) * 2;

            ApplyForces(FleeingSpeed * elapsedTime);
        }

        public void StartFleeing()
        {
            State = BoidState.Fleeing;
        }
    }
}