using System;
using System.Collections.Generic;
using Geometry;
using ShooterCore;
using ShooterCore.Buffs;
using ShooterCore.Objects;

namespace Network
{
    public static class Serializer
    {
        public static byte[] SerializeInput(int actionId, ShooterCore.Action action)
        {
            var bytes = new byte[sizeof(int) + 1 + (action.IsShooting ? sizeof(double) : 0)];
            var current = 0;
            
            foreach (var b in BitConverter.GetBytes(actionId))
                bytes[current++] = b;

            var x = action.Dx;
            var y = action.Dy;

            if (x != 0)
            {
                bytes[current] |= 1;
                if (x == 1)
                    bytes[current] |= 2;
            }

            if (y != 0)
            {
                bytes[current] |= 4;
                if (y == 1)
                    bytes[current] |= 8;
            }

            if (action.IsShooting)
            {
                bytes[current++] |= 16;

                foreach (var b in BitConverter.GetBytes(action.MouseX))
                    bytes[current++] = b;
                foreach (var b in BitConverter.GetBytes(action.MouseY))
                    bytes[current++] = b;
            }

            return bytes;
        }

        public static (int, ShooterCore.Action) DeserializeInput(byte[] bytes)
        {
            var current = 0;
            
            var actionId = BitConverter.ToInt32(bytes, current);
            current += sizeof(int);
            
            var x = 0;
            var y = 0;

            if ((bytes[current] & 1) == 1)
                x = (bytes[current] & 2) == 2 ? 1 : -1;
            
            if ((bytes[current] & 4) == 4)
                y = (bytes[current] & 8) == 8 ? 1 : -1;

            var isShooting = (bytes[current++] & 16) == 16;

            var mouseX = isShooting ? BitConverter.ToInt32(bytes, current) : 0;
            var mouseY = isShooting ? BitConverter.ToInt32(bytes, current + sizeof(int)) : 0;

            return (actionId, new ShooterCore.Action(x, y, isShooting, mouseX, mouseY));
        }
        
        public static byte[] SerializeWorldState(WorldState worldState)
        {
            var bytes = new List<byte>();

            SerializeCharacters(bytes, worldState);

            SerializeBullets(bytes, worldState);
            
            SerializePickups(bytes, worldState);

            SerializeWalls(bytes, worldState);
            
            return bytes.ToArray();
        }
        
        public static WorldState DeserializeWorldState(byte[] bytes)
        {
            var worldState = new WorldState();
            
            var current = 0;
            
            DeserializeCharacters(bytes, ref current, worldState);
            
            DeserializeBullets(bytes, ref current, worldState);
            
            DeserializePickups(bytes, ref current, worldState);
            
            DeserializeWalls(bytes, ref current, worldState);

            return worldState;
        }

        public static void SerializeCharacters(List<byte> bytes, WorldState worldState)
        {
            // Character:
            //     bool IsAlive
            //     int Hp
            //     (double, double) position
            //     double radius
            bytes.Add((byte)worldState.Characters.Count);
            foreach (var character in worldState.Characters)
            {
                var hp = character.Hp;

                if (character.IsAlive)
                    hp |= 128;
                
                var circle = character.Circle;
                
                bytes.Add((byte)hp);
                foreach (var b in BitConverter.GetBytes(circle.X))
                    bytes.Add(b);
                foreach (var b in BitConverter.GetBytes(circle.Y))
                    bytes.Add(b);
                foreach (var b in BitConverter.GetBytes(circle.Radius))
                    bytes.Add(b);
            }
        }
        
        public static void DeserializeCharacters(byte[] bytes, ref int current, WorldState worldState)
        {
            // Character:
            //     bool IsAlive
            //     int Hp
            //     (double, double) position
            //     double radius
            var charactersCount = (int)bytes[current++];
            for (var i = 0; i < charactersCount; i++)
            {
                var isAlive = (bytes[current] & 128) == 128;
                
                var hp = bytes[current++] & 127;
                
                var x = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);

                var y = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);
                
                var radius = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);
                
                var character = new Character((x, y), radius) {IsAlive = isAlive , Hp = hp};
                worldState.Characters.Add(character);
            }
        }

        public static void SerializeBullets(List<byte> bytes, WorldState worldState)
        {
            // Bullet:
            //     int ParentId
            //     (double, double) position
            //     double radius
            bytes.Add((byte)worldState.Bullets.Count);
            foreach (var bullet in worldState.Bullets)
            {
                var circle = bullet.Circle;
                
                bytes.Add((byte)bullet.ParentId);
                foreach (var b in BitConverter.GetBytes(circle.X))
                    bytes.Add(b);
                foreach (var b in BitConverter.GetBytes(circle.Y))
                    bytes.Add(b);
                foreach (var b in BitConverter.GetBytes(circle.Radius))
                    bytes.Add(b);
            }
        }

        public static void DeserializeBullets(byte[] bytes, ref int current, WorldState worldState)
        {
            // Bullet:
            //     int ParentId
            //     (double, double) position
            //     double radius
            var bulletsCount = (int) bytes[current++];
            for (var i = 0; i < bulletsCount; i++)
            {
                var parentId = (int)bytes[current++];
                
                var x = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);

                var y = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);
                
                var radius = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);
                
                var bullet = new Bullet((x, y), radius, parentId);
                worldState.Bullets.Add(bullet);
            }
        }
        
        public static void SerializePickups(List<byte> bytes, WorldState worldState)
        {
            // Pickup:
            //     BuffType Type
            //     Rectangle rectangle (leftX, lowerY, rightX, upperY)
            bytes.Add((byte)worldState.Pickups.Count);
            foreach (var (rectangle, buffType) in worldState.Pickups)
            {
                bytes.Add((byte)buffType);
                foreach (var b in BitConverter.GetBytes(rectangle.LeftX))
                    bytes.Add(b);
                foreach (var b in BitConverter.GetBytes(rectangle.LowerY))
                    bytes.Add(b);
                foreach (var b in BitConverter.GetBytes(rectangle.RightX))
                    bytes.Add(b);
                foreach (var b in BitConverter.GetBytes(rectangle.UpperY))
                    bytes.Add(b);
            }
        }
        
        public static void DeserializePickups(byte[] bytes, ref int current, WorldState worldState)
        {
            // Pickup:
            //     BuffType Type
            //     Rectangle rectangle (leftX, lowerY, rightX, upperY)
            var pickupsCount = (int) bytes[current++];
            for (var i = 0; i < pickupsCount; i++)
            {
                var buffType = (BuffType)bytes[current++];
                
                var leftX = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);
                
                var lowerY = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);
                
                var rightX = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);
                
                var upperY = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);
                
                worldState.Pickups.Add((new Rectangle((leftX, lowerY), (rightX, upperY)), buffType));
            }
        }

        public static void SerializeWalls(List<byte> bytes, WorldState worldState)
        {
            // Wall:
            //     (double, double) position
            //     double width
            //     double height
            bytes.Add((byte)worldState.Walls.Count);
            foreach (var wall in worldState.Walls)
            {
                var rectangle = wall.Rectangle;
                
                foreach (var b in BitConverter.GetBytes(rectangle.LeftX))
                    bytes.Add(b);
                foreach (var b in BitConverter.GetBytes(rectangle.RightX))
                    bytes.Add(b);
                foreach (var b in BitConverter.GetBytes(rectangle.UpperY))
                    bytes.Add(b);
                foreach (var b in BitConverter.GetBytes(rectangle.LowerY))
                    bytes.Add(b);
            }
        }

        public static void DeserializeWalls(byte[] bytes, ref int current, WorldState worldState)
        {
            // Wall:
            //     (double, double) position
            //     double width
            //     double height
            var wallsCount = (int) bytes[current++];
            for (var i = 0; i < wallsCount; i++)
            {
                var leftX = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);
                
                var rightX = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);
                
                var upperY = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);
                
                var lowerY = BitConverter.ToDouble(bytes, current);
                current += sizeof(double);

                var wall = new Wall((leftX, upperY), rightX - leftX, lowerY - upperY);
                worldState.Walls.Add(wall);
            }
        }
    }
}