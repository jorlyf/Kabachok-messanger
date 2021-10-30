using System;

namespace Serializer.Data
{
    public class Registration
    {
        public string SenderLogin { get; set; }
        public string Time { get; set; } = DateTime.Now.ToLocalTime().ToShortTimeString();

        public Registration(string login)
        {
            SenderLogin = login;
        }
    }
}
