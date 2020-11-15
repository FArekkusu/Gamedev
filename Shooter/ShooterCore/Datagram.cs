using System;
using System.Linq;

namespace ShooterCore
{
    public static class Datagram
    {
        public const long ProtocolId = 546886159060960807;
        
        public static byte[] ProtocolIdBytes = BitConverter.GetBytes(ProtocolId);
        
        public static byte[] Build(PacketType packetType, byte[] bytes = null)
        {
            bytes ??= new byte[0];
            
            var datagram = new byte[1 + sizeof(long) + bytes.Length];
            
            for (var i = 0; i < sizeof(long); i++)
                datagram[i] = ProtocolIdBytes[i];

            datagram[sizeof(long)] = (byte)packetType;

            for (var i = 0; i < bytes.Length; i++)
                datagram[i + sizeof(long) + 1] = bytes[i];
            
            return datagram;
        }

        public static (long, PacketType, byte[]) Parse(byte[] bytes)
        {
            var protocolId = BitConverter.ToInt64(bytes, 0);
            
            var packetType = (PacketType)bytes[sizeof(long)];

            var data = bytes.Skip(1 + sizeof(long)).ToArray();

            return (protocolId, packetType, data);
        }
    }
}