using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace HunterGame
{
    public static class Extensions
    {
        public static Vector2 Limit(this Vector2 vector, double maxLength)
        {
            var length = vector.Length();

            if (length > maxLength)
            {
                vector /= length;
                vector *= (float)maxLength;
            }
            
            return vector;
        }

        public static TSource MinBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
                throw new ArgumentException("MinBy source cannot be null");
            
            if (selector == null)
                throw new ArgumentException("MinBy selector cannot be null");
            
            return source.Select(element => (selector(element), element)).Min().Item2;
        }
    }
}