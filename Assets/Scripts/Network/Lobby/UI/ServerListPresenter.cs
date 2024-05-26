using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Network.Lobby.UI
{
    public class ServerListPresenter : MonoBehaviour
    {
        [SerializeField] private ServerPresenter serverPresenter;
        [SerializeField] private List<ServerPresenter> serversActives;
        
        public void Initialize()
        {
            serversActives = new List<ServerPresenter>();
        }

        public void AddServer(string ip, UnityAction action)
        {
            ServerPresenter instance = Instantiate(serverPresenter, transform);
            instance.Initialize(ip, action);
            serversActives.Add(instance);
        }

        public void RemoveServer(string ip)
        {
            ServerPresenter server = serversActives.Find(server => server.IP == ip);
            if (!server) return;
            
            server.Close();
            serversActives.Remove(server);
        }

        public void Close()
        {
            foreach (ServerPresenter server in serversActives)
            {
                server.Close();
            }
            Destroy(gameObject);
        }
    }
}