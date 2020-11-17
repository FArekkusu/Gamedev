using System;
using System.Linq;

namespace Network
{
    public static class Datagram
    {
        public const long ProtocolId = 546886159060960807;
        
        public static readonly byte[] ProtocolIdBytes = BitConverter.GetBytes(ProtocolId);
        
        public static byte[] Build(int packetId, PacketType packetType, byte[] bytes = null)
        {
            bytes ??= new byte[0];
            
            var datagram = new byte[sizeof(long) + sizeof(int) + 1 + bytes.Length + 1];
            var current = 0;
            
            foreach (var b in ProtocolIdBytes)
                datagram[current++] = b;

            foreach (var b in BitConverter.GetBytes(packetId))
                datagram[current++] = b;

            datagram[current++] = (byte)packetType;

            foreach (var b in bytes)
                datagram[current++] = b;

            datagram[current] = (byte)datagram.Aggregate(0, (checkSum, b) => checkSum ^ b);
            
            return datagram;
        }

        public static (long, int, PacketType, byte[], bool) Parse(byte[] bytes)
        {
            var current = 0;
            
            var protocolId = BitConverter.ToInt64(bytes, current);
            current += sizeof(long);

            var packetId = BitConverter.ToInt32(bytes, current);
            current += sizeof(int);
            
            var packetType = (PacketType)bytes[current];
            current++;

            var data = bytes.Take(bytes.Length - 1).Skip(current).ToArray();
            
            var isValid = bytes.Aggregate(0, (checkSum, b) => checkSum ^ b) == 0;

            return (protocolId, packetId, packetType, data, isValid);
        }
    }
}