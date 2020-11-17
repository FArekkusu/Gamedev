using System;
using System.Net;

namespace ShooterServer
{
    public struct Connection
    {
        public IPEndPoint Address;
        public bool IsPresent;
        public DateTime LastReceivedTime;
        public int LastReceivedId;
    }
}