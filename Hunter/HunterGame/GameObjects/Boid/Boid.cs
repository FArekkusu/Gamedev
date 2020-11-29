using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public abstract class Boid : Creature
    {
        public const int TextureIndependentBorderAvoidanceDistance = 10;
        public const int MaximumAngleChange = 45;
        
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public BoidState State;
        
        public readonly double BorderAvoidanceDistance;

        protected Boid(Texture2D texture, Vector2 position) : base(texture, position)
        {
            BorderAvoidanceDistance = TextureIndependentBorderAvoidanceDistance + Texture.Width / 2;
        }

        public virtual void Update(GameTime gameTime, WorldState worldState) {}
        
        public IEnumerable<Creature> FindInRadius(IEnumerable<Creature> creatures, double radius)
        {
            foreach (var creature in creatures)
                if (creature != this && (creature.CenterPosition - CenterPosition).Length() <= radius)
                    yield return creature;
        }
        
        public void CheckArrival(ref Vector2 target, Random random, Vector2 borderAvoidingForce)
        {
            var distanceLeft = (target - CenterPosition).Length();
            
            if (distanceLeft < 1)
                target = ChooseNewTarget(random);
            else if (borderAvoidingForce != Vector2.Zero)
                target = ChooseNewTarget(random, false);
        }
        
        public Vector2 ChooseNewTarget(Random random, bool keepDirection = true)
        {
            var angle = MathHelper.ToRadians(random.Next(360));

            if (Velocity != Vector2.Zero && keepDirection)
            {
                var oldAngle = MathHelper.ToDegrees((float)Math.Atan2(Velocity.Y, Velocity.X));
                
                angle = oldAngle + random.Next(-MaximumAngleChange, MaximumAngleChange + 1);
                
                angle = MathHelper.ToRadians(angle);
            }

            var direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            
            var distance = random.Next(100, 151);
            
            return CenterPosition + direction * distance;
        }
        
        public Vector2 GetBordersAvoidingForce(double redirectingSpeed, double maxForce)
        {
            var (x, y) = CenterPosition;
            var desired = Velocity;

            if (x < BorderAvoidanceDistance)
                desired.X = (float)redirectingSpeed;
            else if (x > WorldState.MapWidth - BorderAvoidanceDistance)
                desired.X = -(float)redirectingSpeed;
            
            if (y < BorderAvoidanceDistance)
                desired.Y = (float)redirectingSpeed;
            if (y > WorldState.MapHeight - BorderAvoidanceDistance)
                desired.Y = -(float)redirectingSpeed;
            
            var steer = desired - Velocity;

            return steer.Limit(maxForce);
        }
        
        public Vector2 GetArrivalForce(Vector2 target, double maxSpeed, double maxForce)
        {
            var desired = target - CenterPosition;
            var distance = desired.Length();
            
            if (distance > 0)
                desired.Normalize();
            
            var steer = desired * (float)maxSpeed - Velocity;

            return steer.Limit(maxForce);
        }

        public Vector2 GetFleeingForce(List<Creature> creatures, double maxSpeed, double maxForce)
        {
            var sum = Vector2.Zero;

            foreach (var creature in creatures)
            {
                var difference = CenterPosition - creature.CenterPosition;
                var distance = difference.Length();
                
                difference.Normalize();
                difference /= distance;

                sum += difference;
            }

            sum /= creatures.Count;
            sum.Normalize();
            sum *= (float)maxSpeed;

            var steer = sum - Velocity;

            return steer.Limit(maxForce);
        }
        
        public Vector2 GetSeparationForce(IEnumerable<Creature> creatures, double maxDistance, double maxSpeed, double maxForce)
        {
            var sum = Vector2.Zero;
            var count = 0;

            foreach (var creature in creatures)
            {
                var difference = CenterPosition - creature.CenterPosition;
                var distance = difference.Length();
                
                if (distance <= maxDistance)
                {
                    difference.Normalize();
                    difference /= distance;
        
                    sum += difference;
                    count++;
                }
            }
            
            if (count == 0)
                return sum;
        
            sum /= count;
            sum.Normalize();
            sum *= (float)maxSpeed;
        
            var steer = sum - Velocity;
        
            return steer.Limit(maxForce);
        }
        
        public Vector2 GetCohesionForce(IEnumerable<Creature> creatures, double maxDistance, double maxSpeed, double maxForce)
        {
            var sum = Vector2.Zero;
            var count = 0;
        
            foreach (var creature in creatures)
                if (creature != this && (creature.CenterPosition - CenterPosition).Length() <= maxDistance)
                {
                    sum += creature.CenterPosition;
                    count++;
                }
        
            if (count == 0)
                return sum;
            
            sum /= count;
        
            return GetArrivalForce(sum, maxSpeed, maxForce);
        }
        
        public void ApplyForces(double maxSpeed)
        {
            Velocity += Acceleration;
            
            Velocity = Velocity.Limit(maxSpeed);

            Position += Velocity;
            
            Acceleration = Vector2.Zero;
        }
    }
}