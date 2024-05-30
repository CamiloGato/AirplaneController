using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Network.Common;

namespace Network.LocalNetwork
{
    public class NetworkReceptor
    {
        public readonly Action<ServerStatus> OnServerBroadcast;
        
        private readonly UdpClient _udpClient;
        private readonly NetworkConfiguration _networkConfiguration;

        public NetworkReceptor(NetworkConfiguration networkConfiguration)
        {
            // Store and inject
            _networkConfiguration = networkConfiguration;
            
            // SetUp Udp Client and EndPoint
            ushort broadcastPort = _networkConfiguration.broadcastPort;
            _udpClient = new UdpClient(broadcastPort);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);

            // Enable send and receive broadcast messages and made the socket reusable by other clients
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            _udpClient.Client.Bind(endPoint);

            // Creation of Cancellation token
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            
            ServerListening(cancellationTokenSource.Token).Forget();
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
                UnityEngine.Debug.Log("UdpClient has been disposed.");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Exception in listening task: {ex.Message}");
            }
        }
        
        private void ProcessBroadcastMessage(string message)
        {
            ServerStatus serverStatus = new ServerStatus(message);
            OnServerBroadcast.Invoke(serverStatus);
        }
        
    }
}