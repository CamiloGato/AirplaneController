using System.Net;
using System.Net.Sockets;
using System.Text;
using LiteNetLib;
using Network.Common;
using UnityEngine;

namespace Network.LocalNetwork
{
    public class NetworkBroadcast : MonoBehaviour
    {
        private const int BroadcastPort = 8888;
        private UdpClient _udpClient;

        private void Start()
        {
            // SetUp UdpClient
            _udpClient = new UdpClient();
            
            // Enable send and receive broadcast messages
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            
        }

        private void SendBroadcast(ServerStatusEnum statusEnum,ushort port)
        {
            string localIp = NetUtils.GetLocalIp(LocalAddrType.IPv6);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, BroadcastPort);
            string message = $"{statusEnum}|{localIp}|{port}";
            byte[] data = Encoding.ASCII.GetBytes(message);

            _udpClient.Send(data, data.Length, endPoint);
        }
        
        
    }
}