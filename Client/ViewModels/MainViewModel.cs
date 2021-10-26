using System.Windows.Input;

using Client.ViewModels.Base;
using Client.Models;
using Client.Infrastructure.Commands;

namespace Client.ViewModels
{
    internal class MainViewModel : ViewModel
    {
        public ClientUser User { get; set; }
        public MainViewModel()
        {
            User = new ClientUser();
            ConnectCommand = new RelayCommand(OnConnectCommandExecuted);
            SendMessageCommand = new RelayCommand(OnSendMessageCommandExecuted, CanSendMessageCommandExecute);
        }

        #region ConnectCommand
        public ICommand ConnectCommand { get; }
        private void OnConnectCommandExecuted(object p)
        {
            if (!User.IsConnected) User.Connect();
            else User.Disconnect();
        }
        #endregion

        #region
        public string MessageText { get; set; }
        public ICommand SendMessageCommand { get; }
        private void OnSendMessageCommandExecuted(object p)
        {
            User.SendMessage(MessageText);
        }
        private bool CanSendMessageCommandExecute(object p) => User.IsConnected;
        #endregion
    }
}
