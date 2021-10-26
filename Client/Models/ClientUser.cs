using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Sockets;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using System.Net;

using Serializer;
using Serializer.Data;

namespace Client.Models
{
    internal class ClientUser : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
        #endregion

        private TcpClient tcpClient;
        public ConnectionAddress ConnectionAddress { get; set; } = new ConnectionAddress();
        private string _Name;
        public string Name { get => _Name; set => Set(ref _Name, value); }

        public ClientUser()
        {
            tcpClient = new TcpClient();
        }
        public void Connect()
        {
            if (!IsConnected)
            {
                try
                {
                    tcpClient = new TcpClient();
                    tcpClient.Connect(IPAddress.Parse(ConnectionAddress.IP), ConnectionAddress.Port);
                    ListenMessages();
                    UpdateView();
                }
                catch (Exception e)
                {
                    MessageBox.Show("ОШИБКА\n" + e.Message);
                }
            }
        }
        public void Disconnect()
        {
            tcpClient.Close();
            UpdateView();
        }
        public async void SendMessage(string text)
        {
            await Task.Run(() =>
            {
                if (IsConnected)
                {
                    Message message = new Message(text, Name);
                    SendData sendData = new SendData(message, Name);

                    string data = SerializeService.Serialize(sendData);

                    using NetworkStream stream = tcpClient.GetStream();
                    using StreamWriter sw = new StreamWriter(stream);

                    sw.Write(data);
                    sw.Flush();
                }
            });
        }
        private async void ListenMessages()
        {
            await Task.Run(() =>
            {
                while (IsConnected)
                {
                    try
                    {
                        using NetworkStream stream = tcpClient.GetStream();
                        using StreamReader sr = new StreamReader(stream);

                        string data = sr.ReadToEnd();
                        MessageBox.Show(data);
                        //if (data.Length > 0)
                        //{
                        //    SendData sendData = SerializeService.Deserialize(data);
                        //    if (sendData.Data is Message message)
                        //    {
                        //        MessageBox.Show(message.Text);
                        //    }
                        //}
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("ОШИБКА при получении сообщения от сервера:\n" + e.Message);
                    }
                }
            });
        }


        public bool IsConnected { get => tcpClient.Connected; }
        public string ConnectionInfo { get => IsConnected ? "Подключен" : "Нет соединения"; }
        public string ConnectCommandText { get => IsConnected ? "Отключиться" : "Подключиться"; }
        private void UpdateView()
        {
            OnPropertyChanged("IsConnected");
            OnPropertyChanged("ConnectionInfo");
            OnPropertyChanged("ConnectCommandText");
        }
    }
}
