using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ShooterCore
{
    public static class SafeCommunicator
    {
        public static async Task<(bool, UdpReceiveResult)> Receive(UdpClient client)
        {
            try
            {
                var datagram = await client.ReceiveAsync();

                return (true, datagram);
            }
            catch (Exception)
            {
                return (false, new UdpReceiveResult());
            }
        }

        public static async Task<bool> Send(UdpClient client, byte[] datagram)
        {
            try
            {
                await client.SendAsync(datagram, datagram.Length);
                
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }
        
        public static async Task<bool> Send(UdpClient client, byte[] datagram, IPEndPoint endpoint)
        {
            try
            {
                await client.SendAsync(datagram, datagram.Length, endpoint);
                
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}