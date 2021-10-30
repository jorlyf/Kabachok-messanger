using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Sockets;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Net;

using Serializer;
using Serializer.Data;
using System.Windows.Data;

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

        private TcpClient TcpClient;
        private StreamWriter SW;
        private StreamReader SR;
        public ConnectionAddress ConnectionAddress { get; set; } = new ConnectionAddress();
        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        private string _Name;
        public string Name { get => _Name; set => Set(ref _Name, value); }

        public ClientUser()
        {
            TcpClient = new TcpClient();

            object lockobj = new object();
            BindingOperations.EnableCollectionSynchronization(Messages, lockobj); // Messages.Add() invoke in main thread
        }
        public void Connect()
        {
            if (IsConnected) return;
            try
            {
                Disconnect();
                TcpClient = new TcpClient();
                TcpClient.Connect(IPAddress.Parse(ConnectionAddress.IP), ConnectionAddress.Port);
                SW = new StreamWriter(TcpClient.GetStream());
                SR = new StreamReader(TcpClient.GetStream());

                Registration registration = new Registration(Name);
                SendData sendData = new SendData(registration, Name);

                string data = SerializeService.Serialize(sendData);

                ListenMessages();
                UpdateView();

                SW.WriteLine(data);
                SW.Flush();
            }
            catch (Exception e)
            {
                MessageBox.Show("ОШИБКА\n" + e.Message);
                UpdateView();
            }
        }
        public void Disconnect()
        {
            if (!IsConnected) return;
            try
            {
                Disconnect disconnect = new Disconnect(Name);
                SendData sendData = new SendData(disconnect, Name);
                string data = SerializeService.Serialize(sendData);
                SW.WriteLine(data);
                SW.Close();

                TcpClient.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Не удалось корректно отключиться от сервера\n\n{e.Message}");
            }
            UpdateView();
        }
        private async void ListenMessages()
        {
            await Task.Run(() =>
            {
                try
                {
                    while (IsConnected)
                    {
                        string data = SR.ReadLine();
                        if (data != null)
                        {

                            SendData sendData = SerializeService.Deserialize(data);
                            if (sendData.Data is ServerMessage serverMessage)
                            {
                                if (serverMessage.Key == 401)
                                    TcpClient.Close();

                                Messages.Add((Message)serverMessage);
                            }
                            else if (sendData.Data is Message message)
                                Messages.Add(message);


                            UpdateView();
                        }
                    }
                }
                catch (Exception) { UpdateView(); }
            });
        }
        public async void SendMessage(string text)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(text)) return;

                Message message = new Message(text, Name);
                SendData sendData = new SendData(message, Name);

                string data = SerializeService.Serialize(sendData);
                SW.WriteLine(data);
                SW.Flush();

                UpdateView();
            });
        }


        public bool IsConnected { get => TcpClient.Connected; }
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
