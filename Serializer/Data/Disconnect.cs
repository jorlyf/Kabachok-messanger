using System;

namespace Serializer.Data
{
    public class Disconnect
    {
        public string SenderLogin { get; set; }
        public string Time { get; set; } = DateTime.Now.ToLocalTime().ToShortTimeString();

        public Disconnect(string login)
        {
            SenderLogin = login;
        }
    }
}
