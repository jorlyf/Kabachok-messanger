using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

using Serializer;

namespace HostServer
{
    internal class ConnectedClient
    {
        public TcpClient TcpClient;
        public string Name;
        public string ConnectedDate;
        public ConnectedClient(TcpClient tcpClient, string name)
        {
            TcpClient = tcpClient;
            Name = name;
            ConnectedDate = DateTime.Now.ToLocalTime().ToString();
        }
        public async void SendMessage(SendData message)
        {
            await Task.Run(() =>
            {
                try
                {
                    string data = SerializeService.Serialize(message);

                    NetworkStream stream = TcpClient.GetStream();
                    StreamWriter sw = new StreamWriter(stream);

                    sw.Write(data);
                    sw.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine("ОШИБКА при переадресации сообщения другим пользователям:\n" + e.Message);
                }
            });
        }
    }
}
