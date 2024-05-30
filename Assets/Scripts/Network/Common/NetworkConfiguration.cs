using UnityEngine;

namespace Network.Common
{
    [CreateAssetMenu(menuName = "Network/Configuration", fileName = "NetworkConfiguration")]
    public class NetworkConfiguration : ScriptableObject
    {
        public ushort broadcastPort;
    }
}