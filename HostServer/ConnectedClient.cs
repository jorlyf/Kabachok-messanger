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
        public DateTime ConnectedDate;
        public ConnectedClient(TcpClient tcpClient, string name)
        {
            TcpClient = tcpClient;
            Name = name;
            ConnectedDate = DateTime.Now;
        }
        public async void SendMessage(SendData sendData)
        {
            await Task.Run(() =>
            {
                StreamWriter sw = new StreamWriter(TcpClient.GetStream());

                string data = SerializeService.Serialize(sendData);
                Console.WriteLine($"{sendData.Date} {sendData.Data.SenderLogin} {sendData.Data.Text}");

                sw.WriteLine(data);
                sw.Flush();

            });
        }
    }
}
