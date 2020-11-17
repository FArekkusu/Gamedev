using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using ShooterCore.Buffs;
using ShooterCore.Objects;

namespace ShooterCore
{
    public static class ObjectsUpdater
    {
        public static void PerformActions(IEnumerable<(int, int, Action)> actions, List<Character> characters, List<Bullet> bullets, List<(Rectangle, BuffType)> pickups, List<Wall> walls, PickupManager pickupManager, Dictionary<int, int> lastPerformedActions)
        {
            foreach (var (playerId, actionId, action) in actions)
            {
                if (!characters[playerId].IsAlive || lastPerformedActions[playerId] >= actionId)
                    continue;

                lastPerformedActions[playerId] = actionId;

                if (action.Dx != 0 || action.Dy != 0)
                    TryMove(characters, walls, playerId, (action.Dx, action.Dy));
                        
                var character = characters[playerId];
                var circle = character.Circle;
                
                TryCollideWithPickups(character, pickups, pickupManager);
                
                if (action.IsShooting && character.Cooldown <= 0)
                {
                    var shotDirection = Math.Atan2(action.MouseY - circle.Y, action.MouseX - circle.X);
                    
                    Shoot(characters, bullets, playerId, shotDirection);
                }
            }
        }
        
        public static void UpdateCharactersStatuses(IEnumerable<Character> characters, double timeDelta)
        {
            foreach (var character in characters)
            {
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
        
        public static void UpdateBullets(List<Bullet> bullets, List<Character> characters, List<Wall> walls, double timeDelta)
        {
            for (var i = 0; i < bullets.Count; i++)
            {
                var bullet = bullets[i] = CreateMovedBullet(bullets[i], timeDelta);

                if (IsCollidingWithWalls(bullet, walls))
                    bullet.IsAlive = false;
                else
                    TryCollideWithCharacters(bullet, characters);
            }
        }
        
        public static void TryMove(List<Character> characters, IEnumerable<Wall> walls, int playerId, (double, double) movementVector)
        {
            var normalizedMovementVector = Utils.Normalize(movementVector);
            
            var newCharacter = CreateMovedCharacter(characters[playerId], normalizedMovementVector);
            
            var canMove = !IsCollidingWithWalls(newCharacter, walls) && !IsCollidingWithCharacters(newCharacter, characters, playerId);

            if (canMove)
                characters[playerId] = newCharacter;
        }

        public static void Shoot(List<Character> characters, List<Bullet> bullets, int playerId, double shotDirection)
        {
            var character = characters[playerId];
            var circle = character.Circle;
                            
            character.Cooldown = Character.BaseCooldown;
                            
            bullets.Add(new Bullet((circle.X, circle.Y), 2.5, playerId)
            {
                Damage = character.Damage,
                LinearVelocity = character.BulletLinearVelocity,
                Direction = shotDirection
            });
        }

        public static void ApplyBuff(Character character, Buff buff)
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
        
        public static Character CreateMovedCharacter(Character character, (double, double) movementVector)
        {
            var circle = character.Circle;

            var (dx, dy) = movementVector;
                            
            dx *= character.LinearVelocity;
            dy *= character.LinearVelocity;

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

        public static Bullet CreateMovedBullet(Bullet bullet, double timeDelta)
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

        public static bool IsCollidingWithCharacters(CircularObject circularObject, List<Character> characters, int ignoredId = -1)
        {
            for (var i = 0; i < characters.Count; i++)
            {
                if (i == ignoredId)
                    continue;
                                
                var enemy = characters[i];

                if (enemy.IsAlive && CollisionTester.TestCircleCircle(circularObject.Circle, enemy.Circle))
                    return true;
            }

            return false;
        }
        
        public static bool IsCollidingWithWalls(CircularObject circularObject, IEnumerable<Wall> walls)
        {
            foreach (var wall in walls)
                if (CollisionTester.TestCircleRectangle(circularObject.Circle, wall.Rectangle))
                    return true;

            return false;
        }

        public static void TryCollideWithCharacters(Bullet bullet, List<Character> characters)
        {
            for (var i = 0; i < characters.Count; i++)
            {
                var character = characters[i];
                                
                if (i != bullet.ParentId && character.IsAlive && CollisionTester.TestCircleCircle(bullet.Circle, character.Circle))
                {
                    character.Hp = Math.Max(0, character.Hp - bullet.Damage);

                    if (character.Hp == 0)
                        character.IsAlive = false;
                                    
                    bullet.IsAlive = false;

                    return;
                }
            }
        }
        
        public static void TryCollideWithPickups(Character character, List<(Rectangle, BuffType)> pickups, PickupManager pickupManager)
        {
            for (var i = 0; i < pickups.Count; i++)
            {
                var (rectangle, buffType) = pickups[i];
                var buff = pickupManager.GetBuff(buffType).Clone();

                if (CollisionTester.TestCircleRectangle(character.Circle, rectangle))
                {
                    ApplyBuff(character, buff);
                    
                    pickups.RemoveAt(i);

                    pickupManager.Notify((rectangle.LeftX, rectangle.UpperY));

                    i--;
                }
            }
        }
    }
}