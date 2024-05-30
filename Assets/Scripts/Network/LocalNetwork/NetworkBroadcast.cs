using System.Net;
using System.Net.Sockets;
using System.Text;
using LiteNetLib;
using Network.Common;

namespace Network.LocalNetwork
{
    public class NetworkBroadcast
    {
        private readonly UdpClient _udpClient;
        private readonly NetworkConfiguration _networkConfiguration;
        
        public NetworkBroadcast(NetworkConfiguration networkConfiguration)
        {
            // Store and inject configuration
            _networkConfiguration = networkConfiguration;
            
            // SetUp UdpClient
            _udpClient = new UdpClient();
            
            // Enable send and receive broadcast messages
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            
        }

        public void SendBroadcast(ServerStatusEnum statusEnum, ushort port, string localIp = "")
        {
            if (localIp == "")
            {
                localIp = NetUtils.GetLocalIp(LocalAddrType.IPv6);
            }
            
            ushort broadcastPort = _networkConfiguration.broadcastPort;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
            ServerStatus serverStatus = new ServerStatus(statusEnum, port, localIp);
            string message = serverStatus.ToString();
            byte[] data = Encoding.ASCII.GetBytes(message);

            _udpClient.Send(data, data.Length, endPoint);
        }
        
    }
}