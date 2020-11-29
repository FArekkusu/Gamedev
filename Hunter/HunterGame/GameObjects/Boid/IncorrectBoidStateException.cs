using System;

namespace HunterGame.GameObjects.Boid
{
    public class IncorrectBoidStateException : Exception
    {
        public IncorrectBoidStateException() {}
        
        public IncorrectBoidStateException(string message) : base(message) {}
        
        public IncorrectBoidStateException(string message, Exception inner) : base(message, inner) {}
    }
}