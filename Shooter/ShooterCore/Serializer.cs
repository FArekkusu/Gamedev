using System;
using System.Collections.Generic;
using Geometry;

namespace ShooterCore
{
    public static class Serializer
    {
        public static byte[] SerializeInput((int, int) movementDirection, bool isShooting, double shotDirection)
        {
            var bytes = new byte[1 + (isShooting ? sizeof(double) : 0)];

            var (x, y) = movementDirection;

            if (x != 0)
            {
                bytes[0] |= 1 << 0;
                if (x == 1)
                    bytes[0] |= 1 << 1;
            }

            if (y != 0)
            {
                bytes[0] |= 1 << 2;
                if (y == 1)
                    bytes[0] |= 1 << 3;
            }

            if (isShooting)
            {
                bytes[0] |= 1 << 4;
                var shotDirectionBytes = BitConverter.GetBytes(shotDirection);

                for (var i = 0; i < shotDirectionBytes.Length; i++)
                    bytes[i + 1] = shotDirectionBytes[i];
            }

            return bytes;
        }

        public static ((int, int), bool, double) DeserializeInput(byte[] bytes)
        {
            var x = 0;
            var y = 0;

            if ((bytes[0] & 1) == 1)
                x = (bytes[0] & 2) == 2 ? 1 : -1;
            
            if ((bytes[0] & 4) == 4)
                y = (bytes[0] & 8) == 8 ? 1 : -1;

            var isShooting = (bytes[0] & 16) == 16;
            var shotDirection = isShooting ? BitConverter.ToDouble(bytes, 1) : 0;

            return ((x, y), isShooting, shotDirection);
        }
        
        public static byte[] SerializeWorldState(WorldState worldState)
        {
            var bytes = new List<byte>();
            
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

            return bytes.ToArray();
        }
        
        public static WorldState DeserializeWorldState(byte[] bytes)
        {
            var worldState = new WorldState();
            
            var current = 0;
            
            // Character:
            //     bool IsAlive
            //     int Hp
            //     (double, double) position
            //     double radius
            var charactersCount = (int)bytes[current++];
            for (var i = 0; i < charactersCount; i++)
            {
                var isAlive = (bytes[current] & 128) > 0;
                
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

            return worldState;
        }
    }
}