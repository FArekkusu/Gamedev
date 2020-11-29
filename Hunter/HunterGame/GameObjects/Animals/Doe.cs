using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public class Doe : Boid
    {
        public const double FleeingSpeed = 225;
        public const double WanderingSpeed = 75;
        public const double MaxForce = 5;
        public const double TextureIndependentFearDistance = 70;
        public const double TextureIndependentCalmingDistance = 100;
        
        public readonly double FearDistance;
        public readonly double CalmingDistance;
        public readonly double SeparationBoostMargin;
        public readonly double SeparationActivationDistance;
        
        public DoeGroup Group;

        public Vector2 WanderTarget;

        public Doe(Texture2D texture, Vector2 position, DoeGroup group) : base(texture, position)
        {
            FearDistance = TextureIndependentFearDistance + Texture.Width / 2.0;
            CalmingDistance = TextureIndependentCalmingDistance + Texture.Width / 2.0;
            SeparationBoostMargin = Texture.Width;
            SeparationActivationDistance = Texture.Width * 1.5;
            
            State = BoidState.Wandering;
            WanderTarget = CenterPosition;
            
            group.Add(this);
        }

        public override void Update(GameTime gameTime, WorldState worldState)
        {
            Group.CheckAlive();

            var elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;

            if (State == BoidState.Wandering)
                Wander(elapsedTime, worldState);
            else if (State == BoidState.Fleeing)
                Flee(elapsedTime, worldState);
        }

        public void Wander(double elapsedTime, WorldState worldState)
        {
            var close = FindInRadius(worldState.Creatures, FearDistance).ToList();
            var enemies = close.Where(creature => creature is Hunter || creature is Wolf);

            if (enemies.Any())
            {
                StartFleeing();
                return;
            }
            
            var otherDoes = close.OfType<Doe>().ToList();
            var closeDoesFromOtherGroup = otherDoes.Where(doe => doe.Group != Group && (doe.CenterPosition - CenterPosition).Length() <= FearDistance).ToList();
            
            var borderAvoidingForce = GetBordersAvoidingForce(FleeingSpeed, MaxForce);
            var separationForce = GetSeparationForce(otherDoes, SeparationActivationDistance, WanderingSpeed, MaxForce);

            if (this == Group.Leader)
                HandleLeader(closeDoesFromOtherGroup, worldState.Random, borderAvoidingForce);
            else
                HandleFollower(ref separationForce);
            
            Acceleration += borderAvoidingForce * 2;
            Acceleration += separationForce * 0.15f;

            ApplyForces(WanderingSpeed * elapsedTime);
        }

        public void HandleLeader(List<Doe> closeDoesFromOtherGroup, Random random, Vector2 borderAvoidingForce)
        {
            if (!Group.IsComplete && closeDoesFromOtherGroup.Count > 0)
            {
                var otherGroup = closeDoesFromOtherGroup[0].Group;

                Group.Join(otherGroup);

                if (otherGroup.Size > DoeGroup.MaximumSize)
                    otherGroup.SplitIntoHalves();
            }
            else
                CheckArrival(ref WanderTarget, random, borderAvoidingForce);

            Acceleration += GetArrivalForce(WanderTarget, WanderingSpeed, MaxForce) * 2;
        }

        public void HandleFollower(ref Vector2 separationForce)
        {
            Acceleration += GetArrivalForce(Group.Leader.CenterPosition, WanderingSpeed, MaxForce) * 0.05f;
                
            if ((Group.Leader.CenterPosition - CenterPosition).Length() <= SeparationBoostMargin)
                separationForce *= 10;
        }

        public void StartWandering(Random random)
        {
            State = BoidState.Wandering;
            
            if (this == Group.Leader)
                WanderTarget = ChooseNewTarget(random);
        }

        public void Flee(double elapsedTime, WorldState worldState)
        {
            var close = FindInRadius(worldState.Creatures, CalmingDistance).ToList();
            var enemies = close.Where(creature => creature is Hunter || creature is Wolf).ToList();

            if (enemies.Count == 0)
            {
                StartWandering(worldState.Random);
                return;
            }

            var otherDoes = close.OfType<Doe>().ToList();
            
            Acceleration += GetSeparationForce(otherDoes, SeparationActivationDistance, FleeingSpeed, MaxForce) * 0.2f;
            Acceleration += GetFleeingForce(enemies, FleeingSpeed, MaxForce);
            Acceleration += GetBordersAvoidingForce(FleeingSpeed, MaxForce) * 2;
            
            ApplyForces(FleeingSpeed * elapsedTime);
        }

        public void StartFleeing()
        {
            State = BoidState.Fleeing;
        }
    }
}