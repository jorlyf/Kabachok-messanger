using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;

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
                        try
                        {
                            TcpClient tcpClient = this.AcceptTcpClient();
                            SendData sendData;

                            StreamReader sr = new StreamReader(tcpClient.GetStream());
                            string data = sr.ReadLine(); // единоразово проводит регистрацию
                            sendData = SerializeService.Deserialize(data);

                            if (sendData.Data is Registration registration)
                            {
                                RegistrateConnectedClient(tcpClient, registration);
                            }

                        }
                        catch (Exception)
                        {
                            Console.WriteLine("ОШИБКА: не получилось зарегистрировать клиента");
                        }
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
            await Task.Run(() =>
            {
                try
                {
                    StreamReader sr = new StreamReader(connectedClient.TcpClient.GetStream());
                    while (this.Active && connectedClient.TcpClient.Connected)
                    {
                        string data = sr.ReadLine();
                        if (data != null)
                        {
                            SendData sendData = SerializeService.Deserialize(data);

                            ProcessData(sendData);
                        }

                        // await Task.Delay(20);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"ОШИБКА: {connectedClient.Name} разорвал соединение");
                    DisconnectConnectedClient(connectedClient);
                }
            });
        }
        private async void StartPingConnectedClient(ConnectedClient connectedClient)
        {
            await Task.Run(() =>
            {
                
            });
        }
        private void RegistrateConnectedClient(TcpClient tcpClient, Registration registration)
        {
            if (Clients.Any(client => client.Name == registration.SenderLogin))
            {
                StreamWriter sw = new StreamWriter(tcpClient.GetStream());
                ServerMessage serverMessage = new ServerMessage($"Никнейм {registration.SenderLogin } уже занят!", "СЕРВЕР", 401);
                SendData sendData = new SendData(serverMessage, "СЕРВЕР");
                string data = SerializeService.Serialize(sendData);
                sw.WriteLine(data);
                sw.Close();

                tcpClient.Close();
                return;
            }

            ConnectedClient connectedClient = new ConnectedClient(tcpClient, registration.SenderLogin);

            Clients.Add(connectedClient);
            StartListenConnectedClient(connectedClient);
            StartPingConnectedClient(connectedClient);

            Console.WriteLine($"{connectedClient.ConnectedDate} Новое подключение: {connectedClient.Name}");
        }
        private async void ProcessData(SendData sendData)
        {
            await Task.Run(() =>
            {
                if (sendData.Data is Message)
                {
                    SendMessagesToAll(sendData);
                }
                else if (sendData.Data is Disconnect disconnect)
                {
                    ConnectedClient connectedClient = Clients.FirstOrDefault(client => client.Name == disconnect.SenderLogin);
                    DisconnectConnectedClient(connectedClient);
                }
            });
        }
        private void SendMessagesToAll(SendData sendData)
        {
            Clients.ForEach(client =>
            {
                try
                {
                    client.SendMessage(sendData);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ОШИБКА при переадресации сообщения. Удаляю пользователя {client.Name}");
                    DisconnectConnectedClient(client);
                }
            });
        }
        private void DisconnectConnectedClient(ConnectedClient connectedClient)
        {
            if (connectedClient == null) return;

            Clients.Remove(connectedClient);
            Console.WriteLine($"{DateTime.Now.ToLocalTime()} {connectedClient.Name} был отключен");
        }
    }
}
