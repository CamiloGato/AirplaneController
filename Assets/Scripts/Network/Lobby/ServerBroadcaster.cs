using System.Net;
using System.Net.Sockets;
using System.Text;
using LiteNetLib;
using UnityEngine;

namespace Network.Lobby
{
    public class ServerBroadcaster : MonoBehaviour
    {
        private UdpClient _udpClient;
        private const int BroadcastPort  = 8888;

        private void Start()
        {
            _udpClient = new UdpClient();
            _udpClient.EnableBroadcast = true;
        }

        public void BroadcastServer(string serverPort, string state)
        {
            string localIP = NetUtils.GetLocalIp(LocalAddrType.IPv6);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, BroadcastPort);

            string message = $"{state}|{localIP}|{serverPort}";
            byte[] data = Encoding.ASCII.GetBytes(message);

            _udpClient.Send(data, data.Length, endPoint);
        }

        private void OnDestroy()
        {
            _udpClient.Close();
        }
    }
}