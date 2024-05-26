using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Network.Lobby
{
    public class ServerDiscovery : MonoBehaviour
    {
        public Action<string, ushort, bool> OnServerBroadcast;
        
        private UdpClient _udpClient;
        private const int BroadcastPort = 8888;
        private CancellationTokenSource _cancellationTokenSource;
        
        private void Start()
        {
            UdpClient udpClient = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, BroadcastPort);
            
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(endPoint);
            
            _udpClient = udpClient;
            _cancellationTokenSource = new CancellationTokenSource();
            ServerListening(_cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid ServerListening(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    UdpReceiveResult receivedResults = await _udpClient.ReceiveAsync().AsUniTask();
                    string message = Encoding.ASCII.GetString(receivedResults.Buffer);
                    Debug.Log($"Received broadcast: {message}");
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
            string[] parts = message.Split('|');
            if (parts.Length == 3)
            {
                string serverIP = parts[1];
                ushort serverPort = ushort.Parse(parts[2]);
                
                if (parts[0] == "ServerAvailable")
                {
                    Debug.Log($"Server found at {serverIP}:{serverPort}");
                    OnServerBroadcast.Invoke(serverIP, serverPort, true);
                }
                else
                {
                    Debug.Log($"Server stopped at {serverIP}:{serverPort}");
                    OnServerBroadcast.Invoke(serverIP, serverPort, false);
                }
                
            }
        }

        private void StopListening()
        {
            if (_cancellationTokenSource == null) return;
            
            _cancellationTokenSource.Cancel();
            _udpClient.Close();
        }
        
        private void OnDestroy()
        {
            StopListening();
        }
    }
}