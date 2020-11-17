using System.Collections.Generic;
using System.Linq;

namespace ShooterClient
{
    public class FiniteQueue
    {
        public const int MaxLength = 8;

        public readonly List<byte[]> Items = new List<byte[]>();

        public void Add(byte[] action)
        {
            if (Items.Count == MaxLength)
                Items.RemoveAt(0);
        
            Items.Add(action);
        }

        public byte[] Join()
        {
            var n = Items.Sum(item => item.Length);
        
            var bytes = new byte[n];
            var current = 0;
            
            foreach (var item in Items)
                foreach (var b in item)
                    bytes[current++] = b;

            return bytes;
        }
    }
}