using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using Network.Common;
using Network.LocalNetwork;
using UnityEngine;

namespace Network
{
    public class NetworkConnection : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private NetworkConfiguration networkConfiguration;
        
        private NetworkBroadcast _networkBroadcast;
        
        private Tugboat _transport;
        private LocalConnectionState _serverState;
        private LocalConnectionState _clientState;

        private void Start()
        {
            _transport = networkManager.TransportManager.GetTransport<Tugboat>();
            _networkBroadcast = new NetworkBroadcast(networkConfiguration);
        }
        
        
        private void OnEnable()
        {
            networkManager.ServerManager.OnServerConnectionState += On_ServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState += On_ClientConnectionState;
        }

        private void OnDisable()
        {
            networkManager.ServerManager.OnServerConnectionState -= On_ServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState -= On_ClientConnectionState;
        }
        
        private void On_ServerConnectionState(ServerConnectionStateArgs stateArgs)
        {
            _serverState = stateArgs.ConnectionState;
            ushort port = _transport.GetPort();
            
            switch (_serverState)
            {
                case LocalConnectionState.Started:
                    _networkBroadcast.SendBroadcast(ServerStatusEnum.Available, port);
                    break;
                case LocalConnectionState.Stopping:
                case LocalConnectionState.Stopped:
                    _networkBroadcast.SendBroadcast(ServerStatusEnum.UnAvailable, port);
                    break;
            }
        }

        private void On_ClientConnectionState(ClientConnectionStateArgs stateArgs)
        {
            _clientState = stateArgs.ConnectionState;
        }

        public void StartServer(string ipV6, ushort port)
        {
            if (_serverState != LocalConnectionState.Stopped)
            {
                networkManager.ServerManager.StopConnection(true);
                return;
            }

            _transport.SetServerBindAddress(ipV6, IPAddressType.IPv6);
            _transport.SetPort(port);

            networkManager.ServerManager.StartConnection();
            networkManager.ClientManager.StartConnection();
        }
        
    }
}