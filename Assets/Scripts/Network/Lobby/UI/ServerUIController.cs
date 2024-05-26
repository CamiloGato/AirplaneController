using System.Collections.Generic;
using LiteNetLib;
using Unity.Collections;
using UnityEngine;

namespace Network.Lobby.UI
{
    public class ServerUIController : MonoBehaviour
    {
        [SerializeField] private ServerDiscovery serverDiscovery;
        [SerializeField] private ConnectionManager connectionManager;
        [SerializeField] private ServerCreationPresenter serverCreationPresenter;
        [SerializeField] private ServerListPresenter serverListPresenter;

        [ReadOnly] public List<string> ips;

        private void OnEnable()
        {
            ips = new List<string>();
            serverDiscovery.OnServerBroadcast += AddRemoveServer;
            serverCreationPresenter.Initialize(OnButtonCreatePressed);
            serverListPresenter.Initialize();
        }
        
        private void OnButtonCreatePressed()
        {
            ushort port = ushort.Parse(serverCreationPresenter.InputText);
            string ipV6 = NetUtils.GetLocalIp(LocalAddrType.IPv6);
            connectionManager.StartServer(ipV6, port);
        }

        private void AddRemoveServer(string serverIp, ushort serverPort, bool isOpen)
        {
            string ip = $"{serverIp}:{serverPort}";
            if (isOpen)
            {
                if (ips.Contains(ip)) return;
                
                ips.Add(ip);
                serverListPresenter.AddServer(ip, () => connectionManager.StartClient(serverIp, serverPort) );
            }
            else
            {
                ips.Remove(ip);
                serverListPresenter.RemoveServer(ip);
            }
        }

        private void OnDisable()
        {
            serverDiscovery.OnServerBroadcast -= AddRemoveServer;
            serverCreationPresenter.Close();
            serverListPresenter.Close();
        }
    }
}