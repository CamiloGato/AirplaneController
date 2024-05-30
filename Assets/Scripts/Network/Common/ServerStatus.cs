using System;
using System.Text;

namespace Network.Common
{
    public class ServerStatus
    {
        private readonly ServerStatusEnum _serverStatusEnum;
        private readonly ushort _port;
        private readonly string _ip;

        public bool IsAvailable => _serverStatusEnum == ServerStatusEnum.Available;
        public string Ip => $"{_ip}:{_port}";

        public ServerStatus(ServerStatusEnum serverStatusEnum, ushort port, string ip)
        {
            _serverStatusEnum = serverStatusEnum;
            _port = port;
            _ip = ip;
        }

        public ServerStatus(string text)
        {
            string[] parts = text.Split('|');
            if (parts.Length == 3)
            {
                string status = parts[0];
                string serverIP = parts[1];
                ushort serverPort = ushort.Parse(parts[2]);

                if (!Enum.TryParse(status, out ServerStatusEnum result))
                {
                    return;
                }

                _serverStatusEnum = result;
                _port = serverPort;
                _ip = serverIP;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(_ip);
            builder.Append('|');
            builder.Append(_port);
            builder.Append('|');
            builder.Append(_serverStatusEnum.ToString());
            
            return builder.ToString();
        }
    }
}