using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

using Serializer;
using Serializer.Data;

namespace HostServer
{
    internal class Server : TcpListener
    {
        private readonly IPAddress IP;
        private readonly int Port;
        private readonly List<ConnectedClient> Clients = new List<ConnectedClient>();

        public Server(IPAddress ip, int port) : base(ip, port)
        {
            IP = ip;
            Port = port;
        }
        public async void StartListen() // Подключает новых клиентов
        {
            await Task.Run(() =>
            {
                try
                {
                    this.Start();
                    Console.WriteLine($"Сервер запущен на {IP}:{Port}");
                    while (this.Active)
                    {
                        TcpClient tcpClient = this.AcceptTcpClient();
                        ConnectedClient connectedClient = new ConnectedClient(tcpClient, "vasya");

                        Clients.Add(connectedClient);
                        StartListenConnectedClient(connectedClient);
                        Console.WriteLine($"{connectedClient.ConnectedDate} Новое подключение: {connectedClient.Name}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ОШИБКА в слушании новых подключений: {e.Message}");
                    Console.WriteLine("Перезапустите приложение и попробуйте снова!");
                    Console.ReadKey();

                    Environment.Exit(-1);
                }
            });
        }
        public void StopListen()
        {
            this.Stop();
        }

        private async void StartListenConnectedClient(ConnectedClient connectedClient)
        {
            await Task.Run(async () =>
            {
                try
                {
                    using NetworkStream stream = connectedClient.TcpClient.GetStream();
                    using StreamReader sr = new StreamReader(stream);

                    while (this.Active && connectedClient.TcpClient.Connected)
                    {
                        string strData = sr.ReadToEnd();
                        if (strData.Length > 0)
                        {
                            Console.WriteLine(strData);
                            //SendData sendData = SerializeService.Deserialize(strData);
                            //ProcessData(sendData);
                        }

                        await Task.Delay(20);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ОШИБКА: " + e.Message);
                }
            });
        }
        private async void ProcessData(SendData sendData)
        {
            await Task.Run(() =>
            {
                if (sendData.Data is Message)
                {
                    SendMessagesToAll(sendData);
                }
            });
        }
        private void SendMessagesToAll(SendData message)
        {
            Clients.ForEach(client =>
            {
                client.SendMessage(message);
            });
        }
    }
}
