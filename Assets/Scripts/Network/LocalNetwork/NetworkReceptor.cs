using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Network.Common;
using UnityEngine;

namespace Network.LocalNetwork
{
    public class NetworkReceptor : MonoBehaviour
    {
        public readonly Action<ServerStatus> OnServerBroadcast;

        private UdpClient _udpClient;
        private const int BroadcastPort = 8888;
        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            // SetUp Udp Client and EndPoint
            _udpClient = new UdpClient(BroadcastPort);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, BroadcastPort);

            // Enable send and receive broadcast messages and made the socket reusable by other clients
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            _udpClient.Client.Bind(endPoint);

            // Creation of Cancellation token
            _cancellationTokenSource = new CancellationTokenSource();
            
            ServerListening(_cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid ServerListening(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    UdpReceiveResult receiveResult = await _udpClient.ReceiveAsync().AsUniTask();
                    string message = Encoding.ASCII.GetString(receiveResult.Buffer);
                    
                    ProcessBroadcastMessage(message);
                }
            }
            catch (ObjectDisposedException)
            {
                Debug.Log("UdpClient has been disposed.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception in listening task: {ex.Message}");
            }
        }
        
        private void ProcessBroadcastMessage(string message)
        {
            ServerStatus serverStatus = new ServerStatus(message);
            OnServerBroadcast.Invoke(serverStatus);
        }
        
    }
}