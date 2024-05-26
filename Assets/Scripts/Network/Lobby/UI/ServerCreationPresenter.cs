using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Network.Lobby.UI
{
    public class ServerCreationPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button createButton;

        public string InputText => inputField.text;
        
        public void Initialize(UnityAction action)
        {
            createButton.onClick.AddListener(action);
        }

        public void Close()
        {
            createButton.onClick.RemoveAllListeners();
            Destroy(gameObject);
        }
    }
}