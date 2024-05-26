using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using UnityEngine;

namespace Network.Lobby
{
    public class ConnectionManager : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private ServerBroadcaster serverBroadcaster;

        private Tugboat _transport;
        private LocalConnectionState _serverState;
        private LocalConnectionState _clientState;
        
        private void Start()
        {
            _transport = networkManager.TransportManager.GetTransport<Tugboat>();
        }

        private void OnEnable()
        {
            networkManager.ServerManager.OnServerConnectionState += ServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState += ClientConnectionState;
        }

        private void OnDisable()
        {
            networkManager.ServerManager.OnServerConnectionState -= ServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState -= ClientConnectionState;
        }
        
        private void ServerConnectionState(ServerConnectionStateArgs stateArgs)
        {
            _serverState = stateArgs.ConnectionState;

            switch (_serverState)
            {
                case LocalConnectionState.Started:
                    serverBroadcaster.BroadcastServer(_transport.GetPort().ToString(), "ServerAvailable");
                    break;
                case LocalConnectionState.Stopping:
                case LocalConnectionState.Stopped:
                    serverBroadcaster.BroadcastServer(_transport.GetPort().ToString(), "ServerUnAvailable");
                    break;
            }
        }

        private void ClientConnectionState(ClientConnectionStateArgs stateArgs)
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

        public void StartClient(string ipV6, ushort port)
        {
            if (_clientState != LocalConnectionState.Stopped)
            {
                networkManager.ClientManager.StopConnection();
                return;
            }

            _transport.SetClientAddress(ipV6);
            _transport.SetPort(port);

            networkManager.ClientManager.StartConnection();
        }
    }
}