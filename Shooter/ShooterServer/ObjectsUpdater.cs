using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using ShooterCore;

namespace ShooterServer
{
    public class ObjectsUpdater
    {
        public WorldState WorldState;
        public PickupManager PickupManager;

        public ObjectsUpdater(WorldState worldState, PickupManager pickupManager)
        {
            WorldState = worldState;
            PickupManager = pickupManager;
        }

        public void UpdateCharactersStatuses(double timeDelta)
        {
            for (var i = 0; i < WorldState.Characters.Count; i++)
            {
                var character = WorldState.Characters[i];
                        
                if (!character.IsAlive)
                    continue;

                if (character.Cooldown > 0)
                    character.Cooldown -= timeDelta;
                        
                foreach (var buff in character.Buffs)
                {
                    buff.Timer -= timeDelta;
                    if (buff.Timer <= 0)
                        buff.Revert(character);
                }

                character.Buffs = character.Buffs.Where(buff => buff.Timer > 0).ToList();
            }
        }

        public void PerformActions(List<(int, ((int, int), bool, double))> actions, double timeDelta)
        {
            foreach (var (playerId, ((x, y), isShooting, shotDirection)) in actions)
            {
                if (!WorldState.Characters[playerId].IsAlive)
                    continue;
                        
                if (x != 0 || y != 0)
                    TryMove(playerId, (x, y), timeDelta);
                        
                var character = WorldState.Characters[playerId];
                        
                CollideWithPickups(character);

                if (isShooting && WorldState.Characters[playerId].Cooldown <= 0)
                    Shoot(character, playerId, shotDirection);
            }
        }
        
        public void TryMove(int playerId, (double, double) movementVector, double timeDelta)
        {
            var normalizedMovementVector = Utils.Normalize(movementVector);
            
            var newCharacter = CreateMovedCharacter(playerId, normalizedMovementVector, timeDelta);
            
            var canMove = !IsCollidingWithWalls(newCharacter) && !IsCollidingWithCharacters(newCharacter, playerId);

            if (canMove)
                WorldState.Characters[playerId] = newCharacter;
        }

        public Character CreateMovedCharacter(int playerId, (double, double) movementVector, double timeDelta)
        {
            var character = WorldState.Characters[playerId];
            var circle = character.Circle;

            var (dx, dy) = movementVector;
                            
            dx *= character.LinearVelocity * timeDelta;
            dy *= character.LinearVelocity * timeDelta;

            return new Character((circle.X + dx, circle.Y + dy), circle.Radius)
            {
                Hp = character.Hp,
                Damage = character.Damage,
                LinearVelocity = character.LinearVelocity,
                BulletLinearVelocity = character.BulletLinearVelocity,
                Cooldown = character.Cooldown,
                Buffs = character.Buffs
            };
        }

        public bool IsCollidingWithWalls(CircularObject circularObject)
        {
            foreach (var wall in WorldState.Walls)
                if (CollisionTester.TestCircleRectangle(circularObject.Circle, wall.Rectangle))
                    return true;

            return false;
        }
        
        public bool IsCollidingWithCharacters(CircularObject circularObject, int ignoredId = -1)
        {
            for (var i = 0; i < WorldState.Characters.Count; i++)
            {
                if (i == ignoredId)
                    continue;
                                
                var enemy = WorldState.Characters[i];

                if (enemy.IsAlive && CollisionTester.TestCircleCircle(circularObject.Circle, enemy.Circle))
                    return true;
            }

            return false;
        }

        public void CollideWithPickups(Character character)
        {
            for (var i = 0; i < WorldState.Pickups.Count; i++)
            {
                var (rectangle, buffType) = WorldState.Pickups[i];
                var buff = PickupManager.GetBuff(buffType).Clone();

                if (CollisionTester.TestCircleRectangle(character.Circle, rectangle))
                {
                    ApplyBuff(character, buff);
                    
                    WorldState.Pickups.RemoveAt(i);

                    PickupManager.Notify((rectangle.LeftX, rectangle.UpperY));

                    i--;
                }
            }
        }

        public void ApplyBuff(Character character, Buff buff)
        {
            if (buff.ReplacementStrategy == ModifierReplacementStrategy.Replace)
                for (var i = 0; i < character.Buffs.Count; i++)
                    if (buff == character.Buffs[i])
                    {
                        character.Buffs[i] = buff;
                        return;
                    }
            
            character.Buffs.Add(buff);
            buff.Apply(character);
        }

        public void Shoot(Character character, int playerId, double shotDirection)
        {
            var circle = character.Circle;
                            
            character.Cooldown = 0.5;
                            
            WorldState.Bullets.Add(new Bullet((circle.X, circle.Y), 2.5, playerId)
            {
                Damage = character.Damage,
                LinearVelocity = character.BulletLinearVelocity,
                Direction = shotDirection
            });
        }

        public void UpdateBullets(double timeDelta)
        {
            for (var i = 0; i < WorldState.Bullets.Count; i++)
            {
                var bullet = WorldState.Bullets[i] = CreateMovedBullet(WorldState.Bullets[i], timeDelta);

                if (IsCollidingWithWalls(bullet))
                    bullet.IsAlive = false;
                else
                    CollideWithCharacters(bullet);
            }
            
            WorldState.Bullets = WorldState.Bullets.Where(bullet => bullet.IsAlive).ToList();
        }

        public Bullet CreateMovedBullet(Bullet bullet, double timeDelta)
        {
            var circle = bullet.Circle;

            var dx = Math.Cos(bullet.Direction) * bullet.LinearVelocity * timeDelta;
            var dy = Math.Sin(bullet.Direction) * bullet.LinearVelocity * timeDelta;
                        
            return new Bullet((circle.X + dx, circle.Y + dy), circle.Radius, bullet.ParentId)
            {
                Damage = bullet.Damage,
                LinearVelocity = bullet.LinearVelocity,
                Direction = bullet.Direction
            };
        }

        public void CollideWithCharacters(Bullet bullet)
        {
            for (var i = 0; i < WorldState.Characters.Count; i++)
            {
                var character = WorldState.Characters[i];
                                
                if (i != bullet.ParentId && character.IsAlive && CollisionTester.TestCircleCircle(bullet.Circle, character.Circle))
                {
                    character.Hp -= bullet.Damage;

                    if (character.Hp <= 0)
                        character.IsAlive = false;
                                    
                    bullet.IsAlive = false;

                    return;
                }
            }
        }
    }
}