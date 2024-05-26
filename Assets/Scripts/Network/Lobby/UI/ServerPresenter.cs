using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Network.Lobby.UI
{
    public class ServerPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text serverIp;
        [SerializeField] private Button button;
        
        public string IP
        {
            get;
            private set;
        }

        public void Initialize(string ip, UnityAction action)
        {
            IP = ip;
            serverIp.text = ip;
            button.onClick.AddListener(action);
        }

        public void Close()
        {
            button.onClick.RemoveAllListeners();
            Destroy(gameObject);
        }
        
    }
}