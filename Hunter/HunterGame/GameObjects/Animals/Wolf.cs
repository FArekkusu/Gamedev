using System;
using System.Linq;
using Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public class Wolf : Boid
    {
        public const double HuntingSpeed = 300;
        public const double WanderingSpeed = 75;
        public const double MaxForce = 5;
        public const double TextureIndependentAgroDistance = 60;
        public const double TextureIndependentCalmingDistance = 100;
        public const double LifeTime = 30;
        public const double StarvationTime = 25;

        public readonly double AgroDistance;
        public readonly double CalmingDistance;
        
        public double LifeTimer = LifeTime;

        public Vector2 WanderTarget;
        
        public Wolf(Texture2D texture, Vector2 position) : base(texture, position)
        {
            AgroDistance = TextureIndependentAgroDistance + Texture.Width / 2.0;
            CalmingDistance = TextureIndependentCalmingDistance + Texture.Width / 2.0;
            
            State = BoidState.Wandering;
            WanderTarget = CenterPosition;
        }

        public override void Update(GameTime gameTime, WorldState worldState)
        {
            var elapsedTime = Math.Min(LifeTimer, gameTime.ElapsedGameTime.TotalSeconds);
            
            LifeTimer -= elapsedTime;

            if (LifeTimer <= 0)
            {
                IsAlive = false;
                return;
            }
            
            if (State == BoidState.Wandering)
                Wander(elapsedTime, worldState);
            else if (State == BoidState.Following)
                Follow(elapsedTime, worldState);

            foreach (var creature in worldState.Creatures)
                if (creature.IsAlive && !(creature is Wolf) && CollisionTester.TestCircleCircle(Circle, creature.Circle))
                {
                    creature.IsAlive = false;
                    LifeTimer = LifeTime;
                    State = BoidState.Wandering;
                }
        }

        public void Wander(double elapsedTime, WorldState worldState)
        {
            var close = FindInRadius(worldState.Creatures, AgroDistance);
            var preyFound = close.Any(creature => !(creature is Wolf));
            
            if (preyFound && LifeTimer <= StarvationTime)
            {
                StartFollowing();
                return;
            }

            var borderAvoidingForce = GetBordersAvoidingForce(HuntingSpeed, MaxForce);

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

        public void Follow(double elapsedTime, WorldState worldState)
        {
            var close = FindInRadius(worldState.Creatures, CalmingDistance);
            var prey = close.Where(creature => !(creature is Wolf)).ToList();
            
            if (prey.Count == 0)
            {
                StartWandering(worldState.Random);
                return;
            }

            var closest = prey.MinBy(creature => (creature.CenterPosition - CenterPosition).Length());

            Acceleration += GetArrivalForce(closest.CenterPosition, HuntingSpeed, MaxForce);
            Acceleration += GetBordersAvoidingForce(HuntingSpeed, MaxForce);

            ApplyForces(HuntingSpeed * elapsedTime);
        }

        public void StartFollowing()
        {
            State = BoidState.Following;
        }
    }
}