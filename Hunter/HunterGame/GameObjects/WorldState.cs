using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterGame
{
    public class WorldState
    {
        public const int MapWidth = 2000;
        public const int MapHeight = 2000;
        public const int BorderWidth = 5;
        
        public readonly Dictionary<Type, Texture2D> Textures;
        public readonly GraphicsDevice GraphicsDevice;
        public readonly Random Random;
        
        public Hunter Hunter;
        public List<Bullet> Bullets = new List<Bullet>();
        public List<Creature> Creatures = new List<Creature>();
        public readonly List<Pit> Pits = new List<Pit>();

        public WorldState(Dictionary<Type, Texture2D> textures, GraphicsDevice graphicsDevice, Random random)
        {
            Textures = textures;
            GraphicsDevice = graphicsDevice;
            Random = random;
        }

        public void AddHunter()
        {
            var hunterPosition = new Vector2(MapWidth / 2f, MapHeight / 2f);
            
            Hunter = new Hunter(Textures[typeof(Hunter)], hunterPosition, Textures[typeof(Bullet)]);
            
            Creatures.Add(Hunter);
        }
        
        public void AddAnimals(int hareCount, int doeGroupsCount, int wolfCount)
        {
            for (var i = 0; i < hareCount; i++)
            {
                var position = GenerateSafePosition(typeof(Hare));

                AddAnimal(typeof(Hare), position);
            }

            for (var i = 0; i < doeGroupsCount; i++)
            {
                var group = new DoeGroup();
                
                var groupPosition = GenerateSafePosition(typeof(Doe));

                var membersCount = Random.Next(DoeGroup.MinimumCompleteSize, DoeGroup.MaximumSize + 1);
                
                for (var j = 0; j < membersCount; j++)
                {
                    var position = groupPosition;

                    position.X += j % 4;
                    position.Y += j >> 2;
                    
                    AddAnimal(typeof(Doe), position, group);
                }
            }

            for (var i = 0; i < wolfCount; i++)
            {
                var position = GenerateSafePosition(typeof(Wolf));

                AddAnimal(typeof(Wolf), position);
            }
        }

        public Vector2 GenerateSafePosition(Type t)
        {
            var margin = Boid.TextureIndependentBorderAvoidanceDistance + Textures[t].Width;
            
            var x = Random.Next(margin, MapWidth - margin);
            var y = Random.Next(margin, MapHeight - margin);

            return new Vector2(x, y);
        }

        public void AddAnimal(Type t, params object[] args)
        {
            var fullArgs = new object[args.Length + 1];
            fullArgs[0] = Textures[t];

            for (var i = 0; i < args.Length; i++)
                fullArgs[i + 1] = args[i];
            
            var creature = Activator.CreateInstance(t, fullArgs) as Creature;
            
            Creatures.Add(creature);
        }

        public void AddBorders()
        {
            var verticalBorderSize = new Vector2(BorderWidth, MapHeight + BorderWidth * 2);
            var horizontalBorderSize = new Vector2(MapWidth + BorderWidth * 2, BorderWidth);

            var topLeft = new Vector2(-BorderWidth, -BorderWidth);
            var topRight = new Vector2(MapWidth, -BorderWidth);
            var bottomLeft = new Vector2(-BorderWidth, MapHeight);
            
            var borders = new []
            {
                (topLeft, horizontalBorderSize),
                (bottomLeft, horizontalBorderSize),
                
                (topLeft, verticalBorderSize),
                (topRight, verticalBorderSize),
            };
            
            foreach (var (position, dimensions) in borders)
                Pits.Add(new Pit(position, dimensions, GraphicsDevice, Color.Red));
        }

        public void Update(GameTime gameTime, Vector2 centerOffset)
        {
            UpdateGameObjects(gameTime, centerOffset);
            
            var creaturesWithCircles = Creatures.Select(creature => (creature, creature.Circle)).ToList();
            CheckBulletsCollisions(creaturesWithCircles);
            CheckPitsCollisions(creaturesWithCircles);

            Creatures = Creatures.Where(creature => creature.IsAlive).ToList();
            Bullets = Bullets.Where(bullet => bullet.IsAlive).ToList();
        }

        public void UpdateGameObjects(GameTime gameTime, Vector2 centerOffset)
        {
            foreach (var creature in Creatures)
                if (creature is Boid boid)
                    boid.Update(gameTime, this);

            foreach (var bullet in Bullets)
                bullet.Update(gameTime);
            
            Hunter.Update(gameTime, centerOffset, Bullets);
        }

        public void CheckBulletsCollisions(List<(Creature, Circle)> creaturesWithCircles)
        {
            foreach (var bullet in Bullets)
            {
                var bulletCircle = bullet.Circle;
                
                foreach (var (creature, circle) in creaturesWithCircles)
                    if (creature.IsAlive && !(creature is Hunter) && CollisionTester.TestCircleCircle(bulletCircle, circle))
                    {
                        creature.IsAlive = false;
                        bullet.IsAlive = false;
                        break;
                    }
            }
        }

        public void CheckPitsCollisions(IEnumerable<(Creature, Circle)> creaturesWithCircles)
        {
            foreach (var (creature, circle) in creaturesWithCircles)
                foreach (var pit in Pits)
                    if (creature.IsAlive && CollisionTester.TestCircleRectangle(circle, pit.Rectangle))
                    {
                        creature.IsAlive = false;
                        break;
                    }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 centerOffset)
        {
            foreach (var pit in Pits)
                pit.Draw(spriteBatch);
            
            foreach (var creature in Creatures)
                if (!(creature is Hunter))
                    creature.Draw(spriteBatch);

            foreach (var bullet in Bullets)
                bullet.Draw(spriteBatch);
            
            Hunter.Draw(spriteBatch, centerOffset);
        }
    }
}