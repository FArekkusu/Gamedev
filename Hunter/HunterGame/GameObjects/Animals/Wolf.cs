using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using HunterGame.GameObjects.Bases;
using HunterGame.GameObjects.Boid;
using HunterGame.GameObjects.Boid.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame.GameObjects.Animals
{
    public class Wolf : Boid.Boid, IWandering, IFollowing
    {
        public const double HuntingSpeed = 300;
        public const double WanderingSpeed = 75;
        public const double MaxForce = 5;
        public const double TextureIndependentAgroDistance = 100;
        public const double TextureIndependentCalmingDistance = 150;
        public const double LifeTime = 40;
        public const double StarvationTime = 30;

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

            if (State == BoidState.Wandering)
                Wander(elapsedTime, worldState);
            else if (State == BoidState.Following)
                Follow(elapsedTime, worldState);
            else
                throw new IncorrectBoidStateException($"Wolf cannot be in state {State}");
            
            if (LifeTimer <= StarvationTime)
                foreach (var creature in worldState.Creatures)
                    if (creature.IsAlive && !(creature is Wolf) && CollisionTester.TestCircleCircle(Circle, creature.Circle))
                    {
                        creature.IsAlive = false;
                        LifeTimer = LifeTime;
                        State = BoidState.Wandering;
                    }
            
            if (LifeTimer <= 0)
                IsAlive = false;
        }

        public void Wander(double elapsedTime, WorldState worldState)
        {
            var preyFound = FindPrey(worldState.Creatures, AgroDistance).Any();
            
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
            var prey = FindPrey(worldState.Creatures, CalmingDistance).ToList();
            
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
        
        public IEnumerable<Creature> FindPrey(IEnumerable<Creature> creatures, double distance)
        {
            return FindInRadius(creatures, distance).Where(creature => !(creature is Wolf));
        }
    }
}