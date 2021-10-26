using System;
using System.Net;

namespace HostServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Address address = GetAddress();
            Server server = new Server(address.IP, address.Port);
            server.StartListen();

            Console.ReadLine();
        }

        static Address GetAddress()
        {
            Console.WriteLine("Введите Ваш IP-адрес и порт которые будет слушать сервер");
            Console.WriteLine("пример: 19.221.74.162:7550\n");

            do
            {
                try
                {
                    string input = Console.ReadLine();
                    IPAddress ip = IPAddress.Parse(input.Split(":")[0]);
                    int port = int.Parse(input.Split(":")[1]);

                    return new Address(ip, port);
                }
                catch { Console.WriteLine("ОШИБКА: Неправильный формат ввода!"); }
            }
            while (true);
        }
    }

    struct Address
    {
        public IPAddress IP;
        public int Port;
        public Address(IPAddress ip, int port)
        {
            IP = ip;
            Port = port;
        }
    }
}
